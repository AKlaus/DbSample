using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AK.DbSample.Database.Entities;

[Index(nameof(Name), IsUnique = true, Name = "Idx_Client_Name_Unique")]
public class Client
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }
	
	[Column(TypeName = "nvarchar(500)")]
	public string Name { get; set; } = null!;

	public virtual ICollection<Invoice> Invoices { get; set; } = null!;
}