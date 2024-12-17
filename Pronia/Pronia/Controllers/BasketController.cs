using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Services.Implementations.ProniaMVC.Services.Implementations;
using Pronia.Services.InterFaces;
using Pronia.Utilities.Exceptions;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly ProniaDBContext _context;
        private readonly UserManager<AppUser> _userManeger;
        private readonly IBasketService _basketservice;


        public BasketController(ProniaDBContext context, UserManager<AppUser> userManager, IBasketService basketservice)
        {
            _context = context;
            _userManeger = userManager;
            _basketservice = basketservice;

        }
        public async Task<IActionResult> Index()
        {

         

            return View(await _basketservice.GetBasketAsync());
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null || id < 1) throw new BadRequestException($"Wrong Request!!");

            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) return NotFound();


            if (User.Identity.IsAuthenticated)
            {

                //AppUser user=await _userManager.FindByNameAsync(User.Identity.Name);
                //AppUser? user = await _userManager.Users
                //    .Include(u => u.BasketItems)
                //    .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                AppUser? user = await _userManeger.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


                BasketItem item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (item is null)
                {
                    user.BasketItems.Add(new BasketItem
                    {
                        ProductId = id.Value,
                        Count = 1,
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

            return RedirectToAction(nameof(GetBasket));

        }
        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["basket"]);
        }


        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null || id < 1)
            {
                return BadRequest();
            }

            if (User.Identity.IsAuthenticated)
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManeger.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) throw new NotFoundException($"404 Not Found!!");

                var item = user.BasketItems.FirstOrDefault(b => b.ProductId == id);

                if (item != null)
                {
                    user.BasketItems.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {

                string cookies = Request.Cookies["basket"];
                if (cookies != null)
                {
                    var basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);
                    var item = basket.FirstOrDefault(b => b.Id == id);

                    if (item != null)
                    {
                        basket.Remove(item);

                        string json = JsonConvert.SerializeObject(basket);
                        Response.Cookies.Append("basket", json);
                    }
                }
            }

            return RedirectToAction("Index", "Basket");
        }

       
        [HttpPost]
        public async Task<IActionResult> ChangeItemQuantity(int? id, string change)
        {
            if (id == null || id < 1 || string.IsNullOrEmpty(change)) return BadRequest();

            int changeInt = Convert.ToInt32(change);
            if (changeInt != 1 && changeInt != -1) return BadRequest(); 

            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) throw new NotFoundException($"404 Not Found!!");

            if (User.Identity.IsAuthenticated)
            {
               
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManeger.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) throw new NotFoundException($"404 Not Found!!");


                var item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (item == null) throw new NotFoundException($"404 Not Found!!");


                item.Count += changeInt;
                if (item.Count <= 0)
                {
                    user.BasketItems.Remove(item);  
                }

                await _context.SaveChangesAsync();
            }
            else
            {
               
                string cookies = Request.Cookies["basket"];
                if (cookies != null)
                {
                    var basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);
                    var item = basket.FirstOrDefault(b => b.Id == id);
                    if (item == null) throw new NotFoundException($"404 Not Found!!");


                    item.Count += changeInt;
                    if (item.Count <= 0)
                    {
                        basket.Remove(item); 
                    }

                    string json = JsonConvert.SerializeObject(basket); 
                    Response.Cookies.Append("basket", json); 
                }
                else
                {
                    return NotFound(); 
                }
            }

            
            return RedirectToAction("Index", "Basket");
        }
        // [Authorize(Roles ="Admin,Member")]
        public async Task<IActionResult> CheckOut()
        {
            OrderVM orderVM = new OrderVM()
            {
                BasketInOrderItemVMs = await _context.BasketItems
                .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(bi => new BasketInOrderItemVM
                {


                    Count = bi.Count,
                    Name = bi.Product.Name,
                    Price = bi.Product.Price,


                }  ).ToListAsync()


            };


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {
            var basketitems = await _context.BasketItems
                .Include(b => b.Product)
                .Where(b => b.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                orderVM.BasketInOrderItemVMs = basketitems.Select(bi => new BasketInOrderItemVM
                {
                    Count = bi.Count,
                    Name = bi.Product.Name,
                    Price = bi.Product.Price,
                    SubTotal = bi.Count * bi.Product.Price


                }).ToList();
                return View(orderVM);
            };

            Order order = new Order
            {
                Address = orderVM.Address,
               CreatedAt = DateTime.Now,
                IsDeleted = false,
                Status = null,
                AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),

                OrderItems = basketitems.Select(b => new OrderItem
                {
                    AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ProductId = b.Product.Id,
                    Price = b.Product.Price,
                    Count = b.Count

                }).ToList(),
                TotalPrice = basketitems.Sum(b => b.Count * b.Product.Price)
            };

            _context.Orders.Add(order);
            _context.BasketItems.RemoveRange(basketitems);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");



        }
    }
}
