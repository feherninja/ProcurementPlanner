using DeliveryManager.Components.Pages;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using DeliveryManager.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components;

public partial class ItemSelectionDialogBase : ComponentBase
{
	[CascadingParameter]
	public IMudDialogInstance MudDialog { get; set; } = null!;

	[Parameter]
	public List<DeliveryPlannerBase.SelectedItem> ExistingItems { get; set; } = new();

	[Inject]
	private IItemService ItemService { get; set; } = null!;

	[Inject]
	private ISnackbar Snackbar { get; set; } = null!;

	[Inject]
	private CompanyContext CompanyContext { get; set; } = null!;

	public MudDataGrid<Item> dataGrid = null!;
	public List<Item> availableItems = new();
	public HashSet<Item> selectedItems = new();
	public Dictionary<int, int> itemQuantities = new();
	public bool loading = false;

	protected override async Task OnInitializedAsync()
	{
		await LoadAvailableItems();
	}

	private async Task LoadAvailableItems()
	{
		loading = true;

		try
		{
			// Get all items
			var allItems = await ItemService.GetAllAsync(CompanyContext.ActiveCompanyId);

			// Get IDs of items already in the delivery planner
			var existingItemIds = ExistingItems.Select(si => si.Item.Id).ToHashSet();

			// Filter out items already in the planner
			availableItems = allItems
			                 .Where(i => !existingItemIds.Contains(i.Id))
			                 .ToList();
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

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			// Initialize quantities for all items (default to 1)
			foreach (var item in availableItems)
			{
				itemQuantities[item.Id] = 1;
			}

			StateHasChanged();
		}
	}

	public void Cancel()
	{
		MudDialog.Cancel();
	}

	public async Task AddSelectedItems()
	{
		if (!selectedItems.Any())
		{
			Snackbar.Add("Válasszon ki legalább egy terméket", Severity.Warning);
			return;
		}

		try
		{
			var itemsToAdd = selectedItems.Select(item => new DeliveryPlannerBase.SelectedItem
			{
				Item = item,
				Quantity = itemQuantities.GetValueOrDefault(item.Id, 1)
			}).ToList();

			MudDialog.Close(DialogResult.Ok(itemsToAdd));
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Hozzáadás közbeni hiba: {ex.Message}", Severity.Error);
		}
	}
}