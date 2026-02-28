// Services/IComponentService.cs

using DeliveryManager.Components.Pages;
using DeliveryManager.Db;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryManager.Services;

// Services/ComponentService.cs

public class ComponentService(DeliveryDbContext context) : IComponentService
{
	public async Task<List<Component>> GetAllAsync(int companyId)
	{
		return await context.Components
		                    .Where(c => c.CompanyId == companyId)
		                    .OrderBy(c => c.Name)
		                    .ToListAsync();
	}

	public async Task<Component?> GetByIdAsync(int id)
	{
		return await context.Components.FindAsync(id);
	}

	public async Task<Component> CreateAsync(Component component)
	{
		context.Components.Add(component);
		await context.SaveChangesAsync();
		return component;
	}

	public async Task<Component> UpdateAsync(Component component)
	{
		context.Components.Update(component);
		await context.SaveChangesAsync();
		return component;
	}

	public async Task DeleteAsync(int id)
	{
		var component = await context.Components.FindAsync(id);

		if (component != null)
		{
			context.Components.Remove(component);
			await context.SaveChangesAsync();
		}
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await context.Components.AnyAsync(c => c.Id == id);
	}

	public async Task UpdateStockLevelsAsync(List<DeliveryPlannerBase.RequiredComponent> requiredComponents)
	{
		foreach (var requiredComponent in requiredComponents)
		{
			var component = await context.Components.FindAsync(requiredComponent.Component.Id);

			if (component != null)
			{
				// Calculate how much we ordered (based on package size)
				int orderedQuantity = requiredComponent.CalculateRequiredOrderQuantity() * component.OrderPackageSize;

				// New stock = current stock + what we ordered - what we're shipping
				component.Stock = component.Stock + orderedQuantity - requiredComponent.TotalQuantity;

				context.Components.Update(component);
			}
		}

		await context.SaveChangesAsync();
	}
}