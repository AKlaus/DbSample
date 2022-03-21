using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AK.DbSample.Database.Entities;

public class Invoice
{
	[Key, Column(TypeName = "nvarchar(100)")]
	public string Number { get; set; } = null!;

	public long ClientId { get; set; }
	[Column(TypeName = "datetime")]
	public DateOnly Date { get; set; }
	
	[Column(TypeName = "decimal(18,2)")]
	public decimal Amount { get; set; }
	
	public virtual Client Client { get; set; } = null!;
}