using ProcurementPlanner.Components.Pages;
using ProcurementPlanner.Models;

namespace ProcurementPlanner.Interfaces;

public interface IComponentService
{
	Task<List<Component>> GetAllAsync(int companyId);
	Task<Component?> GetByIdAsync(int id);
	Task<Component> CreateAsync(Component component);
	Task<Component> UpdateAsync(Component component);
	Task DeleteAsync(int id);
	Task<bool> ExistsAsync(int id);
	Task UpdateStockLevelsAsync(List<OrderPlannerBase.RequiredComponent> requiredComponents);
	Task<int> CountByCompanyAsync(int companyId);
}