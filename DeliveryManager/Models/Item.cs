namespace DeliveryManager.Models;

public class Item
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }
    
	// Navigation properties
	public ICollection<ItemComponent> ItemComponents { get; set; } = new List<ItemComponent>();
	public ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
    
	// Calculated properties
	public decimal TotalCost => ItemComponents.Sum(ic => ic.Quantity * ic.Component.CostPerUnit);
}