using System.ComponentModel.DataAnnotations;
using DeliveryManager.Models;
using DeliveryManager.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components;

public partial class ItemDialogBase : ComponentBase
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public Item? Item { get; set; }
    [Inject] private IItemService ItemService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    public MudForm form = null!;
    public ItemModel model = new();

    protected bool IsEditMode => Item != null;

    protected override void OnInitialized()
    {
        if (Item != null)
        {
            // Edit mode - populate from existing item
            model = new ItemModel
            {
                Name = Item.Name,
                Description = Item.Description
            };
        }
    }

    public void Cancel()
    {
        MudDialog.Cancel();
    }

    public async Task Save()
    {
        await form.Validate();
        
        if (!form.IsValid)
        {
            Snackbar.Add("Please fix validation errors", Severity.Error);
            return;
        }

        try
        {
            if (IsEditMode && Item != null)
            {
                // Update existing
                Item.Name = model.Name;
                Item.Description = model.Description;
                
                await ItemService.UpdateAsync(Item);
                Snackbar.Add("Item updated successfully", Severity.Success);
            }
            else
            {
                // Create new
                var newItem = new Item
                {
                    Name = model.Name,
                    Description = model.Description
                };
                
                await ItemService.CreateAsync(newItem);
                Snackbar.Add("Item created successfully", Severity.Success);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving item: {ex.Message}", Severity.Error);
        }
    }

    // Form model for binding and validation
    public class ItemModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}