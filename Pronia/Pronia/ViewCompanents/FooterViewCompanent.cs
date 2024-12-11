using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;

namespace ProniaMVC.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly ProniaDBContext _context;

        public FooterViewComponent(ProniaDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return View(settings);

        }
    }
}