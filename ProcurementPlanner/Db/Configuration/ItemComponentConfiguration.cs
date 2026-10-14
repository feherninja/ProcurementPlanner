using ProcurementPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProcurementPlanner.Db.Configuration;

public class ItemComponentConfiguration : IEntityTypeConfiguration<ItemComponent>
{
	public void Configure(EntityTypeBuilder<ItemComponent> builder)
	{
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Quantity).HasPrecision(18, 3);
        
		builder.HasOne(e => e.Item)
		       .WithMany(i => i.ItemComponents)
		       .HasForeignKey(e => e.ItemId)
		       .OnDelete(DeleteBehavior.Cascade);
               
		builder.HasOne(e => e.Component)
		       .WithMany(c => c.ItemComponents)
		       .HasForeignKey(e => e.ComponentId)
		       .OnDelete(DeleteBehavior.Cascade);

		// Prevent duplicate Item-Component combinations
		builder.HasIndex(e => new { e.ItemId, e.ComponentId }).IsUnique();
	}
}