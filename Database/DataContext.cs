using AK.DbSample.Database.Entities;
using AK.DbSample.Database.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AK.DbSample.Database;

public class DataContext : DbContext
{
	public DataContext() {}

	public DataContext(DbContextOptions<DataContext> options) : base(options) {}
	
	public virtual DbSet<Client> Clients { get; set; } = null!;
	public virtual DbSet<Invoice> Invoices { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Invoice>(builder =>
			{
				// Date is a DateOnly property and date on database
				builder.Property(x => x.Date)
					.HasConversion<DateOnlyConverter, DateOnlyComparer>();

				// Set cascade delete
				builder.HasOne(p => p.Client)
					.WithMany(b => b.Invoices)
					.OnDelete(DeleteBehavior.Cascade);
			});
	}
}