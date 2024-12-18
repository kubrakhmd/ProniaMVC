using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Pronia.DAL;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Exceptions;
using Pronia.ViewModels;
using GetCategoryVM = Pronia.Areas.ViewModel.GetCategoryVM;
namespace Pronia.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProniaDBContext _context;

        public ShopController(ProniaDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? search, int? categoryId, int key = 1, int page = 1)
        {
            IQueryable<Product> query = _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null));
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }
            if (categoryId != null && categoryId > 0)
            {
                query = query.Where(pi => pi.CategoryId == categoryId);
            };
            switch (key)
            {
                case (int)SortType.Name:
                    query = query.OrderBy(p => p.Name);
                    break;

                case (int)SortType.Price:
                    query = query.OrderByDescending(p => p.Price);
                    break;

                case (int)SortType.Date:
                    query = query.OrderBy(p => p.CreatedAt);
                    break;
            }
            int count = query.Count();
            double totalPage = Math.Ceiling((double)count / 3);

            query = query.Skip((page - 1) * 3).Take(3);


            ShopVM shopVM = new ShopVM
            {
                Products = await query.Select(p => new GetProductVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                    HoverImage = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false).Image,
                    Price = p.Price,
                }).ToListAsync(),

                Categories = await _context.Categories.Select(c => new CategoryGetVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Count = c.Products.Count
                }).ToListAsync(),

                Search = search,
                CategoryId = categoryId,
                Key = key,
                TotalPage = totalPage,
                CurrentPage = page
            };



            return View(shopVM);
        }

        public async Task<IActionResult> Detail(int? id)
            {
                if (id == null || id <= 0) return BadRequest();
                Product? product = await _context.Products.Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                    .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                    .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
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
