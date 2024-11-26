using Microsoft.EntityFrameworkCore;
using Pronia.DAL;

namespace Pronia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ProniaDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            var app = builder.Build();
            app.UseStaticFiles();
            app.UseRouting();

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
