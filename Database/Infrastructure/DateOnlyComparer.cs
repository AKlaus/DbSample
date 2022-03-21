using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AK.DbSample.Database.Infrastructure;

/// <summary>
///		Compares <see cref="DateOnly" />.
/// </summary>
public class DateOnlyComparer : ValueComparer<DateOnly>
{
	/// <summary>
	///		Creates a new instance of this converter.
	/// </summary>
	public DateOnlyComparer() : base(
		(d1, d2) => d1 == d2 && d1.DayNumber == d2.DayNumber, 
		d => d.GetHashCode()) {}
}

/// <summary>
///		Compares <see cref="DateOnly?" />.
/// </summary>
public class NullableDateOnlyComparer : ValueComparer<DateOnly?>
{
	/// <summary>
	/// Creates a new instance of this converter.
	/// </summary>
	public NullableDateOnlyComparer() : base(
		(d1, d2) => d1 == d2 && d1.GetValueOrDefault().DayNumber == d2.GetValueOrDefault().DayNumber,
		d => d.GetHashCode()) {}
}