﻿using Pronia.Models.Base;

namespace Pronia.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int Count { get; set; }



        public int ProductId { get; set; }
        public Product  Product { get; set; }
        public string AppUserId  { get; set; }
        public AppUser User { get; set; }



    }
}
