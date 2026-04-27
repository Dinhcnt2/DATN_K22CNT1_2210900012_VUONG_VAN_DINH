using Microsoft.EntityFrameworkCore;
using VVD_2210900012_DATN.Models;

namespace VVD_2210900012_DATN
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===== ADD SERVICES =====
            builder.Services.AddControllersWithViews();

            // 🔥 SESSION CHUẨN
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // ===== DB =====
            var connectionString = builder.Configuration.GetConnectionString("BookConnection");

            builder.Services.AddDbContext<BookstoreContext>(options =>
                options.UseSqlServer(connectionString));

            var app = builder.Build();

            // ===== PIPELINE =====
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 🔥 SESSION PHẢI ĐẶT SAU ROUTING
            app.UseSession();

            app.UseAuthorization();

            // ===== ROUTE AREA =====
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            // ===== ROUTE DEFAULT =====
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
