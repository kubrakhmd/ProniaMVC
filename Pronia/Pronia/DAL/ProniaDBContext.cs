using System.Runtime.InteropServices;
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



	}





    }
