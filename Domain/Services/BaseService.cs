using AK.DbSample.Database;

namespace AK.DbSample.Domain.Services;

public abstract class BaseService(DataContext dataContext)
{
	protected readonly DataContext DataContext = dataContext;
}