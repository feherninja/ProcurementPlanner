namespace DeliveryManager.Models;

public class Delivery
{
	public int Id { get; set; }
	public string? CustomerName { get; set; }
	public string? CustomerAddress { get; set; }
	public DateTime DeliveryDate { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }
	public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
	public string? Notes { get; set; }

	// Navigation properties
	public ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();

	// Calculated properties
	//public decimal TotalCost => DeliveryItems.Sum(di => di.Quantity * di.Item.TotalCost);
}