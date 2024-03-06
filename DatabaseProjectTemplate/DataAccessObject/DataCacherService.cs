using DatabaseProjectTemplate.DatabaseConnection;
using System.Data;

namespace DatabaseProjectTemplate.DataAccessObject
{
    public static class DataCacherService
    {
        private static readonly Dictionary<Type, Dictionary<int, object>> _cache = new Dictionary<Type, Dictionary<int, object>>();

        public static IEnumerable<T> GetAll<T>() where T : DataAccessObjectBase
        {
            Type type = typeof(T);

            Dictionary<int, object> dictionary = _cache[type];
            dictionary.Clear();

            foreach (DataRow row in SqlDatabaseConnection.Instance.ExecuteQuery(DAOQueryCacher.GetSelectQuery<T>()).Rows)
            {
                T? obj = IDataAccessObject.LoadFromRow<T>(row);
                dictionary.Add(obj.ID, obj);
                yield return obj;
            }
        }

        public static bool AddOrUpdate<T>(T item) where T : DataAccessObjectBase
        {
            Type type = typeof(T);

            Dictionary<int, object> dictionary = _cache[type];

            if (dictionary.ContainsKey(item.ID))
            {
                dictionary[item.ID] = item;
            }
            else
            {
                dictionary.Add(item.ID, item);
            }

            return true;
        }

        public static bool Remove<T>(T item) where T : DataAccessObjectBase
        {
            Type type = typeof(T);

            Dictionary<int, object> dictionary = _cache[type];

            return dictionary.Remove(item.ID);
        }

        public static IEnumerable<T> GetAllBy<T>(Predicate<T> predicate) where T : DataAccessObjectBase
        {
            return GetAll<T>().Where(n => predicate(n));
        }

        public static T? GetFirstBy<T>(Predicate<T> predicate) where T : DataAccessObjectBase
        {
            return GetAll<T>().FirstOrDefault(n => predicate(n));
        }
    }
}