using DeliveryManager.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components.Pages;

public partial class Components : ComponentBase
{
	private MudDataGrid<Component> dataGrid;
	private List<Component> components = new();
	private bool loading = true;

	protected override async Task OnInitializedAsync()
	{
		await LoadComponents();
	}

	private async Task LoadComponents()
	{
		loading = true;

		try
		{
			components = await ComponentService.GetAllAsync();
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Error loading components: {ex.Message}", Severity.Error);
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

		var dialog = await DialogService.ShowAsync<ComponentDialog>("Create Component", parameters, options);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await LoadComponents(); // Refresh the grid
		}
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

		var dialog = await DialogService.ShowAsync<ComponentDialog>("Edit Component", parameters, options);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await LoadComponents(); // Refresh the grid
		}
	}

	private async Task OpenDeleteDialog(Component component)
	{
		var confirmed = await DialogService.ShowMessageBox(
		                                                   "Delete Component",
		                                                   $"Are you sure you want to delete '{component.Name}'? This action cannot be undone.",
		                                                   yesText: "Delete",
		                                                   cancelText: "Cancel");

		if (confirmed == true)
		{
			try
			{
				await ComponentService.DeleteAsync(component.Id);
				Snackbar.Add($"Component '{component.Name}' deleted successfully", Severity.Success);
				await LoadComponents(); // Refresh the grid
			}
			catch (Exception ex)
			{
				Snackbar.Add($"Error deleting component: {ex.Message}", Severity.Error);
			}
		}
	}
}