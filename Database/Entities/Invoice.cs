using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
#pragma warning disable CS8618

namespace AK.DbSample.Database.Entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Invoice
{
	[Key, Column(TypeName = "nvarchar(100)")]
	public string Number { get; set; }

	public long ClientId { get; set; }
	[Column(TypeName = "datetime")]
	public DateOnly Date { get; set; }
	
	[Column(TypeName = "decimal(18,2)")]
	public decimal Amount { get; set; }
	
	public virtual Client Client { get; set; }
}