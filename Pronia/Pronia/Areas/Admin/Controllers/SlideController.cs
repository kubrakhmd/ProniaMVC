using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Extensions;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SlideController : Controller
    {
        private readonly ProniaDBContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(ProniaDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();

            return View(slides);
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Test()
        {

            return Content(Guid.NewGuid().ToString());
        }


        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if (!ModelState.IsValid) return View();


            if (!slide.Photo.ValidateType("image/"))
            {

                ModelState.AddModelError("Photo", "File type is not correct");
                return View();
            }

            if (!slide.Photo.ValidateSize(FileSize.MB, 2))
            {
                ModelState.AddModelError("Photo", "File size must be less than 2mb");
                return View();
            }

            string fileName = await slide.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();

            slide.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}