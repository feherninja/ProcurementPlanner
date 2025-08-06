namespace DeliveryManager.Models;

public class Component
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	//public string Unit { get; set; } = string.Empty; // e.g., "kg", "pieces", "meters"
	//public decimal CostPerUnit { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }
	
	public int Stock { get; set; }
	public int OrderPackageSize { get; set; }
	
    
	// Navigation properties
	public ICollection<ItemComponent> ItemComponents { get; set; } = new List<ItemComponent>();
}