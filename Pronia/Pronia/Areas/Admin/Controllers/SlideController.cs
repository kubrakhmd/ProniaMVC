using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SlideController : Controller
    {
        private readonly ProniaDBContext _context;

        public SlideController(ProniaDBContext context)
        {
            _context = context;
        }
        public async Task <IActionResult> Index()
        {
            List<Slide>slides=await _context.Slides.ToListAsync();

            return View(slides);
        }
    }
}
