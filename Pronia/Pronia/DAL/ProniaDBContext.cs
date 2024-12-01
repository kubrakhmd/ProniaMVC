﻿using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.DAL
{

    public class ProniaDBContext : DbContext

    {

        public ProniaDBContext(DbContextOptions<ProniaDBContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Slide>Slides { get; set; }  
        public DbSet<Product>Products { get; set; }
	    public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Tag> Tags { get; set; }    
        public DbSet<ProductTag>ProductTags { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
    }
    }
