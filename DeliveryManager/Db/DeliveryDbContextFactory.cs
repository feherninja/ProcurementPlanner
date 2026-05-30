using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DeliveryManager.Db;

public class DeliveryDbContextFactory : IDesignTimeDbContextFactory<DeliveryDbContext>
{
	public DeliveryDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<DeliveryDbContext>();
		var appFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
			?? Directory.GetCurrentDirectory();
		var dbPath = Path.Combine(appFolder, "delivery.db");
		optionsBuilder.UseSqlite($"Data Source={dbPath}");
		return new DeliveryDbContext(optionsBuilder.Options);
	}
}