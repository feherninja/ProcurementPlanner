using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components.Pages;

public partial class Companies : ComponentBase
{
	private MudDataGrid<Company> dataGrid;
	private List<Company> companies = new();
	private bool loading = true;

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
		var confirmed = await DialogService.ShowMessageBox(
		                                                   "Cég törlése",
		                                                   $"Biztosan törölni szeretnéd ezt a céget: '{company.Name}'?",
		                                                   yesText: "Törlés",
		                                                   cancelText: "Mégse");

		if (confirmed != true) return;

		try
		{
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