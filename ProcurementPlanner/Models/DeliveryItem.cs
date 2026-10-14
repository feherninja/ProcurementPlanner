namespace ProcurementPlanner.Models;

public class DeliveryItem
{
	public int Id { get; set; }
	public int DeliveryId { get; set; }
	public int ItemId { get; set; }
	public int Quantity { get; set; }
    
	// Navigation properties
	public Delivery Delivery { get; set; } = null!;
	public Item Item { get; set; } = null!;
}