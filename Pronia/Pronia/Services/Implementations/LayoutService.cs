
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

        public LayoutService(ProniaDBContext context,IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
           
            {
                List<BasketCookieItemVM> cookiesVM;
                string cookie = _http.HttpContext.Request.Cookies["basket"];

                List<BasketItemVM> basketVM = new();

                if (cookie != null) return basketVM;

                cookiesVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);

                foreach (var item in cookiesVM)
                {

                    Product product = await _context.Products.Include(p => p.ProductImages.Where(p => p.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);

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


                return basketVM;
            }
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }
    }
}