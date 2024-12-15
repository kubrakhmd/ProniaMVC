using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.DAL;
using Pronia.Areas.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {

        
            private readonly ProniaDBContext _context;
            private readonly UserManager<AppUser> _usermaneger;

            public HomeController(ProniaDBContext context, UserManager<AppUser> usermaneger)
            {
                _context = context;
                _usermaneger = usermaneger;
            }
            public async Task<IActionResult> Index()
            {
                List<OrderAdminVM> orderVM = await _context.Orders
                       .Include(o => o.AppUser)
                       .Select(o => new OrderAdminVM
                       {
                           OrderId = o.Id,
                           UserName = o.AppUser.UserName,
                      
                           Subtotal = o.TotalPrice,
                           Status = o.Status,
                           CreatedAt = DateTime.Now.ToString("dddd, dd MMMM yyyy")
                       }).ToListAsync();
                return View(orderVM);
            }

        }
    }
