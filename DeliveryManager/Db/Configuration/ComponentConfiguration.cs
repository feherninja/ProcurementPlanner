using DeliveryManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryManager.Db.Configuration;

public class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
	public void Configure(EntityTypeBuilder<Component> builder)
	{
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
		builder.Property(e => e.Unit).IsRequired().HasMaxLength(50);
		builder.Property(e => e.CostPerUnit).HasPrecision(18, 2);
		builder.HasIndex(e => e.Name);
	}
}