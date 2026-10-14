using ProcurementPlanner.Interfaces;
using ProcurementPlanner.Models;
using ProcurementPlanner.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProcurementPlanner.Components.Pages;

public partial class ItemsBase : ComponentBase
{
	[Inject]
	private IItemService ItemService { get; set; } = null!;

	[Inject]
	private ISnackbar Snackbar { get; set; } = null!;

	[Inject]
	private IDialogService DialogService { get; set; } = null!;

	[Inject]
	public CompanyContext CompanyContext { get; set; } = null!;

	[Inject]
	public ICompanyService CompanyService { get; set; } = null!;

	public List<Company> companies = new();

	public MudDataGrid<Item> dataGrid = null!;
	public List<Item> items = new();
	public bool loading = true;

	protected override async Task OnInitializedAsync()
	{
		companies = await CompanyService.GetAllAsync();

		if (!companies.Any(c => c.Id == CompanyContext.ActiveCompanyId))
			CompanyContext.ActiveCompanyId = companies.FirstOrDefault()?.Id ?? 1;

		await LoadItems();
	}

	public async Task LoadItems()
	{
		loading = true;

		try
		{
			items = await ItemService.GetAllAsync(CompanyContext.ActiveCompanyId);
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Betöltési hiba: {ex.Message}", Severity.Error);
		}
		finally
		{
			loading = false;
		}
	}

	public async Task OnCompanyChanged(int companyId)
	{
		CompanyContext.ActiveCompanyId = companyId;
		await LoadItems();
		StateHasChanged();
	}

	public async Task OpenCreateDialog()
	{
		var parameters = new DialogParameters();
		var options = new DialogOptions
		{
			CloseOnEscapeKey = true,
			MaxWidth = MaxWidth.Small,
			FullWidth = true
		};

		var dialog = await DialogService.ShowAsync<ItemDialog>("Termék létrehozása", parameters, options);
		var result = await dialog.Result;

		if (!result.Canceled)
			await LoadItems(); // Refresh the grid
	}

	public async Task OpenAddComponentDialog(Item item)
	{
		var parameters = new DialogParameters<ComponentSelectionDialog>
		{
			{x => x.Item, item}
		};

		var options = new DialogOptions
		{
			CloseOnEscapeKey = true,
			MaxWidth = MaxWidth.ExtraLarge,
			FullWidth = true
		};

		var dialog = await DialogService.ShowAsync<ComponentSelectionDialog>("Komponens hozzáadása", parameters, options);
		var result = await dialog.Result;

		if (!result.Canceled)
			await LoadItems(); // Refresh to show new components
	}

	public async Task RemoveComponentFromItem(Item item, Component component)
	{
		var confirmed = await DialogService.ShowMessageBox(
		                                                   "Komponens eltávolítása",
		                                                   $"'{component.Name}' komponens eltávolítása '{item.Name}' termékből?",
		                                                   yesText: "Törlés",
		                                                   cancelText: "Mégse");

		if (confirmed == true)
		{
			try
			{
				await ItemService.RemoveComponentFromItemAsync(item.Id, component.Id);
				Snackbar.Add($"'{component.Name}' komponens eltávolítva '{item.Name}' termékből", Severity.Success);
				await LoadItems(); // Refresh to show updated component list
			}
			catch (Exception ex)
			{
				Snackbar.Add($"Törlési hiba: {ex.Message}", Severity.Error);
			}
		}
	}

	public async Task OpenEditDialog(Item item)
	{
		var parameters = new DialogParameters<ItemDialog>
		{
			{x => x.Item, item}
		};

		var options = new DialogOptions
		{
			CloseOnEscapeKey = true,
			MaxWidth = MaxWidth.Small,
			FullWidth = true
		};

		var dialog = await DialogService.ShowAsync<ItemDialog>("Termék szerkesztése", parameters, options);
		var result = await dialog.Result;

		if (!result.Canceled)
			await LoadItems(); // Refresh the grid
	}

	public async Task OpenDeleteDialog(Item item)
	{
		var confirmed = await DialogService.ShowMessageBox(
		                                                   "Termék törlése",
		                                                   $"Biztosan törölni szeretnéd ezt a termék: '{item.Name}'?",
		                                                   yesText: "Törlés",
		                                                   cancelText: "Mégse");

		if (confirmed == true)
		{
			try
			{
				await ItemService.DeleteAsync(item.Id);
				Snackbar.Add($"Termék '{item.Name}' sikeresen törölve", Severity.Success);
				await LoadItems();
			}
			catch (Exception ex)
			{
				Snackbar.Add($"Törlési hiba: {ex.Message}", Severity.Error);
			}
		}
	}
}