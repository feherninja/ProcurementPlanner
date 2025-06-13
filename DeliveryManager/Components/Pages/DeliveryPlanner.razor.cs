using Microsoft.AspNetCore.Components;
using MudBlazor;
using DeliveryManager.Models;
using DeliveryManager.Services;

namespace DeliveryManager.Components.Pages;

public partial class DeliveryPlannerBase : ComponentBase
{
    [Inject] private IItemService ItemService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    public List<SelectedItem> selectedItems = new();
    public List<RequiredComponent> requiredComponents = new();

    protected override void OnInitialized()
    {
        CalculateComponents();
    }

    public async Task OpenAddItemDialog()
    {
        var parameters = new DialogParameters<ItemSelectionDialog>
        {
            { x => x.ExistingItems, selectedItems }
        };
        
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<ItemSelectionDialog>("Select Items", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled && result.Data != null)
        {
            var itemsToAdd = (List<SelectedItem>)result.Data;
            selectedItems.AddRange(itemsToAdd);
            CalculateComponents();
            StateHasChanged();
        }
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
                TotalQuantity = g.Sum(x => x.TotalQuantity)
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
        public decimal TotalQuantity { get; set; }
    }
}