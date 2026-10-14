using ProcurementPlanner.Components.Pages;
using ProcurementPlanner.Models;

namespace ProcurementPlanner.Services;

public class DeliveryPlanningService
{
	public List<OrderPlannerBase.SelectedItem> SelectedItems { get; set; } = new();
	public List<OrderPlannerBase.RequiredComponent> RequiredComponents { get; set; } = new();

	public Company? SelectedCompany { get; set; }
	public int? SelectedCompanyId { get; set; } = 1; // optional, useful for binding

	public DateTime? DateOfDelivery { get; set; } = DateTime.Now;

	public string DeliveryNumber { get; set; } = String.Empty;

	public string WeekOfDelivery { get; set; } = String.Empty;

	public void ClearPlanning()
	{
		SelectedItems.Clear();
		RequiredComponents.Clear();
		DateOfDelivery = DateTime.Now;
		DeliveryNumber = String.Empty;
		WeekOfDelivery = String.Empty;
	}
}