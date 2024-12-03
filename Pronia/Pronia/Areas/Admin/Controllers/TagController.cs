using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModels;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly ProniaDBContext _context;

        public TagController(ProniaDBContext context)
        {
            _context=context;   
        }
        public async Task <IActionResult>Index()
        {
            List<Tag> tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();

            return View(tags);
            
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }

            bool result = await _context.Tags.AnyAsync(t => t.Name.Trim() == tagVM.Name.Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "Name Already exists");
                return View();

            }
            Tag tag = new()
            {
                Name = tagVM.Name,
            };
            tag.CreatedAt = DateTime.Now;
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }
       
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tag is null) return NotFound();

            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateTagVM tagVM)
        {
            if (id == null || id < 1) return BadRequest();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (existed is null) return NotFound();

            bool result = await _context.Tags.AnyAsync(t => t.Name == tagVM.Name && t.Id != id);

            if (result)
            {
                ModelState.AddModelError(nameof(Tag.Name), "Tag already exists");
                return View();
            }

            existed.Name = tagVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

            
        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || id < 1) return BadRequest();

         Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tag is null) return NotFound();


            tag.IsDeleted = true;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


    }
}
