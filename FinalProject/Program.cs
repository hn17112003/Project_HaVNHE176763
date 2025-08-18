using FinalProject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddScoped<SalesReportingService>();

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ElectronicsStoreContext>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("Authentication:Google:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("Authentication:Google:ClientSecret").Value;
    options.CallbackPath = "/signin-google";

    options.Events.OnCreatingTicket = async context =>
    {
        var email = context.Principal.FindFirst(ClaimTypes.Email)?.Value;

        if (email != null)
        {
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ElectronicsStoreContext>();

            var user = dbContext.Users.SingleOrDefault(u => u.Email == email);

            if (user != null)
            {
                var session = context.HttpContext.Session;
                session.SetString("Username", user.Username);
                session.SetInt32("UserId", user.UserId);
            }
            else
            {
                context.Response.Redirect("/register?email=" + Uri.EscapeDataString(email));
            }
        }
    };
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
