using ProcurementPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace ProcurementPlanner.Db;

public class ProcurementDbContext : DbContext
{
	public ProcurementDbContext(DbContextOptions<ProcurementDbContext> options) : base(options)
	{
	}

	public DbSet<Component> Components => Set<Component>();
	public DbSet<Item> Items => Set<Item>();
	public DbSet<ItemComponent> ItemComponents => Set<ItemComponent>();
	public DbSet<Delivery> Deliveries => Set<Delivery>();
	public DbSet<DeliveryItem> DeliveryItems => Set<DeliveryItem>();
	public DbSet<Company> Companies => Set<Company>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Apply all configurations from the assembly
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcurementDbContext).Assembly);
	}

	public override int SaveChanges()
	{
		UpdateTimestamps();
		return base.SaveChanges();
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		UpdateTimestamps();
		return await base.SaveChangesAsync(cancellationToken);
	}

	private void UpdateTimestamps()
	{
		var entries = ChangeTracker.Entries()
		                           .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

		foreach (var entry in entries)
		{
			switch (entry)
			{
				case {State: EntityState.Added}:
				{
					// Check if the entity has a CreatedAt property
					var createdAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");

					if (createdAtProperty != null && createdAtProperty.CurrentValue == null)
					{
						createdAtProperty.CurrentValue = DateTime.UtcNow;
					}

					break;
				}
				case {State: EntityState.Modified}:
				{
					// Check if the entity has an UpdatedAt property
					var updatedAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");

					if (updatedAtProperty != null)
					{
						updatedAtProperty.CurrentValue = DateTime.UtcNow;
					}

					break;
				}
			}
		}
	}
}