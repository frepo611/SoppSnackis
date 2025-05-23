
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Data;
using SoppSnackis.Areas.Identity.Data;

namespace SoppSnackis;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        // Register your DbContext with the DI container
        builder.Services.AddDbContext<ForumDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection")));

        builder.Services.AddDbContext<SoppSnackisIdentityDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection")));

        builder.Services.AddDefaultIdentity<SoppSnackisUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<SoppSnackisIdentityDbContext>();

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

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.MapRazorPages();

        app.Run();
    }
}
