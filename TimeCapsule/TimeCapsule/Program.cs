using Microsoft.EntityFrameworkCore;
using TimeCapsule;
using TimeCapsule.Models;
using Microsoft.AspNetCore.Identity;
using TimeCapsule.Services;
using TimeCapsule.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<TimeCapsuleContext>().AddDefaultTokenProviders().AddDefaultUI();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddOptions<PortalSettings>()
    .BindConfiguration("PortalSettings")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<CapsuleService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IFormManagementService, FormManagementService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();

var connectionString = builder.Configuration.GetConnectionString("Database") ?? throw new ArgumentNullException("ConnectionString");

builder.Services.AddDbContext<TimeCapsuleContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableDetailedErrors();
});

//Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    if (app.Environment.IsDevelopment())
    {
        endpoints.MapGet("/debug-config", ctx =>
        {
            var config = app.Services.GetRequiredService<IConfiguration>();
            return ctx.Response.WriteAsync((config as IConfigurationRoot).GetDebugView());
        });
    }
});

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<TimeCapsuleContext>();
    ctx.Database.Migrate();
}
app.Run();


