using ProcurementPlanner.Interfaces;
using ProcurementPlanner.Models;
using ProcurementPlanner.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProcurementPlanner.Components.Pages;

public partial class Components : ComponentBase
{
	private MudDataGrid<Component> dataGrid;
	private List<Component> components = new();
	private bool loading = true;

	[Inject]
	private CompanyContext CompanyContext { get; set; } = null!;

	[Inject]
	public ICompanyService CompanyService { get; set; } = null!;

	private List<Company> companies = new();

	protected override async Task OnInitializedAsync()
	{
		companies = await CompanyService.GetAllAsync();

		// ensure active company is valid
		if (!companies.Any(c => c.Id == CompanyContext.ActiveCompanyId))
			CompanyContext.ActiveCompanyId = companies.FirstOrDefault()?.Id ?? 1;

		await LoadComponents();
	}

	private async Task LoadComponents()
	{
		loading = true;

		try
		{
			components = await ComponentService.GetAllAsync(CompanyContext.ActiveCompanyId);
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
		var parameters = new DialogParameters();
		var options = new DialogOptions
		{
			CloseOnEscapeKey = true,
			MaxWidth = MaxWidth.Small,
			FullWidth = true
		};

		var dialog = await DialogService.ShowAsync<ComponentDialog>("Komponens létrehozása", parameters, options);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await LoadComponents(); // Refresh the grid
		}
	}

	private async Task OnCompanyChanged(int companyId)
	{
		CompanyContext.ActiveCompanyId = companyId;
		await LoadComponents();
		StateHasChanged();
	}

	private async Task OpenEditDialog(Component component)
	{
		var parameters = new DialogParameters<ComponentDialog>
		{
			{x => x.Component, component}
		};

		var options = new DialogOptions
		{
			CloseOnEscapeKey = true,
			MaxWidth = MaxWidth.Small,
			FullWidth = true
		};

		var dialog = await DialogService.ShowAsync<ComponentDialog>("Komponens szerkesztése", parameters, options);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await LoadComponents(); // Refresh the grid
		}
	}

	private async Task OpenDeleteDialog(Component component)
	{
		var confirmed = await DialogService.ShowMessageBox(
		                                                   "Komponens törlése",
		                                                   $"Biztosan törölni szeretnéd ezt a komponenst: '{component.Name}'?",
		                                                   yesText: "Törlés",
		                                                   cancelText: "Mégse");

		if (confirmed == true)
		{
			try
			{
				await ComponentService.DeleteAsync(component.Id);
				Snackbar.Add($"Komponens '{component.Name}' törölve", Severity.Success);
				await LoadComponents(); // Refresh the grid
			}
			catch (Exception ex)
			{
				Snackbar.Add($"Törlési hiba: {ex.Message}", Severity.Error);
			}
		}
	}
}