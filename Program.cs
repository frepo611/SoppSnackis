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
            .AddEntityFrameworkStores<SoppSnackisIdentityDbContext>();

        var app = builder.Build();

        // Seed the Identity database with a default user
        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SoppSnackisUser>>();
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
                // Optional: Check result.Succeeded if you need to take further action on failure.
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
