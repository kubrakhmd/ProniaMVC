using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Exceptions;
using Pronia.ViewModels;
namespace Pronia.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProniaDBContext _context;

        public ShopController(ProniaDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || id <= 0) return BadRequest();
            Product product = await _context.Products.Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p=>p.ProductTags) 
                .ThenInclude(pt=>pt.Tag)
                .Include(p=>p.ProductColors)
                .ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductSizes)
                .ThenInclude(ps=>ps.Size)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) throw new NotFoundException($" Not Found!!");
            DetailVM detailVM = new DetailVM
            {
                Product = product,
                RelatedProducts = await _context.Products.Where(p => p.CategoryId == product.CategoryId && p.Id != id)
            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null))
            .Take(8)
           .ToListAsync()
            };

            return View(detailVM);
        }
    }
}
