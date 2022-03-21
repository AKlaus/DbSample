using AK.DbSample.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace AK.DbSample.Database;

public class DataContext : DbContext
{
	public DataContext() {}

	public DataContext(DbContextOptions<DataContext> options) : base(options) {}
	
	public virtual DbSet<Client> Clients { get; set; }
	public virtual DbSet<Invoice> Invoices { get; set; }
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Invoice>()
			.HasOne(p => p.Client)
			.WithMany(b => b.Invoices)
			.OnDelete(DeleteBehavior.Cascade);
	}
}