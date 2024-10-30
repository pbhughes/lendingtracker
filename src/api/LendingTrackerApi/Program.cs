using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using LendingTrackerApi.Models;
using Microsoft.OpenApi.Models;
using Redoc;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

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

//add authorization
var auth0Domain = builder.Configuration["Auth0:Domain"];
var auth0Audience = builder.Configuration["Auth0:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = $"https://${auth0Domain}/";
    options.Audience = auth0Audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authorized_user", policy =>
        policy
            .RequireClaim("scope", "lender"));
});

var app = builder.Build();

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
app.UseAuthorization();

// Define CRUD endpoints for User
app.MapGet("/users", async (LendingTrackerContext db) =>
    await db.Users.ToListAsync())
    .WithTags("Users").RequireAuthorization("authorized_user");

app.MapGet("/users/{id}", async (int id, LendingTrackerContext db) =>
    await db.Users.FindAsync(id) is User user ? Results.Ok(user) : Results.NotFound())
    .WithTags("Users").RequireAuthorization("authorized_user");

app.MapPost("/users", async (User user, LendingTrackerContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.UserId}", user);
  
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
app.MapGet("/items", async (LendingTrackerContext db) =>
    await db.Items.ToListAsync()).WithTags("Items").RequireAuthorization("authorized_user");

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
