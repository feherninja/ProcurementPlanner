using ProcurementPlanner.Components.Pages;
using ProcurementPlanner.Interfaces;
using ProcurementPlanner.Models;
using ProcurementPlanner.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProcurementPlanner.Components;

public partial class ItemSelectionDialogBase : ComponentBase
{
	[CascadingParameter]
	public IMudDialogInstance MudDialog { get; set; } = null!;

	[Parameter]
	public List<OrderPlannerBase.SelectedItem> ExistingItems { get; set; } = new();

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
	public string searchString = string.Empty;

	protected bool QuickFilter(Item item)
	{
		if (string.IsNullOrWhiteSpace(searchString))
			return true;

		return Matches(item.PartNumber) || Matches(item.DrawingNumber) || Matches(item.Name) || Matches(item.Description);

		bool Matches(string? value) => value?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true;
	}

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
			var itemsToAdd = selectedItems.Select(item => new OrderPlannerBase.SelectedItem
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