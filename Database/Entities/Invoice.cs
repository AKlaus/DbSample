using System.ComponentModel.DataAnnotations;

namespace AK.DbSample.Database.Entities;

public class Invoice
{
	[Key]
	public string Number { get; set; }
	public long ClientId { get; set; }
	public DateOnly Date { get; set; }
	public decimal Amount { get; set; }
	
	public virtual Client Client { get; set; }
}