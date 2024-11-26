using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class HomeController:Controller
    {
        public readonly ProniaDBContext _context;

        public HomeController(ProniaDBContext context)
        {
            _context =context;
            
        }
        public async Task<IActionResult> Index()
        {



            HomeVM homeVM = new HomeVM
            {
                Slides = await _context.Slides.OrderBy(s => s.Order).Take(2).ToListAsync(),
                Products = await _context.Products
            .Take(8)
            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null))
            .ToListAsync()
            };


            return View(homeVM);
        }
    }
}
