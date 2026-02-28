using DeliveryManager.Models;

namespace DeliveryManager.Interfaces;

public interface IItemService
{
	Task<List<Item>> GetAllAsync(int companyId);
	Task<Item?> GetByIdAsync(int id);
	Task<Item> CreateAsync(Item item);
	Task<Item> UpdateAsync(Item item);
	Task DeleteAsync(int id);
	Task<bool> ExistsAsync(int id);
	Task AddComponentToItemAsync(int itemId, int componentId, int quantity);
	Task RemoveComponentFromItemAsync(int itemId, int componentId);
	Task UpdateComponentQuantityAsync(int itemId, int componentId, int quantity);
}