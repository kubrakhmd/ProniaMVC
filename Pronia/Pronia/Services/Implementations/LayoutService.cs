
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
       

        public LayoutService(ProniaDBContext context)
        {
            _context = context;
            ;
        }

     

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }
    }
}