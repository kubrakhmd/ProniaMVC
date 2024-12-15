using Pronia.ViewModels;

namespace Pronia.Services.InterFaces
{
    public interface IBasketService
    {
        Task<List<BasketItemVM>> GetBasketAsync();

    }
}
