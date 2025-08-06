namespace DeliveryManager.Models;

public class ItemComponent
{
	public int Id { get; set; }
	public int ItemId { get; set; }
	public int ComponentId { get; set; }
	public int Quantity { get; set; }
    
	// Navigation properties
	public Item Item { get; set; } = null!;
	public Component Component { get; set; } = null!;
}