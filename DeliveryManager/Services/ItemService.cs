using DeliveryManager.Db;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryManager.Services;

public class ItemService(DeliveryDbContext context) : IItemService
{
	public async Task<List<Item>> GetAllAsync(int companyId)
	{
		return await context.Items
		                    .Where(i => i.CompanyId == companyId)
		                    .Include(i => i.ItemComponents)
		                    .ThenInclude(ic => ic.Component)
		                    .OrderBy(i => i.Name)
		                    .ToListAsync();
	}

	public async Task<Item?> GetByIdAsync(int id)
	{
		return await context.Items
		                    .Include(i => i.ItemComponents)
		                    .ThenInclude(ic => ic.Component)
		                    .FirstOrDefaultAsync(i => i.Id == id);
	}

	public async Task<Item> CreateAsync(Item item)
	{
		context.Items.Add(item);
		await context.SaveChangesAsync();
		return item;
	}

	public async Task<Item> UpdateAsync(Item item)
	{
		context.Items.Update(item);
		await context.SaveChangesAsync();
		return item;
	}

	public async Task DeleteAsync(int id)
	{
		var item = await context.Items.FindAsync(id);

		if (item != null)
		{
			context.Items.Remove(item);
			await context.SaveChangesAsync();
		}
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await context.Items.AnyAsync(i => i.Id == id);
	}

	public async Task AddComponentToItemAsync(int itemId, int componentId, int quantity)
	{
		// Check if relationship already exists
		var existing = await context.ItemComponents
		                            .FirstOrDefaultAsync(ic => ic.ItemId == itemId && ic.ComponentId == componentId);

		if (existing != null)
		{
			// Update existing quantity
			existing.Quantity = quantity;
		}
		else
		{
			// Create new relationship
			var itemComponent = new ItemComponent
			{
				ItemId = itemId,
				ComponentId = componentId,
				Quantity = quantity
			};

			context.ItemComponents.Add(itemComponent);
		}

		await context.SaveChangesAsync();
	}

	public async Task RemoveComponentFromItemAsync(int itemId, int componentId)
	{
		var itemComponent = await context.ItemComponents
		                                 .FirstOrDefaultAsync(ic => ic.ItemId == itemId && ic.ComponentId == componentId);

		if (itemComponent != null)
		{
			context.ItemComponents.Remove(itemComponent);
			await context.SaveChangesAsync();
		}
	}

	public async Task UpdateComponentQuantityAsync(int itemId, int componentId, int quantity)
	{
		var itemComponent = await context.ItemComponents
		                                 .FirstOrDefaultAsync(ic => ic.ItemId == itemId && ic.ComponentId == componentId);

		if (itemComponent != null)
		{
			itemComponent.Quantity = quantity;
			await context.SaveChangesAsync();
		}
	}

	public async Task<int> CountByCompanyAsync(int companyId)
	{
		return context.Items.Count(i => i.CompanyId == companyId);
	}
}