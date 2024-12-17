namespace Pronia.Areas.ViewModels
{
    public class PaginatedVM<T>
    {
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public List<GetProductAdminVM> ProductAdminVMs { get; set; }
    }
}
