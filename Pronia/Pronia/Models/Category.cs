using System.ComponentModel.DataAnnotations;
using Pronia.Models.Base;

namespace Pronia.Models
{
	public class Category:BaseEntity

    {
        [Required(ErrorMessage = "Ad mutleqdir")]
        [MaxLength(30, ErrorMessage = "Uzunlugu 30dan cox ola bilmez")]

        public string Name{ get; set; }
		public List<Product>? Products { get; set; }

	}
}
