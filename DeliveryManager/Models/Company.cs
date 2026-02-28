namespace DeliveryManager.Models;

public class Company
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string ShortName { get; set; }
	public string Address { get; set; }
	public string Email { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}