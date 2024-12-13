
using System;
using System.Security.Claims;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Services.InterFaces;
using Pronia.ViewModels;

namespace Pronia.Services.Implementations
{
    public class LayoutService : ILayoutService
    {
        private readonly ProniaDBContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly ClaimsPrincipal _user;

        public LayoutService(ProniaDBContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
            _user = _http.HttpContext.User;
        }

        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
            List<BasketItemVM> basketVM = new();
            if (_user.Identity.IsAuthenticated)
            {
               
                basketVM = await _context.BasketItems
                         .Where(bi => bi.AppUserId == _user.FindFirstValue(ClaimTypes.NameIdentifier))
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
                string cookie = _http.HttpContext.Request.Cookies["basket"];


                if (cookie is null)
                {
                    return basketVM;
                }

                cookiesVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);
                foreach (BasketCookieItemVM item in cookiesVM)
                {
                    Product product = await _context.Products.Include(p => p.ProductImages.Where(p => p.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (product is not null)
                    {
                        basketVM.Add(new BasketItemVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Image = product.ProductImages[0].Image,
                            Price = product.Price,
                            Count = item.Count,
                            SubTotal = item.Count * product.Price

                        });
                    }
                }

            }


            return basketVM;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }
    }
}