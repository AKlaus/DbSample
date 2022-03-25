using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
#pragma warning disable CS8618

namespace AK.DbSample.Database.Entities;

[Index(nameof(Name), IsUnique = true, Name = "IX_Client_Name_Unique")]
public class Client
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }
	
	[Column(TypeName = "nvarchar(100)")]
	public string Name { get; set; }

	public virtual ICollection<Invoice> Invoices { get; set; }
}