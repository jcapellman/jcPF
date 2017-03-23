using System.Threading.Tasks;

namespace jcPF.WPF.DAL
{
    public class BaseDAL<T>
    {
        public BaseDAL()
        {
            using (var dbFactory = new SQLite.SQLiteConnection(Common.Constants.SQLITE_FILENAME))
            {
                dbFactory.CreateTable<T>();
            }
        }

        protected async Task<bool> WriteAsync(T obj)
        {
            var dbFactory = new SQLite.SQLiteAsyncConnection(Common.Constants.SQLITE_FILENAME);

            return await dbFactory.InsertAsync(obj) > 0;
        }
    }
}