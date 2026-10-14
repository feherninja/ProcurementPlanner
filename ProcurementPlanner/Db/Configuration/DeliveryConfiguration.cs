using ProcurementPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProcurementPlanner.Db.Configuration;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
	public void Configure(EntityTypeBuilder<Delivery> builder)
	{
		builder.HasKey(e => e.Id);
		builder.Property(e => e.CustomerName).HasMaxLength(200);
		builder.Property(e => e.CustomerAddress).HasMaxLength(500);
		builder.Property(e => e.Status).HasConversion<string>();
		builder.HasIndex(e => e.DeliveryDate);
	}
}