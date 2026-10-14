namespace DeliveryManager.Models;

public class Component
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }

	public string PartNumber { get; set; } = string.Empty;    // termékszám
	public string ExternalNumber { get; set; } = string.Empty; // külső alkatrészszám
	
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }
	
	public int Stock { get; set; }
	public int OrderPackageSize { get; set; }
	public decimal Price { get; set; }
	
	public int CompanyId { get; set; }
	public Company Company { get; set; } = null!;
    
	// Navigation properties
	public ICollection<ItemComponent> ItemComponents { get; set; } = new List<ItemComponent>();
}