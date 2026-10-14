using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProcurementPlanner.Services;

// The user-facing steps of a database import: confirm, pick the file, import, reload
public class DatabaseImportFlow(
	DatabaseImportService importService,
	AppUpdateService appUpdateService,
	IDialogService dialogService,
	ISnackbar snackbar,
	NavigationManager navigationManager)
{
	public async Task OfferOnFirstStartAsync()
	{
		var load = await dialogService.ShowMessageBox(
			"Meglévő adatbázis betöltése",
			(MarkupString)("A programnak még nincs adatbázisa ezen a gépen.<br/><br/>" +
			"Szeretnél betölteni egy meglévő adatbázist, például a program korábbi verziójából (delivery.db)?<br/><br/>" +
			"Ezt később is megteheted a Beállítások oldalon."),
			yesText: "Igen, betöltöm",
			cancelText: "Nem");

		if (load == true)
			await PickAndImportAsync();
	}

	public async Task ReplaceAsync()
	{
		var confirmed = await dialogService.ShowMessageBox(
			"Adatbázis betöltése",
			(MarkupString)("<strong>A betöltés lecseréli a jelenlegi összes adatot</strong> (cégek, termékek, komponensek, készlet) " +
			"a kiválasztott adatbázis tartalmára. A két adatbázis nem fésülődik össze.<br/><br/>" +
			$"A jelenlegi adatokról előtte automatikusan biztonsági mentés készül ide:<br/>{importService.BackupFolder}"),
			yesText: "Kiválasztás és csere",
			cancelText: "Mégse");

		if (confirmed == true)
			await PickAndImportAsync();
	}

	private async Task PickAndImportAsync()
	{
		var file = await FilePicker.Default.PickAsync(new PickOptions
		{
			PickerTitle = "Betöltendő adatbázis kiválasztása",
			FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
			{
				{ DevicePlatform.WinUI, [".db"] }
			})
		});

		if (file == null)
			return;

		var error = await importService.ImportAsync(file.FullPath);
		if (error != null)
		{
			snackbar.Add(error, Severity.Error, config => config.RequireInteraction = true);
			return;
		}

		// Reload the UI so every page reads the new data; the notice survives because the service is a singleton
		appUpdateService.StartupNotice = $"Az adatbázis betöltése sikerült. A korábbi adatok mentése: {importService.BackupFolder}";
		navigationManager.NavigateTo("/", forceLoad: true);
	}
}
