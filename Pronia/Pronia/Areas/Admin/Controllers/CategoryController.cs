﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ViewModel;
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
            var categorieVMs = await _context.Categories.Where(c => !c.IsDeleted).Include(c => c.Products).Select(c => new GetCategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count
            }).ToListAsync();
            return View(categorieVMs);
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
        // Salam      Salam
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (id == null || id < 1) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == category.Name.Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(Category.Name), "Category already exists");
                return View();
            }

            //if (existed.Name == category.Name) return RedirectToAction(nameof(Index));

            existed.Name = category.Name;

            //_context.Categories.Update(existed);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();


            category.IsDeleted = true;
            //_context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}