using Velopack;
using Velopack.Locators;
using Velopack.Sources;

namespace ProcurementPlanner.Services;

// Database location and self-update. Installed builds keep the database in <install dir>\data,
// because updates replace the app folder; dev builds keep it next to the executable.
public class AppUpdateService
{
	private const string ReleasesRepoUrl = "https://github.com/feherninja/ProcurementPlanner";

	private readonly UpdateManager? updateManager;
	private UpdateInfo? pendingUpdate;

	public string DatabasePath { get; }

	// True on the first start with this data folder, until the user has answered the import question
	public bool DatabaseCreated { get; set; }

	// Shown once after the UI reloads, e.g. after a database import
	public string? StartupNotice { get; set; }

	public AppUpdateService()
	{
		var isInstalled = VelopackLocator.IsCurrentSet && VelopackLocator.Current.CurrentlyInstalledVersion != null;

		var dataFolder = isInstalled
			? Path.Combine(VelopackLocator.Current.RootAppDir!, "data")
			: Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;

		Directory.CreateDirectory(dataFolder);
		DatabasePath = Path.Combine(dataFolder, "procurement.db");
		DatabaseCreated = !File.Exists(DatabasePath);

		if (!isInstalled)
			return;

		// PROCUREMENTPLANNER_UPDATE_SOURCE (a local folder or URL with Velopack releases) lets a release be tested before it is published
		var testSource = Environment.GetEnvironmentVariable("PROCUREMENTPLANNER_UPDATE_SOURCE");
		updateManager = string.IsNullOrWhiteSpace(testSource)
			? new UpdateManager(new GithubSource(ReleasesRepoUrl, null, false))
			: new UpdateManager(testSource);
	}

	public string? CurrentVersion => updateManager?.CurrentVersion?.ToString();

	// Checks and downloads in the background; returns the new version when one is ready to apply.
	// A downloaded update is also applied automatically on the next start.
	public async Task<string?> DownloadUpdateAsync()
	{
		if (updateManager == null)
			return null;

		try
		{
			pendingUpdate = await updateManager.CheckForUpdatesAsync();
			if (pendingUpdate == null)
				return null;

			await updateManager.DownloadUpdatesAsync(pendingUpdate);
			return pendingUpdate.TargetFullRelease.Version.ToString();
		}
		catch
		{
			// Offline or GitHub unreachable: try again on the next start
			pendingUpdate = null;
			return null;
		}
	}

	public void ApplyUpdateAndRestart()
	{
		if (updateManager != null && pendingUpdate != null)
			updateManager.ApplyUpdatesAndRestart(pendingUpdate.TargetFullRelease);
	}
}
