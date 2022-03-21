using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AK.DbSample.Database.Entities;

[Index(nameof(Name), IsUnique = true, Name = "Idx_Client_Name_Unique")]
public class Client
{
	[Key]
	public long Id { get; set; }
	public string Name { get; set; }
	
	public virtual ICollection<Invoice> Invoices { get; set; }
}