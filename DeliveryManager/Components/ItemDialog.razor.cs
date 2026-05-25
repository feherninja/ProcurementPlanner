using System.ComponentModel.DataAnnotations;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using DeliveryManager.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components;

public partial class ItemDialogBase : ComponentBase
{
	[CascadingParameter]
	public IMudDialogInstance MudDialog { get; set; } = null!;

	[Parameter]
	public Item? Item { get; set; }

	[Inject]
	private IItemService ItemService { get; set; } = null!;

	[Inject]
	private ISnackbar Snackbar { get; set; } = null!;

	[Inject]
	private CompanyContext CompanyContext { get; set; } = null!;

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
				Description = Item.Description,
				PartNumber = Item.PartNumber,
				DrawingNumber = Item.DrawingNumber,
				Price = Item.Price
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
			Snackbar.Add("Validációs hibák léptek fel", Severity.Error);
			return;
		}

		try
		{
			if (IsEditMode && Item != null)
			{
				// Update existing
				Item.Name = model.Name;
				Item.Description = model.Description;
				Item.PartNumber = model.PartNumber;
				Item.DrawingNumber = model.DrawingNumber;
				Item.Price = model.Price;
				Item.CompanyId = CompanyContext.ActiveCompanyId;

				await ItemService.UpdateAsync(Item);
				Snackbar.Add("Termék sikeresen mentve", Severity.Success);
			}
			else
			{
				// Create new
				var newItem = new Item
				{
					Name = model.Name,
					Description = model.Description,
					PartNumber = model.PartNumber,
					DrawingNumber = model.DrawingNumber,
					Price = model.Price,
					CompanyId = CompanyContext.ActiveCompanyId
				};

				await ItemService.CreateAsync(newItem);
				Snackbar.Add("Termék sikeresen létrehozva", Severity.Success);
			}

			MudDialog.Close(DialogResult.Ok(true));
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Mentés közbeni hiba: {ex.Message}", Severity.Error);
		}
	}

	// Form model for binding and validation
	public class ItemModel
	{
		[Required(ErrorMessage = "Név megadása kötelezo")]
		[StringLength(200, ErrorMessage = "Név nem lehet 200 karakternél hosszabb")]
		public string Name { get; set; } = string.Empty;

		[StringLength(500, ErrorMessage = "leírás nem lehet 500 karakternél hosszabb")]
		public string? Description { get; set; }

		public string? PartNumber { get; set; }

		public string? DrawingNumber { get; set; }

		public int Price { get; set; }
	}
}