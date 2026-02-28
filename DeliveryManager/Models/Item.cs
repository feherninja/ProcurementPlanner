namespace DeliveryManager.Models;

public class Item
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }

	// cikkszám
	public string PartNumber { get; set; } = string.Empty;

	// rajzszám
	public string DrawingNumber { get; set; } = string.Empty;

	public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }

	public int CompanyId { get; set; }
	public Company Company { get; set; } = null!;
	
	// Navigation properties
	public ICollection<ItemComponent> ItemComponents { get; set; } = new List<ItemComponent>();
	public ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
}