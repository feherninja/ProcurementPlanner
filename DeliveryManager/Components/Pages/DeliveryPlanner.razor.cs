using CommunityToolkit.Maui.Storage;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using DeliveryManager.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components.Pages;

public partial class DeliveryPlannerBase : ComponentBase
{
	[Inject]
	private IItemService ItemService { get; set; } = null!;

	[Inject]
	private ISnackbar Snackbar { get; set; } = null!;

	[Inject]
	private IDialogService DialogService { get; set; } = null!;

	[Inject]
	public DeliveryPlanningService PlanningService { get; set; } = null!;

	[Inject]
	public IComponentService ComponentService { get; set; } = null!;

	[Inject]
	public IDeliveryDocumentService DocumentService { get; set; } = null!;

	[Inject]
	public ICompanyService CompanyService { get; set; } = null!;

	[Inject]
	public IPurchaseOrderDocumentService PurchaseOrderDocumentService { get; set; } = null!;

	[Inject]
	public CompanyContext CompanyContext { get; set; } = null!;

	public List<Company> companies = new();

	public List<SelectedItem> selectedItems => PlanningService.SelectedItems;
	public List<RequiredComponent> requiredComponents => PlanningService.RequiredComponents;

	public DateTime? DateOfDelivery => PlanningService.DateOfDelivery;
	public string DeliveryNumber => PlanningService.DeliveryNumber;

	protected override async Task OnInitializedAsync()
	{
		companies = await CompanyService.GetAllAsync();

		CompanyContext.ActiveCompanyId = companies.FirstOrDefault(c => c.Id == CompanyContext.ActiveCompanyId)?.Id ?? 1;
		
		PlanningService.SelectedCompanyId = CompanyContext.ActiveCompanyId;
		PlanningService.SelectedCompany = companies.FirstOrDefault(c => c.Id == CompanyContext.ActiveCompanyId);
		PlanningService.ClearPlanning(); // important when switching company

		CalculateComponents();
	}

	public void OnCompanyChanged(int? companyId)
	{
		if (companyId is null) return;

		CompanyContext.ActiveCompanyId = companyId.Value;

		PlanningService.SelectedCompanyId = companyId;
		PlanningService.SelectedCompany = companies.FirstOrDefault(c => c.Id == companyId);

		PlanningService.ClearPlanning(); // prevents mixing items/components across companies
		CalculateComponents();
		StateHasChanged();
	}

	public async Task OpenAddItemDialog()
	{
		var parameters = new DialogParameters<ItemSelectionDialog>
		{
			{x => x.ExistingItems, selectedItems}
		};

		var options = new DialogOptions
		{
			CloseOnEscapeKey = true,
			MaxWidth = MaxWidth.Medium,
			FullWidth = true
		};

		var dialog = await DialogService.ShowAsync<ItemSelectionDialog>("Kiválasztott cikkek", parameters, options);
		var result = await dialog.Result;

		if (!result.Canceled && result.Data != null)
		{
			var itemsToAdd = (List<SelectedItem>) result.Data;
			selectedItems.AddRange(itemsToAdd);
			CalculateComponents();
			StateHasChanged();
		}
	}

	public void ResetDelivery()
	{
		PlanningService.ClearPlanning();
		CalculateComponents();
		StateHasChanged();
	}

	public async Task PrintDeliveryPlan()
	{
		var toOrder = PlanningService.RequiredComponents
		                             .Where(rc => rc.CalculateRequiredOrderQuantity() > 0)
		                             .ToList();

		if (toOrder.Count == 0)
		{
			Snackbar.Add("Nincs mit rendelni (minden komponensből van elég raktáron).", Severity.Info);
			return;
		}

		try
		{
			// var pdfBytes = DocumentService.GenerateDeliveryDocument(PlanningService);
			// var fileName = $@"Delivery_{DateTime.Now:yyyyMMdd-HH:mm:ss}.pdf";

			var pdfBytes = PurchaseOrderDocumentService.GeneratePurchaseOrderDocument(PlanningService);
			var fileName = $@"Order_{DateTime.Now:yyyyMMdd-HH:mm:ss}.pdf";

			using var stream = new MemoryStream(pdfBytes);
			var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);

			if (fileSaverResult.IsSuccessful)
			{
				Snackbar.Add($"PDF sikeresen mentve!", Severity.Success);
				await ApplyDelivery();
			}
			else
			{
				Snackbar.Add($"Mentés közbeni hiba: {fileSaverResult.Exception?.Message}", Severity.Error);
			}
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Generálás közbeni hiba: {ex.Message}", Severity.Error);
		}
	}

	private async Task ApplyDelivery()
	{
		await ComponentService.UpdateStockLevelsAsync(requiredComponents.ToList());
		ResetDelivery();
	}

	public void RemoveItem(SelectedItem selectedItem)
	{
		selectedItems.Remove(selectedItem);
		CalculateComponents();
		StateHasChanged();
	}

	public void CalculateComponents()
	{
		// Clear previous calculations
		requiredComponents.Clear();

		// Group all components by ComponentId and sum quantities
		var componentGroups = selectedItems
		                      .SelectMany(si => si.Item.ItemComponents.Select(ic => new
		                      {
			                      Component = ic.Component,
			                      TotalQuantity = ic.Quantity * si.Quantity
		                      }))
		                      .GroupBy(x => x.Component.Id)
		                      .Select(g => new RequiredComponent
		                      {
			                      Component = g.First().Component,
			                      TotalQuantity = g.Sum(x => (int) x.TotalQuantity)
		                      })
		                      .ToList();

		requiredComponents.AddRange(componentGroups);
		StateHasChanged();
	}

	// Helper classes
	public class SelectedItem
	{
		public Item Item { get; set; } = null!;
		public int Quantity { get; set; } = 1;
	}

	public class RequiredComponent
	{
		public Component Component { get; set; } = null!;
		public int TotalQuantity { get; set; }

		public int CalculateRequiredOrderQuantity()
		{
			if (Component.Stock - TotalQuantity >= 0)
				return 0;

			int result = (TotalQuantity - Component.Stock) / Component.OrderPackageSize + 1;
			Console.WriteLine(result);
			return result;
		}

		public int CalculateNewStock()
		{
			var calc = CalculateRequiredOrderQuantity();
			if (calc > 0)
				return Component.OrderPackageSize * calc + Component.Stock - TotalQuantity;

			return Component.Stock - TotalQuantity;
		}
	}
}