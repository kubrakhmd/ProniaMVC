
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        public readonly ProniaDBContext _context;

        public HomeController(ProniaDBContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Slides = await _context.Slides
                .OrderBy(s => s.Order)
                .Take(2)
                .ToListAsync(),


                NewProducts = await _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null))
                .ToListAsync(),

            };
            return View(homeVM);

        }
         public IActionResult Error(string errormessage)
        {
            return View(model: errormessage);
        }
    }
}