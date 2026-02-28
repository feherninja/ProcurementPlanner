using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using DeliveryManager.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components;

public partial class ComponentSelectionDialogBase : ComponentBase
{
	[CascadingParameter]
	public IMudDialogInstance MudDialog { get; set; } = null!;

	[Parameter]
	public Item Item { get; set; } = null!;

	[Inject]
	private IComponentService ComponentService { get; set; } = null!;

	[Inject]
	private IItemService ItemService { get; set; } = null!;

	[Inject]
	private ISnackbar Snackbar { get; set; } = null!;

	[Inject]
	private CompanyContext CompanyContext { get; set; } = null!;

	public MudDataGrid<Component> dataGrid = null!;
	public List<Component> availableComponents = new();
	public HashSet<Component> selectedComponents = new();
	public Dictionary<int, int> componentQuantities = new();
	public bool loading = false;

	protected string ItemName => Item?.Name ?? "Cikk";

	protected override async Task OnInitializedAsync()
	{
		await LoadAvailableComponents();
	}

	private async Task LoadAvailableComponents()
	{
		loading = true;

		try
		{
			// Get all components
			var allComponents = await ComponentService.GetAllAsync(CompanyContext.ActiveCompanyId);

			// Get IDs of components already in this item
			var existingComponentIds = Item.ItemComponents.Select(ic => ic.ComponentId).ToHashSet();

			// Filter out components already in the item
			availableComponents = allComponents
			                      .Where(c => !existingComponentIds.Contains(c.Id))
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
			// Initialize quantities for all components (default to 1)
			foreach (var component in availableComponents)
			{
				componentQuantities[component.Id] = 1;
			}

			StateHasChanged();
		}
	}

	public void Cancel()
	{
		MudDialog.Cancel();
	}

	public async Task AddSelectedComponents()
	{
		if (!selectedComponents.Any())
		{
			Snackbar.Add("Válassz ki legalább egy komponenst", Severity.Warning);
			return;
		}

		loading = true;

		try
		{
			var addedCount = 0;

			foreach (var component in selectedComponents)
			{
				var quantity = componentQuantities.GetValueOrDefault(component.Id, 1);

				if (quantity > 0)
				{
					await ItemService.AddComponentToItemAsync(Item.Id, component.Id, quantity);
					addedCount++;
				}
			}

			Snackbar.Add($"{addedCount} komponens hozzáadva ehhez {Item.Name}", Severity.Success);
			MudDialog.Close(DialogResult.Ok(true));
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Hozzáadás közbeni hiba: {ex.Message}", Severity.Error);
		}
		finally
		{
			loading = false;
		}
	}
}