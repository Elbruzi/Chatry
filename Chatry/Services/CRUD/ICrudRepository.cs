namespace Chatry.Services.CRUD
{
    public interface ICrudRepository<T> where T : class
    {
        Task<Enum_Results> Async_ADD(T entity);
        Task<Enum_Results> User_is_Exists(T entity);

    }
}
