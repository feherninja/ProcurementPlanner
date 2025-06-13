using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components;

public partial class ComponentDialogBase : ComponentBase
{
	[CascadingParameter]
	public IMudDialogInstance MudDialog { get; set; } = null!;
	[Parameter] public Component? Component { get; set; }

	[Inject]
	private IComponentService ComponentService { get; set; } = null!;

	[Inject]
	private ISnackbar Snackbar { get; set; } = null!;

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
				Unit = Component.Unit,
				CostPerUnit = Component.CostPerUnit
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
			if (IsEditMode && Component != null)
			{
				// Update existing
				Component.Name = model.Name;
				Component.Description = model.Description;
				Component.Unit = model.Unit;
				Component.CostPerUnit = model.CostPerUnit;

				await ComponentService.UpdateAsync(Component);
				Snackbar.Add("Component updated successfully", Severity.Success);
			}
			else
			{
				// Create new
				var newComponent = new Component
				{
					Name = model.Name,
					Description = model.Description,
					Unit = model.Unit,
					CostPerUnit = model.CostPerUnit
				};

				await ComponentService.CreateAsync(newComponent);
				Snackbar.Add("Component created successfully", Severity.Success);
			}

			MudDialog.Close(DialogResult.Ok(true));
		}
		catch (Exception ex)
		{
			Snackbar.Add($"Error saving component: {ex.Message}", Severity.Error);
		}
	}

	// Form model for binding and validation
	public class ComponentModel
	{
		[Required(ErrorMessage = "Name is required")]
		[StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
		public string Name { get; set; } = string.Empty;

		[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Unit is required")]
		[StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
		public string Unit { get; set; } = string.Empty;

		[Required(ErrorMessage = "Cost per unit is required")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0")]
		public decimal CostPerUnit { get; set; }
	}
}