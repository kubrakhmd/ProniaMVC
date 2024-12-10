using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModel;
using Pronia.Areas.ViewModels;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly ProniaDBContext _context;

        public SizeController(ProniaDBContext context)
        {
            _context=context;   
            
        }
        public async Task<IActionResult> Index()
        {
            var sizeVMs = await _context.Sizes.Where(c => !c.IsDeleted).Include(s=> s.ProductSizes).Select(s => new ListSizeVM
            {
                Id = s.Id,
                Name = s.Name,
               
            }).ToListAsync();
            return View(sizeVMs);

        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateSizeVM sizeVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Tags.AnyAsync(t => t.Name.Trim() == sizeVM.Name.Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "Name Already exists");
                return View();

            }
         Size size = new()
            {
                Name = sizeVM.Name,
            };
            size.CreatedAt = DateTime.Now;
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);

            if (size is null) return NotFound();

            return View(size);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSizeVM sizeVM)
        {
            if (id == null || id < 1) return BadRequest();

            Size existed = await _context.Sizes.FirstOrDefaultAsync(s=> s.Id == id);
            if (existed is null) return NotFound();

            bool result = await _context.Sizes.AnyAsync(s => s.Name == sizeVM.Name && s.Id != id);

            if (result)
            {
                ModelState.AddModelError(nameof(Size.Name), "Size already exists");
                return View();
            }

            existed.Name = sizeVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || id < 1) return BadRequest();

            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size is null) return NotFound();


           size.IsDeleted = true;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        } 
    }
}
