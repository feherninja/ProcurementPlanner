using DeliveryManager.Db;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryManager.Services;

public class CompanyService(DeliveryDbContext context) : ICompanyService
{
	private const int DefaultCompanyId = 1;

	public async Task<List<Company>> GetAllAsync()
	{
		return await context.Companies
		                    .OrderBy(c => c.Name)
		                    .ToListAsync();
	}

	public async Task<Company?> GetByIdAsync(int id)
	{
		return await context.Companies.FindAsync(id);
	}

	public async Task<Company> CreateAsync(Company company)
	{
		context.Companies.Add(company);
		await context.SaveChangesAsync();
		return company;
	}

	public async Task<Company> UpdateAsync(Company company)
	{
		// ✅ tracking-safe: update the tracked instance (or load one), don't Attach/Update a possibly-detached instance
		var tracked = await context.Companies.FindAsync(company.Id);
		if (tracked is null)
			throw new InvalidOperationException("Cég nem található.");

		tracked.Name = company.Name;
		tracked.ShortName = company.ShortName;
		tracked.Address = company.Address;
		tracked.Email = company.Email;

		await context.SaveChangesAsync();
		return tracked;
	}

	public async Task DeleteAsync(int id)
	{
		if (id == DefaultCompanyId)
			throw new InvalidOperationException("Az alapértelmezett cég nem törölhető.");

		var company = await context.Companies.FindAsync(id);

		if (company != null)
		{
			context.Companies.Remove(company);
			await context.SaveChangesAsync();
		}
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await context.Companies.AnyAsync(c => c.Id == id);
	}
}