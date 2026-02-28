using DeliveryManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryManager.Db.Configuration;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
	public void Configure(EntityTypeBuilder<Company> builder)
	{
		builder.HasKey(e => e.Id);

		builder.Property(e => e.Name)
		       .IsRequired()
		       .HasMaxLength(200);

		builder.Property(e => e.ShortName)
		       .HasMaxLength(50);

		builder.Property(e => e.Email)
		       .HasMaxLength(200);

		builder.Property(e => e.CreatedAt)
		       .HasDefaultValueSql("CURRENT_TIMESTAMP"); // SQLite

		builder.HasData(
		                new Company
		                {
			                Id = 1,
			                Name = "Aspöck Systems GmbH",
			                ShortName = "Aspöck",
			                Address = "4722 Peuerbach, Enzing 4",
			                Email = "order@aspoeck.com",
			                CreatedAt = new DateTime(2026, 1, 3, 20, 39, 53, 810, DateTimeKind.Utc)
		                });
	}
}