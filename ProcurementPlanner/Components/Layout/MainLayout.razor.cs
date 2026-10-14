using ProcurementPlanner.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProcurementPlanner.Components.Layout;

public partial class MainLayout
{
	[Inject]
	private AppUpdateService AppUpdateService { get; set; } = null!;

	[Inject]
	private ISnackbar Snackbar { get; set; } = null!;

	[Inject]
	private DatabaseImportFlow DatabaseImportFlow { get; set; } = null!;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender)
			return;

		// Start the download first so it runs while the user answers any dialog below
		var download = AppUpdateService.DownloadUpdateAsync();

		if (AppUpdateService.StartupNotice != null)
		{
			Snackbar.Add(AppUpdateService.StartupNotice, Severity.Success, config => config.RequireInteraction = true);
			AppUpdateService.StartupNotice = null;
		}

		// A fresh install usually has data in the previous app version, so offer to bring it over once
		if (AppUpdateService.DatabaseCreated)
		{
			AppUpdateService.DatabaseCreated = false;
			await DatabaseImportFlow.OfferOnFirstStartAsync();
		}

		var newVersion = await download;
		if (newVersion == null)
			return;

		Snackbar.Add($"Új verzió letöltve ({newVersion}). A program következő indításakor települ.",
			Severity.Info, config =>
			{
				config.RequireInteraction = true;
				config.Action = "Újraindítás most";
				config.OnClick = _ =>
				{
					AppUpdateService.ApplyUpdateAndRestart();
					return Task.CompletedTask;
				};
			});
	}
}
