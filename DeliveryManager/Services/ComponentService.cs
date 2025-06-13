// Services/IComponentService.cs

using DeliveryManager.Db;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryManager.Services;

// Services/ComponentService.cs

public class ComponentService(DeliveryDbContext context) : IComponentService
{
	public async Task<List<Component>> GetAllAsync()
	{
		return await context.Components
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
}