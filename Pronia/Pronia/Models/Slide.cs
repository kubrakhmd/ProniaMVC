﻿using System.ComponentModel.DataAnnotations.Schema;
using Pronia.Models.Base;

namespace Pronia.Models
{
	public class Slide : BaseEntity
    {
        public string Title {  get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Image {  get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
