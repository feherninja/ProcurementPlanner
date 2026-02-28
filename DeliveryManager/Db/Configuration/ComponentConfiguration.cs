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
		builder.Property(e => e.Stock).IsRequired();
		builder.Property(e => e.OrderPackageSize).IsRequired();
		builder.HasIndex(e => e.Name);
		
		builder.Property(x => x.CompanyId)
		       .HasDefaultValue(1)
		       .IsRequired();
		
		builder.HasOne(c => c.Company)
		       .WithMany()
		       .HasForeignKey(c => c.CompanyId)
		       .OnDelete(DeleteBehavior.Restrict);
		
		
		
	}
}