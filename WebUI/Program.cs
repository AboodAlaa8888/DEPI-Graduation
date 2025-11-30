using Core.Models;
using Core.RepositoryInterfaces;
using Infrastructure.Data.DbContexts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WebUI.DbSeeder;
using Stripe; // Add this namespace


public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<NursingServicesDbContext>(options =>
        options.UseSqlServer(
         builder.Configuration.GetConnectionString("DefaultConnection"),
         sqlOptions =>
         {
             sqlOptions.EnableRetryOnFailure();
         }
         ));

        builder.Services.AddScoped<INurseRepository, NurseRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IPatientRepository, PatientRepository>();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<NursingServicesDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        });

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        var app = builder.Build();

        // Configure Stripe with the Secret Key from appsettings
        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapDefaultControllerRoute();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await SeedData.InitializeAsync(services);
        }

        app.Run();
    }
}

