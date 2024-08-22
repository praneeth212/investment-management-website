using Managament.Data;
using Managament.Services;
using Management.Repositories;
using Management.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MVCDemoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MVCDemoConnectionString")));
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IInvestmentRepository, InvestmentRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<SipReminderScheduler>();
builder.Services.AddHostedService<ScheduledTasks>();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Ensure the database is created and seeded.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MVCDemoDbContext>();
    DbInitializer.Initialize(dbContext); // Updated call without logger
}

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
app.UseAuthentication();
app.UseAuthorization();

// Configure the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure the default route
app.MapControllerRoute(
    name: "Login",
    pattern: "{controller=Account}/{action=Login}/{id?}");
// Configure the route for employee registration
app.MapControllerRoute(
    name: "register",
    pattern: "{controller=Employee}/{action=Add}/{id?}");

// Configure the route for updating employee information
app.MapControllerRoute(
    name: "update",
    pattern: "{controller=Employee}/{action=Profile}/{id?}");

// Configure the route for deleting an employee
app.MapControllerRoute(
    name: "delete",
    pattern: "{controller=Employee}/{action=Delete}/{id?}");

// Configure the route for creating support tickets
app.MapControllerRoute(
    name: "Support",
    pattern: "{controller=Support}/{action=Create}/{id?}");

// Configure the route for user logout
app.MapControllerRoute(
    name: "Logout",
    pattern: "{controller=Account}/{action=Logout}/{id?}");
// Configure the route for forgot password
app.MapControllerRoute(
    name: "ForgetPassword",
    pattern: "{controller=Employee}/{action=ForgotPassword}/{id?}");

// Configure the route for reset password
app.MapControllerRoute(
    name: "ResetPassword",
    pattern: "{controller=Employee}/{action=ResetPassword}/{token?}");

app.Run();
