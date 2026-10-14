using ProcurementPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProcurementPlanner.Db.Configuration;

public class DeliveryItemConfiguration : IEntityTypeConfiguration<DeliveryItem>
{
	public void Configure(EntityTypeBuilder<DeliveryItem> builder)
	{
		builder.HasKey(e => e.Id);
        
		builder.HasOne(e => e.Delivery)
		       .WithMany(d => d.DeliveryItems)
		       .HasForeignKey(e => e.DeliveryId)
		       .OnDelete(DeleteBehavior.Cascade);
               
		builder.HasOne(e => e.Item)
		       .WithMany(i => i.DeliveryItems)
		       .HasForeignKey(e => e.ItemId)
		       .OnDelete(DeleteBehavior.Restrict);

		// Prevent duplicate Delivery-Item combinations
		builder.HasIndex(e => new { e.DeliveryId, e.ItemId }).IsUnique();
	}
}