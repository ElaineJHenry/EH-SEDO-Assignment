using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EH_SEDO_Assignment.Data;
using EH_SEDO_Assignment.Services;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("EH_SEDO_AssignmentContext") ?? throw new InvalidOperationException("Connection string 'EH_SEDO_AssignmentContext' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

//get connection string
var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{
    //if run in thew development environment, get the connection string from user secrets
    ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
    IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();
    connection = configuration.GetSection("ConnectionStrings")["DevConnection"];
}
else
{
    connection = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DbConnectionString");
}

//set databse context
builder.Services.AddDbContext<EH_SEDO_AssignmentContext>(options => options.UseAzureSql(connection));

//identity service configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    //user settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+'";
    options.User.RequireUniqueEmail = true;

    //password settings
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;

    //failed login lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<EH_SEDO_AssignmentContext>()
    .AddDefaultTokenProviders();

//application cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    //general settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;

    //routing settings
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

//add database repository service
builder.Services.AddScoped<IDatabaseRepository, DatabaseRepository>();

var app = builder.Build();
await SeedService.SeedDatabase(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
