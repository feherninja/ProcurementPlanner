using DeliveryManager.Models;

namespace DeliveryManager.Interfaces;

public interface ICompanyService
{
	Task<List<Company>> GetAllAsync();
	Task<Company?> GetByIdAsync(int id);
	Task<Company> CreateAsync(Company company);
	Task<Company> UpdateAsync(Company company);
	Task DeleteAsync(int id);
	Task<bool> ExistsAsync(int id);
}