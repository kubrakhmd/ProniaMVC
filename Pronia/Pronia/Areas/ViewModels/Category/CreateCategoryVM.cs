﻿using Pronia.Models;

namespace Pronia.Areas.ViewModels
{
    public class CreateCategoryVM
    {
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
