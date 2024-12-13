﻿using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class BasketController : Controller
    {
        private ProniaDBContext _context;
        private readonly UserManager<AppUser> _userManeger;

        public BasketController(ProniaDBContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManeger= userManager;  
            
        }
        public async Task<IActionResult> Index()
        {

            List<BasketItemVM> basketVM = new();
            if (User.Identity.IsAuthenticated)
            {
                // AppUser user=await _userManeger.FindByNameAsync(User.Identity.Name);
                //basketVM = await _userManeger.Users
                //         .Include(u => u.BasketItems)
                //         .Where(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
                //         .SelectMany(u => u.BasketItems).Select(bi => new BasketItemVM
                //         {

                //             Count = bi.Count,
                //             Image = bi.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                //             Name = bi.Product.Name,
                //             Price = bi.Product.Price,
                //             SubTotal = bi.Count * bi.Product.Price

                // }).ToListAsync();
            basketVM = await _context.BasketItems
                     .Where(bi=>bi.AppUserId ==User.FindFirstValue(ClaimTypes.NameIdentifier))
                  .Select(bi => new BasketItemVM
                     {

                         Count = bi.Count,
                         Image = bi.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                         Name = bi.Product.Name,
                         Price = bi.Product.Price,
                         SubTotal = bi.Count * bi.Product.Price


                     }).ToListAsync();


        }
            else
            {

                List<BasketCookieItemVM> cookiesVM;
                string cookie = Request.Cookies["basket"];


                if (cookie != null) return View();

                cookiesVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);

                foreach (var item in cookiesVM)
                {

                    Product product = await _context.Products
                        .Include(p => p.ProductImages.Where(p => p.IsPrimary == true))
                        .FirstOrDefaultAsync(p => p.Id == item.Id);

                    if (product != null)
                    {
                        basketVM.Add(new BasketItemVM
                        {

                            Id = product.Id,
                            Name = product.Name,
                            Image = product.ProductImages[0].Image,
                            Price = product.Price,
                            Count = item.Count,
                            SubTotal = (decimal)(item.Count * product.Price),

                        });
                    }

                }

            }

            return View(basketVM);
        }
        public async Task < IActionResult> AddBasket(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            bool result =await _context.Products.AnyAsync(p=> p.Id == id);
            if (User.Identity.IsAuthenticated)
            {
                // AppUser user=await _userManeger.FindByNameAsync(User.Identity.Name);
                //AppUser? user = await _userManeger.Users
                //    .Include(u => u.BasketItems)
                AppUser? user = await _userManeger.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
               BasketItem item= user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (item != null)
                {

                    user.BasketItems.Add(new BasketItem
                    {
                        ProductId = id.Value,
                        Count =1
                    });
                
                }
                else
                {
                    item.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;

                string cookies = Request.Cookies["basket"];
                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);

                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed != null)
                    {
                        existed.Count++;
                    }
                    else
                    {
                        basket.Add(new BasketCookieItemVM
                        {

                            Id = id.Value,
                            Count = 1

                        });

                    }
                }
                else
                {
                    basket = new();
                    basket.Add(new()
                    {

                        Id = id.Value,
                        Count = 1
                    });


                }
                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("basket", json);

            }


            return RedirectToAction("Index","Home");

        } 
        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["basket"]);
        }
    }
}
