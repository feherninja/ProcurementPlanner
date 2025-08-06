using Microsoft.AspNetCore.Components;
using MudBlazor;
using DeliveryManager.Models;
using DeliveryManager.Services;

namespace DeliveryManager.Components.Pages;

public partial class ItemsBase : ComponentBase
{
    [Inject] private IItemService ItemService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    public MudDataGrid<Item> dataGrid = null!;
    public List<Item> items = new();
    public bool loading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadItems();
    }

    public async Task LoadItems()
    {
        loading = true;
        try
        {
            items = await ItemService.GetAllAsync();
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

    public async Task OpenCreateDialog()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<ItemDialog>("Cikk létrehozása", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
            await LoadItems(); // Refresh the grid
        
    }

    public async Task OpenAddComponentDialog(Item item)
    {
        var parameters = new DialogParameters<ComponentSelectionDialog>
        {
            { x => x.Item, item }
        };
        
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Medium,
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
            $"'{component.Name}' komponens eltávolítása '{item.Name}' cikkből?",
            yesText: "Törlés", 
            cancelText: "Mégse");

        if (confirmed == true)
        {
            try
            {
                await ItemService.RemoveComponentFromItemAsync(item.Id, component.Id);
                Snackbar.Add($"'{component.Name}' komponens eltávolítva '{item.Name}' cikkből", Severity.Success);
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
            { x => x.Item, item }
        };
    
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<ItemDialog>("Cikk szerkesztése", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
            await LoadItems(); // Refresh the grid
        
    }

    public async Task OpenDeleteDialog(Item item)
    {
        var confirmed = await DialogService.ShowMessageBox(
            "Cikk törlése",
            $"Biztosan törölni szeretnéd ezt a cikket: '{item.Name}'?",
            yesText: "Törlés", 
            cancelText: "Mégse");

        if (confirmed == true)
        {
            try
            {
                await ItemService.DeleteAsync(item.Id);
                Snackbar.Add($"Cikk '{item.Name}' sikeresen törölve", Severity.Success);
                await LoadItems();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Törlési hiba: {ex.Message}", Severity.Error);
            }
        }
    }
}