using DeliveryManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryManager.Db.Configuration;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
	public void Configure(EntityTypeBuilder<Item> builder)
	{
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
		builder.HasIndex(e => e.Name);
	}
}