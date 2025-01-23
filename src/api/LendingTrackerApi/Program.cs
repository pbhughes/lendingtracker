using LendingTrackerApi.Extensions;
using LendingTrackerApi.Models;
using LendingTrackerApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using LendingTrackerApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Azure.Communication.Sms;
using Azure.Communication.Email;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;





var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

//Add CORS Support
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin();
                          policy.AllowAnyMethod();
                          policy.AllowAnyHeader();
                      });
});

// Configure DbContext
builder.Services.AddDbContext<LendingTrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), options =>
    {
        options.CommandTimeout(6);
    }));

// Add services to the container.
builder.Services.AddControllers();

// Adding SignalR
builder.Services.AddSignalR();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LendingTracker API", Version = "v1" });
    c.EnableAnnotations();
});

//add authentication and authorization

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"))
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddInMemoryTokenCaches();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    // Specify the valid audiences (array of audience values)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidAudiences = new[] { "806b17b5-8f69-46d7-b9f8-26fff192a38f" } // List multiple valid audiences
    };
});


builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("authorized_user", policy =>
        policy.RequireClaim("http://schemas.microsoft.com/identity/claims/scope", "lender"));

    options.AddPolicy("admin", policy => 
        policy.RequireClaim("extension_userRole", "admin"));

});

//add SMS config support
// Bind configuration section
builder.Services.Configure<MessageSMSSettings>(
    builder.Configuration.GetSection("MessageSMS"));

builder.Services.AddScoped<MessageSMS>();

//inject model validators and services
builder.Services.AddScoped(typeof(IHmacSigner), typeof(HmacSigner));
builder.Services.AddScoped(typeof(IMessageMailer), typeof(MessageMailer));
builder.Services.AddScoped<MessageSignalr>();
builder.Services.AddScoped(typeof(IValidationServices), typeof(ValidatorService));


var app = builder.Build();

// Use CORS
app.UseCors(MyAllowSpecificOrigins);

//use routing for signlR
app.UseRouting();
app.MapHub<MessagesHub>("/messagehub");

//use authorization
app.UseAuthentication();
app.UseAuthorization();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
app.UseReDoc(c =>
{
    // c.SwaggerEndpoint("/swagger/v1/swagger.json", "LendingTracker API V1");
    // c.RoutePrefix = string.Empty; // Set the Swagger UI at the app's root
    c.DocumentTitle = "LendingTracker API Docs";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RoutePrefix = ""; // Set the URL where Redoc will be served
    c.InjectStylesheet("/styles/redoc.css");
});



// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseExceptionHandler("/error");

app.MapGet("/error", (HttpContext context) =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    return exception is DbUpdateException
        ? Results.Problem("A database error occurred.", statusCode: 500)
        : Results.Problem("An unexpected error occurred.", statusCode: 500);
});

// Define CRUD endpoints for User*************************************************User***********************************
app.MapGet("/users", async (LendingTrackerContext db, IHttpContextAccessor httpContext) => {

    if (httpContext == null) return Results.Unauthorized();

    var id = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    if (id is null) return Results.Unauthorized();


    if (!Guid.TryParse(id, out var parsedGuid))
        return Results.Unauthorized();

    return await db.Users.FindAsync(parsedGuid) is not User user ?  Results.NotFound() :  Results.Ok(user);  

}).WithTags("Users").RequireAuthorization("authorized_user");

app.MapGet("/users/{id}", async (int id, LendingTrackerContext db) =>
    await db.Users.FindAsync(id) is User user ? Results.Ok(user) : Results.NotFound())
    .WithTags("Users").RequireAuthorization("authorized_user");

app.MapPost("/users", async (User user, LendingTrackerContext db, IValidationServices validationServices, IHttpContextAccessor httpContext) =>
{
    
    
    var validationResult = validationServices.ValidateUser(user);
    if (!validationResult.Valid)
    {
        return Results.BadRequest(validationResult.ErrorMessage);
    }

    try
    {
        //check for duplicates
        bool exists = await db.Users.FindAsync(user.UserId) is User existing ? true : false;
        if (exists)
        {
            return Results.BadRequest($"User {user.Email} already exists");
        }

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/users/{user.UserId}", user);
    }
    catch (DbUpdateException dbEx)
    {
        return Results.Problem(dbEx.FlattenMessages());
    }
    catch (SqlException sqlEx)
    {
        return Results.Problem(sqlEx.FlattenMessages());
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.FlattenMessages());
    }
}).WithTags("Users").RequireAuthorization("authorized_user");


app.MapPut("/users/{id}", async (Guid id, User updatedUser, LendingTrackerContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    user.FullName = updatedUser.FullName;
    user.Email = updatedUser.Email;
    user.PhoneNumber = updatedUser.PhoneNumber;
    user.Address = updatedUser.Address;
    user.CreatedAt = updatedUser.CreatedAt;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Users").RequireAuthorization("authorized_user");

app.MapDelete("/users/{id}", async (int id, LendingTrackerContext db) =>
{
    if (await db.Users.FindAsync(id) is User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.Ok(user);
    }
    return Results.NotFound();
}).WithTags("Users").RequireAuthorization("authorized_user");


// For Borrowers************************************************************************BORROWERS***************************************************

app.MapGet("/borrowers/confirm/{borrowerId}", async ([FromRoute] string borrowerId, [FromQuery] string apikey, LendingTrackerContext db, IHmacSigner signer) => {
   Borrower b = await db.Borrowers.FindAsync(Guid.Parse(borrowerId));
    if (signer.Verify(b.BorrowerEmail, apikey))
    {
        return Results.Ok(b);
    }
    return Results.Unauthorized();

}).WithTags("Borrowers");

app.MapGet("/borrowers", async (LendingTrackerContext db, ClaimsPrincipal currentUser ) =>
{


   var borrowers = await db.Borrowers
            .Include(b => b.Transactions)
            .Where(b => b.UserId == Guid.Parse(currentUser.GetNameIdentifierId())).ToListAsync();

    return borrowers is null ? Results.NotFound() : Results.Ok(borrowers.ToList());

}).WithTags("Borrowers").RequireAuthorization("authorized_user");


app.MapGet("/borrowers/transactions/{borrowerId}", async (string borrowerId,LendingTrackerContext db, ClaimsPrincipal currentUser, IHttpContextAccessor httpContext) =>
{
    if (httpContext == null) return Results.Unauthorized();

    var id = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (id is null) return Results.Unauthorized();

    var transactions = db.Transactions
     .Include(b => b.Borrower)
     .Join(db.Items, tran => tran.ItemId, item => item.ItemId, (trans, item) => 
        new { BorrowedAt = trans.BorrowedAt, ItemName = item.ItemName, BorrorwerId = trans.BorrowerId, LenderId = trans.LenderId, BorrowerId = trans.BorrowerId , ReturnDate = trans.ReturnDate})
     .Where(t => t.BorrowerId == Guid.Parse(borrowerId) && t.LenderId == Guid.Parse(id));
   
    return transactions is null ? Results.NotFound() : Results.Ok(transactions.ToList());

}).WithTags("Borrowers").RequireAuthorization("authorized_user");



app.MapGet("/borrowers/{id}", async (Guid id, LendingTrackerContext db) =>
    await db.Borrowers.FindAsync(id) is Borrower borrower ? Results.Ok(borrower) : Results.NotFound()).WithTags("Borrowers")
    .RequireAuthorization("authorized_user");



app.MapPost("/borrowers", async ( [FromBody] Borrower borrower,
                                    [FromServices] LendingTrackerContext db,
                                    [FromServices] IMessageMailer mailer,
                                    [FromQuery(Name = "duplicate")] bool duplicate = false) =>
{
    var currentBorrowers = await db.Borrowers.Where<Borrower>(b =>
      b.UserId == borrower.UserId && b.BorrowerEmail == borrower.BorrowerEmail).ToListAsync<Borrower>();

    if (currentBorrowers.Count() > 0 && !duplicate)
    {
        return Results.Conflict($"{borrower.BorrowerEmail} is used already used by another borrower");
    }

    User lender = await db.Users.FindAsync(borrower.UserId);
    if(lender != null)
    {
        await mailer.SendBorrowerAddedNotification(lender, borrower);
    }

   

    db.Borrowers.Add(borrower);
    await db.SaveChangesAsync();

    return Results.Created($"/borrowers/{borrower.BorrowerId}", borrower);



}).WithTags("Borrowers").RequireAuthorization("authorized_user");

app.MapPut("/borrowers/{id}", async (Guid id, Borrower updatedBorrower, LendingTrackerContext db, ClaimsPrincipal currentUser) =>
{
    Guid sub = Guid.Parse(currentUser.GetNameIdentifierId());
    if (sub != updatedBorrower.UserId) 
        return Results.NotFound();

    var borrower = await db.Borrowers.FindAsync(id);
    if (borrower is null) return Results.NotFound();

    borrower.IsEligible = updatedBorrower.IsEligible;
    borrower.BorrowerEmail = updatedBorrower.BorrowerEmail;
    borrower.BorrowerSms = updatedBorrower.BorrowerSms;
    borrower.Name = updatedBorrower.Name;
    borrower.CountryCode = updatedBorrower.CountryCode;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Borrowers").RequireAuthorization("authorized_user");

app.MapDelete("/borrowers/{id}", async (Guid id, LendingTrackerContext db, ClaimsPrincipal currentUser) =>
{
    Guid sub = Guid.Parse(currentUser.GetNameIdentifierId());

    if (await db.Borrowers.FindAsync(id) is Borrower borrower)
    {
        if (borrower.UserId != sub)
        {
            return Results.Unauthorized();
        }
        db.Borrowers.Remove(borrower);
        await db.SaveChangesAsync();
        return Results.Ok(borrower);
    }
    return Results.NotFound();
}).WithTags("Borrowers").RequireAuthorization("authorized_user");

// For Items****************************************************************************************ITEMS********************************************
app.MapGet("/items", async (LendingTrackerContext db, IHttpContextAccessor httpContext, ClaimsPrincipal currentUser) =>
{
    string sub = currentUser.GetNameIdentifierId();

    if (sub is null) return Results.Unauthorized();

    if (!Guid.TryParse(sub, out var parsedGuid))
        return Results.Unauthorized();



    var items = db.Items
            .Include(trans => trans.Transaction)
            .ThenInclude(trans => trans.Borrower)
            .Include(trans => trans.Transaction)
            .ThenInclude(trans => trans.Messages)
            
            
        .Where(i => i.OwnerId == parsedGuid);

    return items is null ? Results.NotFound() : Results.Ok(items.ToList());

}).WithTags("Items").RequireAuthorization("authorized_user");

app.MapGet("/items/{id}", async (int id, LendingTrackerContext db) =>
    await db.Items.FindAsync(id) is Item item ? Results.Ok(item) : Results.NotFound()).WithTags("Items")
    .RequireAuthorization("authorized_user");

app.MapPost("/items", async (Item item, LendingTrackerContext db) =>
{
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.ItemId}", item);
}).WithTags("Items").RequireAuthorization("authorized_user");

app.MapPut("/items/{id}", async (int id, Item updatedItem, LendingTrackerContext db, ClaimsPrincipal currentUser) =>
{
    var claims = currentUser.Claims.Where(c => c.Type == "extention_userRole");
    Guid sub = Guid.Parse(currentUser.GetNameIdentifierId());

    if(updatedItem.OwnerId != sub && claims.Count() == 0)
    {
        return Results.Unauthorized();
    }

    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.ItemName = updatedItem.ItemName;
    item.Description = updatedItem.Description;
    item.IsAvailable = updatedItem.IsAvailable;
    item.StoreLink = updatedItem.StoreLink;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Items").RequireAuthorization("authorized_user");

app.MapDelete("/items/{id}", async (int id, LendingTrackerContext db, ClaimsPrincipal user) =>
{
    string sub = user.GetNameIdentifierId();

    
    if (await db.Items.FindAsync(id) is Item item)
    {
        if(item.OwnerId.ToString() != sub)
        {
            return Results.Unauthorized();
        }
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
    return Results.NotFound();
}).WithTags("Items").RequireAuthorization("authorized_user");

// For Transactions*****************************************************************************************TRANSACTION**********************************************************
app.MapGet("/transactions", async (LendingTrackerContext db) =>
    await db.Transactions.ToListAsync()).WithTags("Transactions").RequireAuthorization("authorized_user");

app.MapGet("/transactions/{id}", async (int id, LendingTrackerContext db) =>
    await db.Transactions.FindAsync(id) is Transaction transaction ? Results.Ok(transaction) : Results.NotFound())
    .WithTags("Transactions").RequireAuthorization("authorized_user");

app.MapPost("/transactions", async (Transaction transaction, LendingTrackerContext db,  ClaimsPrincipal user, IMessageMailer mailer, IConfiguration config) =>
{
    string sub = user.GetNameIdentifierId();

    if (Guid.Parse(sub) != transaction.LenderId)
        return Results.Unauthorized();
    //notify the user the item was checked out
    Borrower b = await db.Borrowers.FindAsync(transaction.BorrowerId);
    Item i = await db.Items.FindAsync(transaction.ItemId);
    User lender = await db.Users.FindAsync (transaction.LenderId);

    Message msg = new Message()
    {
        Id = new Guid(),
        Method = "sms",
        Text = $"Item {i.ItemName} was checked out to you",
        MessageDate = DateTime.Now,
        Phone = b.BorrowerSms,
        ItemId = i.ItemId
    };
    //await SendMessageToPorrower(transaction, msg.Text, db, smg);

    transaction.Messages.Add(msg);
    
    db.Transactions.Add(transaction);
    await db.SaveChangesAsync();

    await SendMessageToPorrowerWithLink(transaction, $"You have {i.ItemName} checked out from {lender.FullName}", i.StoreLink, lender.FullName, i.ItemName, db, mailer,config);
    return Results.Created($"/transactions/{transaction.TransactionId}", transaction);

}).WithTags("Transactions").RequireAuthorization("authorized_user");

app.MapPut("/transactions/{id}", async (int id, Transaction updatedTransaction, LendingTrackerContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);
    if (transaction is null) return Results.NotFound();

    transaction.BorrowedAt = updatedTransaction.BorrowedAt;
    transaction.ReturnedAt = updatedTransaction.ReturnedAt;
    transaction.DueDate = updatedTransaction.DueDate;
    transaction.Status = updatedTransaction.Status;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Transactions").RequireAuthorization("authorized_user");

app.MapDelete("/transactions/{id}", async (Guid id, LendingTrackerContext db, ClaimsPrincipal user, IMessageMailer mailer) =>
{


    if (await db.Transactions.FindAsync(id) is Transaction transaction)
    {
        var item = await db.Items.FindAsync(transaction.ItemId);
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync();
        if(item != null)
        {
            await SendMessageToPorrower(transaction: transaction, message: $"you checked in {item.ItemName}", db, mailer);
        }
       
        return Results.Ok(transaction);

   
    }
    return Results.NotFound();
}).WithTags("Transactions").RequireAuthorization("authorized_user");

//messaging********************************************************************************************************MESSAGES*******************************************************
app.MapPost("/messages", async ([FromBody] Transaction transaction, string message, string phone, string method, string direction, 
    LendingTrackerContext db, ClaimsPrincipal currentUser, IHubContext<MessagesHub> hub, IMessageMailer mailer) =>
{
    Guid sub = Guid.Parse(currentUser.GetNameIdentifierId());

    if (transaction.LenderId != sub)
    {
        Console.WriteLine($"Borrower {transaction} is not associated to user");
        return Results.Unauthorized();
    }
    Message msg = new Message()
    {
        Id = Guid.NewGuid(),
        Method = method,
        Text = message,
        Phone = phone,
        TransactionId = transaction.TransactionId

    };

    //look up borrower to pull SMS device
    await SendMessageToPorrower(transaction, message, db, mailer);

    db.Messages.Add(msg);
    await db.SaveChangesAsync();

    await hub.Clients.All.SendAsync("ReceiveMessage", "user", "signlr");

    return Results.Created($"/message/{msg.Id}", message);



}).WithTags("Messages").RequireAuthorization("authorized_user");

app.MapGet("/messages/{transactionId}", async ( Guid transactionId, LendingTrackerContext db, ClaimsPrincipal currentUser) =>
{
    Guid sub = Guid.Parse(currentUser.GetNameIdentifierId());

    //if (lenderId != sub)
    //{
    //    Console.WriteLine($"Transaction {transactionId} does not belong to user");
    //    return Results.Unauthorized();

    //}

    var messages = db.Messages.Where(m => m.TransactionId == transactionId);


    return messages is null ? Results.NotFound() : Results.Ok(messages.OrderByDescending(m => m.MessageDate).ToList());

}).WithTags("Messages").RequireAuthorization("authorized_user");

//ADMIN Endpoints ****************************************************************************************Admin Iteams********************************************************
app.MapGet("/admin/items", async (LendingTrackerContext db, IHttpContextAccessor httpContext, ClaimsPrincipal currentUser) =>
{
    string sub = currentUser.GetNameIdentifierId();

    if (sub is null) return Results.Unauthorized();

    if (!Guid.TryParse(sub, out var parsedGuid))
        return Results.Unauthorized();

    var items = db.Items;

    return items is null ? Results.NotFound() : Results.Ok(items.ToList());

}).WithTags("Admin").RequireAuthorization("admin");

app.MapPut("/admin/items/{id}", async (int id, Item updatedItem, LendingTrackerContext db, ClaimsPrincipal currentUser) =>
{

    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.ItemName = updatedItem.ItemName;
    item.Description = updatedItem.Description;
    item.IsAvailable = updatedItem.IsAvailable;
    item.StoreLink = updatedItem.StoreLink;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Admin").RequireAuthorization("admin");

// Run the application
app.Run();



//hELPER FUNCTIONS****************************************************************************************HELPER FUNCTIONS****************************************************
static async Task SendMessageToPorrower(Transaction transaction, string message, LendingTrackerContext db, IMessageMailer emailer)
{
    Borrower b = await db.Borrowers.FindAsync(transaction.BorrowerId);
    if (b != null)
    {
        if (b.BorrowerId != null)
        {
            EmailSendOperation sendResult = await emailer.SendEmail(toAddress: b.BorrowerEmail, subject: "Message From Lending Tracker", message: message);
        }
    }
}

static async Task SendMessageToPorrowerWithLink(Transaction transaction, string message, string link, string lenderName,string itemName, LendingTrackerContext db, IMessageMailer emailer, IConfiguration config)
{
    Borrower b = await db.Borrowers.FindAsync(transaction.BorrowerId);
    string logourl = config["Branding:Logo"];
    var emailContent = new EmailContent($"Message From {config["Branding:Name"]} Lending Tracker");
    emailContent.Html = "<html>";
    emailContent.Html += $"<img src='{logourl}' alt='Logo'/>";
    emailContent.Html += $"<h3>Lending Tracker activity from {config["Branding:Name"]} </h3>";
    emailContent.Html += "<br/>";
    emailContent.Html += $"You borrowed a {itemName} from {lenderName} an <strong> {config["Branding:Name"]} </strong> lending tracker user.";
    emailContent.Html += "<br/>";
    emailContent.Html += $"<p>If your looking to purchase a {itemName} you can find the item <a href='{link}'>Here</a> at {config["Branding:Name"]}'s online store.</p>";
    emailContent.Html += $"If you would like to track items you lend out using Lending Tracker signup <a href='{config["LendingTracker:Link"]}' alt='Lending Tracker Signup Link'>Here</a>";
    emailContent.Html += "</html>";
    

    var emailMessage = new EmailMessage(config["MessageMailer:SenderAddress"], b.BorrowerEmail, emailContent);
   
    if (b != null)
    {
        if (b.BorrowerId != null)
        {
            EmailSendOperation sendResult = await emailer.SendEmail(emailMessage);
        }
    }
}