using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProcurementPlanner.Db;

public class ProcurementDbContextFactory : IDesignTimeDbContextFactory<ProcurementDbContext>
{
	public ProcurementDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ProcurementDbContext>();
		var appFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
			?? Directory.GetCurrentDirectory();
		var dbPath = Path.Combine(appFolder, "procurement.db");
		optionsBuilder.UseSqlite($"Data Source={dbPath}");
		return new ProcurementDbContext(optionsBuilder.Options);
	}
}