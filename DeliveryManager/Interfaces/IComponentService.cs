using DeliveryManager.Models;

namespace DeliveryManager.Interfaces;

public interface IComponentService
{
	Task<List<Component>> GetAllAsync();
	Task<Component?> GetByIdAsync(int id);
	Task<Component> CreateAsync(Component component);
	Task<Component> UpdateAsync(Component component);
	Task DeleteAsync(int id);
	Task<bool> ExistsAsync(int id);
}