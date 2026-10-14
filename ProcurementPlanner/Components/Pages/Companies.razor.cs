using ProcurementPlanner.Interfaces;
using ProcurementPlanner.Models;
using ProcurementPlanner.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProcurementPlanner.Components.Pages;

public partial class Companies : ComponentBase
{
	private MudDataGrid<Company> dataGrid;
	private List<Company> companies = new();
	private bool loading = true;

	[Inject] public IItemService ItemService { get; set; }
	[Inject] public IComponentService ComponentService { get; set; }
	
	protected override async Task OnInitializedAsync() => await LoadCompanies();

	private async Task LoadCompanies()
	{
		loading = true;

		try
		{
			companies = await CompanyService.GetAllAsync();
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Hiba: {ex.Message}", Severity.Error);
		}
		finally
		{
			loading = false;
		}
	}

	private async Task OpenCreateDialog()
	{
		var options = new DialogOptions {CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true};
		var dialog = await DialogService.ShowAsync<CompanyDialog>("Cég létrehozása", new DialogParameters(), options);

		var result = await dialog.Result;
		if (!result.Canceled) await LoadCompanies();
	}

	private async Task OpenEditDialog(Company company)
	{
		var parameters = new DialogParameters<CompanyDialog> {{x => x.Company, company}};
		var options = new DialogOptions {CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true};

		var dialog = await DialogService.ShowAsync<CompanyDialog>("Cég szerkesztése", parameters, options);

		var result = await dialog.Result;
		if (!result.Canceled) await LoadCompanies();
	}

	private async Task OpenDeleteDialog(Company company)
	{
		try
		{
			var items = await ItemService.CountByCompanyAsync(company.Id);
			var components = await ComponentService.CountByCompanyAsync(company.Id);

			var confirmed = await DialogService.ShowMessageBox(
			                                                   "Cég törlése",
			                                                   $"Biztosan törölni szeretnéd ezt a céget: '{company.Name}'?\n\n" +
			                                                   $"Ez a művelet törölni fog mindent ami ehhez tartozik:\n"        +
			                                                   $"- Termékek: {items}\n"                                           +
			                                                   $"- Komponensek: {components}\n\n"                               +
			                                                   "A művelet nem visszavonható.",
			                                                   yesText: "Törlés",
			                                                   cancelText: "Mégse");

			if (confirmed != true) return;

			await CompanyService.DeleteAsync(company.Id);
			Snackbar.Add($"Cég '{company.Name}' törölve", Severity.Success);
			await LoadCompanies();
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Törlési hiba: {ex.Message}", Severity.Error);
		}
	}
}