using AK.DbSample.Database;

namespace AK.DbSample.Domain.Services;

public abstract class BaseService
{
	protected readonly DataContext DataContext;

	protected BaseService(DataContext dataContext)
	{
		DataContext = dataContext;
	}
}