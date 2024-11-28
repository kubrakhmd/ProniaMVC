using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ViewModels.Slide
{
    public class CreateSlideVM
    {
       

            public string Title { get; set; }
            public string SubTitle { get; set; }
            public string Description { get; set; }

            public int Order { get; set; }
            [Required]
            public IFormFile Photo { get; set; }
    }
}

