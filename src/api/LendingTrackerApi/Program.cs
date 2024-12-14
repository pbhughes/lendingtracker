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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();

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
        .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
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

});

//inject model validators
builder.Services.AddScoped(typeof(IValidationServices), typeof(ValidatorService));

var app = builder.Build();

// Use CORS
app.UseCors(MyAllowSpecificOrigins);

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

// Define CRUD endpoints for User
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


app.MapPut("/users/{id}", async (int id, User updatedUser, LendingTrackerContext db) =>
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

// Repeat the above pattern for Borrowers, Items, and Transactions

// For Borrowers

app.MapGet("/borrowers", async (LendingTrackerContext db) =>
    await db.Borrowers.ToListAsync()).WithTags("Borrowers").RequireAuthorization("authorized_user");

app.MapGet("/borrowers/{id}", async (int id, LendingTrackerContext db) =>
    await db.Borrowers.FindAsync(id) is Borrower borrower ? Results.Ok(borrower) : Results.NotFound()).WithTags("Borrowers")
    .RequireAuthorization("authorized_user");

app.MapPost("/borrowers", async (Borrower borrower, LendingTrackerContext db) =>
{
    db.Borrowers.Add(borrower);
    await db.SaveChangesAsync();
    return Results.Created($"/borrowers/{borrower.BorrowerId}", borrower);
}).WithTags("Borrowers").RequireAuthorization("authorized_user");

app.MapPut("/borrowers/{id}", async (int id, Borrower updatedBorrower, LendingTrackerContext db) =>
{
    var borrower = await db.Borrowers.FindAsync(id);
    if (borrower is null) return Results.NotFound();

    borrower.IsEligible = updatedBorrower.IsEligible;
    borrower.BorrowerEmail = updatedBorrower.BorrowerEmail;
    borrower.BorrowerSms = updatedBorrower.BorrowerSms;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Borrowers").RequireAuthorization("authorized_user");

app.MapDelete("/borrowers/{id}", async (int id, LendingTrackerContext db) =>
{
    if (await db.Borrowers.FindAsync(id) is Borrower borrower)
    {
        db.Borrowers.Remove(borrower);
        await db.SaveChangesAsync();
        return Results.Ok(borrower);
    }
    return Results.NotFound();
}).WithTags("Borrowers").RequireAuthorization("authorized_user");

// For Items
app.MapGet("/items", async (LendingTrackerContext db, IHttpContextAccessor httpContext) =>
{
    if (httpContext == null) return Results.Unauthorized();

    var id = httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (id is null) return Results.Unauthorized();

    if (!Guid.TryParse(id, out var parsedGuid))
        return Results.Unauthorized();

    var items = db.Items.Where(i => i.OwnerId == parsedGuid);

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

app.MapPut("/items/{id}", async (int id, Item updatedItem, LendingTrackerContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.ItemName = updatedItem.ItemName;
    item.Description = updatedItem.Description;
    item.IsAvailable = updatedItem.IsAvailable;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Items").RequireAuthorization("authorized_user");

app.MapDelete("/items/{id}", async (int id, LendingTrackerContext db) =>
{
    if (await db.Items.FindAsync(id) is Item item)
    {
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
    return Results.NotFound();
}).WithTags("Items").RequireAuthorization("authorized_user");

// For Transactions
app.MapGet("/transactions", async (LendingTrackerContext db) =>
    await db.Transactions.ToListAsync()).WithTags("Transactions").RequireAuthorization("authorized_user");

app.MapGet("/transactions/{id}", async (int id, LendingTrackerContext db) =>
    await db.Transactions.FindAsync(id) is Transaction transaction ? Results.Ok(transaction) : Results.NotFound())
    .WithTags("Transactions").RequireAuthorization("authorized_user");

app.MapPost("/transactions", async (Transaction transaction, LendingTrackerContext db) =>
{
    db.Transactions.Add(transaction);
    await db.SaveChangesAsync();
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

app.MapDelete("/transactions/{id}", async (int id, LendingTrackerContext db) =>
{
    if (await db.Transactions.FindAsync(id) is Transaction transaction)
    {
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync();
        return Results.Ok(transaction);
    }
    return Results.NotFound();
}).WithTags("Transactions").RequireAuthorization("authorized_user");

// Run the application
app.Run();
