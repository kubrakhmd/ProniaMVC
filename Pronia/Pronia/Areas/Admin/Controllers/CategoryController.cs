using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModels;
using Pronia.DAL;
using Pronia.Models; 

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ProniaDBContext _context;

        public CategoryController(ProniaDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.Include(c => c.Products).ToListAsync();

            return View(categories);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == categoryVM.Name.Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "Name Already exists");
                return View();

            }
            Category category = new()
            {
                Name = categoryVM.Name
            };
            category.CreatedAt = DateTime.Now;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


           
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateCategoryVM categoryVM)
        {
            if (id == null || id < 1) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool result = await _context.Categories.AnyAsync(c => c.Name == categoryVM.Name && c.Id != id);

            if (result)
            {
                ModelState.AddModelError(nameof(Category.Name),"Category already exists");
                return View();
            }

            existed.Name = categoryVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

            //_context.Categories.Update(existed);
        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();


            category.IsDeleted = true;
            
            await _context.SaveChangesAsync();
            

            return RedirectToAction(nameof(Index));
        }
    }
}