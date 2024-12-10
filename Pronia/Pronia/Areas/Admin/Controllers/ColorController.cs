using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModel;
using Pronia.Areas.ViewModels;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly ProniaDBContext _context;

        public ColorController(ProniaDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var colorVMs = await _context.Colors.Where(c => !c.IsDeleted).Include(c => c.ProductColors).Select(c => new ListColorVM
            {
                Id = c.Id,
                Name = c.Name,
               
            }).ToListAsync();
            return View(colorVMs);

        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }

            bool result = await _context.Colors.AnyAsync(t => t.Name.Trim() == colorVM.Name.Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "Name Already exists");
                return View();

            }
           Color color = new()
            {
                Name = colorVM.Name,
            };
            color.CreatedAt = DateTime.Now;
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

           Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color is null) return NotFound();

            return View(color);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateColorVM colorVM)
        {
            if (id == null || id < 1) return BadRequest();

            Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool result = await _context.Colors.AnyAsync(c => c.Name == colorVM.Name && c.Id != id);

            if (result)
            {
                ModelState.AddModelError(nameof(Color.Name), "Color already exists");
                return View();
            }

            existed.Name = colorVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || id < 1) return BadRequest();

           Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color is null) return NotFound();


            color.IsDeleted = true;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


    }
}
