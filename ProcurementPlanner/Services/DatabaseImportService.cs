using ProcurementPlanner.Db;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ProcurementPlanner.Services;

// Replaces the app database with one from another install, typically an older app version.
// Works on a copy so the source file is never modified, migrates the copy, backs up the
// current database and only then swaps the contents in.
public class DatabaseImportService(AppUpdateService appUpdateService)
{
	private static readonly string[] RequiredTables = ["__EFMigrationsHistory", "Companies", "Items", "Components", "ItemComponents"];

	public string BackupFolder => Path.Combine(Path.GetDirectoryName(appUpdateService.DatabasePath)!, "backups");

	// Returns null on success, otherwise a message for the user
	public async Task<string?> ImportAsync(string sourcePath)
	{
		var workFolder = Path.Combine(Path.GetTempPath(), $"ProcurementPlanner-import-{Guid.NewGuid():N}");
		Directory.CreateDirectory(workFolder);

		try
		{
			// The WAL side files may hold changes not yet written to the main file
			var stagedPath = Path.Combine(workFolder, "import.db");
			File.Copy(sourcePath, stagedPath);
			foreach (var suffix in new[] { "-wal", "-shm" })
			{
				if (File.Exists(sourcePath + suffix))
					File.Copy(sourcePath + suffix, stagedPath + suffix);
			}

			var stagedConnectionString = $"Data Source={stagedPath};Pooling=False";

			var error = await ValidateAsync(stagedConnectionString);
			if (error != null)
				return error;

			await using (var context = CreateContext(stagedConnectionString))
				await context.Database.MigrateAsync();

			SqliteConnection.ClearAllPools();
			BackupCurrentDatabase();
			ReplaceCurrentDatabase(stagedConnectionString);
			return null;
		}
		catch (SqliteException ex)
		{
			return $"A kiválasztott fájl nem olvasható adatbázisként ({ex.Message}).";
		}
		catch (IOException ex)
		{
			return $"A fájl nem másolható ({ex.Message}). Ha a régi program fut, zárd be, és próbáld újra.";
		}
		finally
		{
			SqliteConnection.ClearAllPools();
			try
			{
				Directory.Delete(workFolder, true);
			}
			catch (IOException)
			{
				// Temp leftovers are harmless
			}
		}
	}

	private async Task<string?> ValidateAsync(string connectionString)
	{
		await using var connection = new SqliteConnection(connectionString);
		await connection.OpenAsync();

		var tables = await ReadColumnAsync(connection, "SELECT name FROM sqlite_master WHERE type = 'table'");
		if (RequiredTables.Any(t => !tables.Contains(t)))
			return "A kiválasztott fájl nem ennek a programnak az adatbázisa.";

		// Older databases are migrated forward; a newer one would lose data in this version
		var applied = await ReadColumnAsync(connection, "SELECT MigrationId FROM __EFMigrationsHistory");
		await using var context = CreateContext($"Data Source={appUpdateService.DatabasePath}");
		var known = context.Database.GetMigrations().ToHashSet();
		if (applied.Any(m => !known.Contains(m)))
			return "A kiválasztott adatbázis a program egy újabb verziójából származik, ezzel a verzióval nem tölthető be.";

		return null;
	}

	private void BackupCurrentDatabase()
	{
		if (!File.Exists(appUpdateService.DatabasePath))
			return;

		Directory.CreateDirectory(BackupFolder);
		var backupPath = Path.Combine(BackupFolder, $"procurement-{DateTime.Now:yyyyMMdd-HHmmss}.db");

		using var current = new SqliteConnection($"Data Source={appUpdateService.DatabasePath}");
		using var backup = new SqliteConnection($"Data Source={backupPath};Pooling=False");
		current.Open();
		backup.Open();
		current.BackupDatabase(backup);
	}

	// SQLite's backup API swaps the whole content and copes with WAL mode and pooled connections
	private void ReplaceCurrentDatabase(string stagedConnectionString)
	{
		using var staged = new SqliteConnection(stagedConnectionString);
		using var current = new SqliteConnection($"Data Source={appUpdateService.DatabasePath}");
		staged.Open();
		current.Open();
		staged.BackupDatabase(current);
	}

	private static ProcurementDbContext CreateContext(string connectionString) =>
		new(new DbContextOptionsBuilder<ProcurementDbContext>().UseSqlite(connectionString).Options);

	private static async Task<HashSet<string>> ReadColumnAsync(SqliteConnection connection, string sql)
	{
		await using var command = connection.CreateCommand();
		command.CommandText = sql;
		await using var reader = await command.ExecuteReaderAsync();

		var values = new HashSet<string>();
		while (await reader.ReadAsync())
			values.Add(reader.GetString(0));

		return values;
	}
}
