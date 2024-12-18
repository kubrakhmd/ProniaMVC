using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Services.Implementations;
using Pronia.Services.InterFaces;
using Pronia.Services.Implementations.ProniaMVC.Services.Implementations;
using Pronia.MiddleWares;

namespace Pronia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ProniaDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;

              
                opt.User.RequireUniqueEmail = true;

                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            }).AddEntityFrameworkStores<ProniaDBContext>().AddDefaultTokenProviders();
            builder.Services.AddScoped<ILayoutService, LayoutService>();
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
           // app.UseMiddleware<GlobalExceptionHandler>();


            app.MapControllerRoute(
           "admin",
           "{area:exists}/{controller=home}/{action=index}/{id?}"
            );
            app.MapControllerRoute(
                "default",

                "{controller=home}/{action=index}/{id?}");
            app.Run();
        }
    }
}
