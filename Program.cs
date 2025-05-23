using Microsoft.EntityFrameworkCore;
using SoppSnackis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace SoppSnackis;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        // Register your DbContext with the DI container
        builder.Services.AddDbContext<SoppSnackisIdentityDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection")));

        builder.Services.AddDefaultIdentity<SoppSnackisUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<SoppSnackisIdentityDbContext>();

        var app = builder.Build();

        // Seed the Identity database with a default user and admin role
        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SoppSnackisUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Ensure the "admin" role exists
            var adminRoleName = "admin";
            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(adminRoleName));
            }
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                adminUser = new SoppSnackisUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
            }
            // Add user to "admin" role if not already in it
            if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
            {
                await userManager.AddToRoleAsync(adminUser, adminRoleName);
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        await app.RunAsync();
    }
}
