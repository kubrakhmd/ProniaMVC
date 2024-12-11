using Pronia.ViewModels;

namespace Pronia.Services.InterFaces
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
        Task <List<BasketItemVM>> GetBasketAsync();
    
    }
}
