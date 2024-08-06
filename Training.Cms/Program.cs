using Microsoft.AspNetCore.Authentication.Cookies;
using Training.Api.Configurations;
using Training.BusinessLogic.Services.Admin;
using Training.Common.Constants;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCoreDependencies(config);

builder.Services.AddAutoMapper(typeof(Program).Assembly);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;

    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireClaim("RolePolicy", RolePolicies.SysAdmin.Name));

    options.AddPolicy("AdminClerk", policy =>
          policy.RequireAssertion(context =>
              context.User.HasClaim("RolePolicy", RolePolicies.SysAdmin.Name) ||
              context.User.HasClaim("RolePolicy", RolePolicies.Clerk.Name)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Create default admin account
using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    await userService.CreateDefaultAdminAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.RunMigration();
app.Run();
