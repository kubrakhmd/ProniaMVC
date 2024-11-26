﻿using Microsoft.EntityFrameworkCore;

namespace Pronia.Models.Base
{
	public abstract class BaseEntity

	{

        public int  Id { get; set; }
		public bool IsDeleted { get; set; }
		
		public DateTime CreatedAt { get; set; }




    }
}
