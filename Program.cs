using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure database context - use SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection string from configuration: {connectionString}");

// Explicitly use SQLite provider
builder.Services.AddDbContext<TodoContext>(options => 
{
    options.UseSqlite(connectionString ?? "Data Source=TodoList.db");
});

// Add AWS CloudWatch logging in production
if (builder.Environment.IsProduction())
{
    builder.Logging.AddAWSProvider();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

// Create the database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TodoContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Log the database path
        logger.LogInformation("Database path: {DbPath}", connectionString);
        
        // Ensure database is created
        context.Database.EnsureCreated();
        logger.LogInformation("Database created successfully");
        
        // Seed the database with initial data if empty
        if (!context.Todos.Any())
        {
            logger.LogInformation("Seeding database with initial data");
            SeedData.Initialize(context);
            logger.LogInformation("Database seeded successfully");
        }
        else
        {
            logger.LogInformation("Database already contains data, skipping seed");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating/seeding the database.");
    }
}

app.Run();
