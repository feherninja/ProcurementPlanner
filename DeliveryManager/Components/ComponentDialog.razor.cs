using System.ComponentModel.DataAnnotations;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using DeliveryManager.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components;

public partial class ComponentDialogBase : ComponentBase
{
	[CascadingParameter]
	public IMudDialogInstance MudDialog { get; set; } = null!;

	[Parameter]
	public Component? Component { get; set; }

	[Inject]
	private IComponentService ComponentService { get; set; } = null!;

	[Inject]
	private ISnackbar Snackbar { get; set; } = null!;

	[Inject]
	public CompanyContext CompanyContext { get; set; } = null!;

	public MudForm form = null!;
	public ComponentModel model = new();

	protected bool IsEditMode => Component != null;

	protected override void OnInitialized()
	{
		if (Component != null)
		{
			// Edit mode - populate from existing component
			model = new ComponentModel
			{
				Name = Component.Name,
				Description = Component.Description,
				Stock = Component.Stock,
				OrderPackageSize = Component.OrderPackageSize
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
			Snackbar.Add("Validációs hibák jelentkeztek", Severity.Error);
			return;
		}

		try
		{
			if (IsEditMode && Component != null)
			{
				// Update existing
				Component.Name = model.Name;
				Component.Description = model.Description;
				Component.Stock = model.Stock;
				Component.OrderPackageSize = model.OrderPackageSize;
				Component.PartNumber = model.PartNumber;
				Component.DrawingNumber = model.DrawingNumber;
				Component.CompanyId = CompanyContext.ActiveCompanyId;

				await ComponentService.UpdateAsync(Component);
				Snackbar.Add("Komponens sikeresen mentve", Severity.Success);
			}
			else
			{
				// Create new
				var newComponent = new Component
				{
					Name = model.Name,
					Description = model.Description,
					Stock = model.Stock,
					OrderPackageSize = model.OrderPackageSize,
					PartNumber = model.PartNumber,
					DrawingNumber = model.DrawingNumber,
					CompanyId = CompanyContext.ActiveCompanyId
				};

				await ComponentService.CreateAsync(newComponent);
				Snackbar.Add("Komponens sikeresen létrehozva", Severity.Success);
			}

			MudDialog.Close(DialogResult.Ok(true));
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Mentési hiba: {ex.Message}", Severity.Error);
		}
	}

	// Form model for binding and validation
	public class ComponentModel
	{
		[Required(ErrorMessage = "Név kötelező")]
		[StringLength(200, ErrorMessage = "Név nem lehet 200 karakternél hosszabb")]
		public string Name { get; set; } = string.Empty;

		[StringLength(500, ErrorMessage = "Leírás nem lehet 500 karakternél hosszabb")]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Raktármennyiség megadása kötelező")]
		public int Stock { get; set; }

		[Required(ErrorMessage = "Rendelési kötegszám megadása kötelező")]
		public int OrderPackageSize { get; set; }

		[Required(ErrorMessage = "Cikkszám megadása kötelező")]
		public string? PartNumber { get; set; }

		[Required(ErrorMessage = "Rajzszám megadása kötelező")]
		public string? DrawingNumber { get; set; }
	}
}