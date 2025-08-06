using DeliveryManager.Components.Pages;

namespace DeliveryManager.Services;

public class DeliveryPlanningService
{
	public List<DeliveryPlannerBase.SelectedItem> SelectedItems { get; set; } = new();
	public List<DeliveryPlannerBase.RequiredComponent> RequiredComponents { get; set; } = new();

	public DateTime? DateOfDelivery { get; set; } = DateTime.Now;

	public string DeliveryNumber { get; set; } = String.Empty;

	public string WeekOfDelivery { get; set; } = String.Empty;

	public void ClearPlanning()
	{
		SelectedItems.Clear();
		RequiredComponents.Clear();
		DateOfDelivery = null;
		DeliveryNumber = String.Empty;
		WeekOfDelivery = String.Empty;
	}
}