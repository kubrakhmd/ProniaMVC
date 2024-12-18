using Pronia.Models;

namespace Pronia.ViewModels
{
    public class ShopVM
    {
        public List<GetProductVM> Products { get; set; }

        public List<CategoryGetVM> Categories { get; set; }

        public string Search { get; set; }

        public int? CategoryId { get; set; }

        public int Key { get; set; }

        public double TotalPage { get; set; }

        public int CurrentPage { get; set; }

        
        }
    }

