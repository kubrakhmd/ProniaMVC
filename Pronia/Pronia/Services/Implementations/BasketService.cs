namespace Pronia.Services.Implementations
{
  
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
   
  
 
    using System.Security.Claims;
    using static System.Net.WebRequestMethods;
    using Pronia.Models;
    using Pronia.ViewModels;
    
    using Pronia.DAL;
    using Pronia.Services.InterFaces;

    namespace ProniaMVC.Services.Implementations
    {
        public class BasketService : IBasketService
        {
            private readonly ProniaDBContext _context;
            private readonly IHttpContextAccessor _http;
            private readonly UserManager<AppUser> _usermanager;
            public readonly ClaimsPrincipal _user;
            public BasketService(ProniaDBContext context, IHttpContextAccessor http, UserManager<AppUser> usermanager)
            {
                _context = context;
                _http = http;
                _usermanager = usermanager;
                _user = http.HttpContext.User;
            }



            public async Task<List<BasketItemVM>> GetBasketAsync()
            {
                List<BasketItemVM> basketVM = new();

                if (_user.Identity.IsAuthenticated)
                {
                    basketVM= await _context.BasketItems.Where(b => b.AppUserId == _user.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(b => new BasketItemVM
                    {
                        Id = b.ProductId,
                        Name = b.Product.Name,
                        Image = b.Product.ProductImages.FirstOrDefault(i => i.IsPrimary == true).Image,
                        Price = b.Product.Price,
                        Count = b.Count,
                        SubTotal = b.Count * b.Product.Price
                    }

                    ).ToListAsync();
                }
                else
                {
                    List<BasketCookieItemVM> cookieVM;
                    string cookie = _http.HttpContext.Request.Cookies["basket"];


                    if (cookie is null)
                    {
                        return basketVM;
                    }

                    cookieVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);

                    foreach (var item in cookieVM)
                    {
                        Product? product = await _context.Products
                        .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                        .FirstOrDefaultAsync(p => p.Id == item.Id);

                        if (product is not null)
                        {
                            basketVM.Add(new BasketItemVM
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Price = product.Price,
                                Image = product.ProductImages[0].Image,
                                Count = item.Count,
                                SubTotal = item.Count * product.Price

                            });
                        }

                    }
                }
                return basketVM;
            }




        }
    }
}
