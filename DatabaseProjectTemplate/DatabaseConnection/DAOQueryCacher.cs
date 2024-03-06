using DatabaseProjectTemplate.DataAccessObject;
using System.Reflection;
using System.Text;

namespace DatabaseProjectTemplate.DatabaseConnection
{
    public static class DAOQueryCacher
    {
        private static Dictionary<Type, string> _cachedInsertQueries = new Dictionary<Type, string>();
        private static Dictionary<Type, string> _cachedDeleteQueries = new Dictionary<Type, string>();
        private static Dictionary<Type, string> _cachedDefSelectQueries = new Dictionary<Type, string>();

        public static string GetInsertQuery(Type type, bool includeReturning = true, bool insertOrUpdate = false)
        {
            if (_cachedInsertQueries.TryGetValue(type, out string val))
            {
                return val;
            }
            StringBuilder strBuilder = new StringBuilder(512);

            string className = type.Name;
            IEnumerable<PropertyInfo?> allProperties = type.GetProperties().Where(n => n.MemberType is MemberTypes.Property && !Attribute.IsDefined(n, typeof(DatabaseIgnore)));

            if (!allProperties.Any())
            {
                throw new ArgumentException("Unable to generate query for class with 0 properties");
            }

            PropertyInfo? primaryKeyProperty = allProperties.FirstOrDefault(n => Attribute.IsDefined(n, typeof(DatabasePrimaryKey)));
            if (includeReturning && primaryKeyProperty is null)
            {
                throw new ArgumentException("Unable to generate query with returning statement for class with Undefined Primary Key Property");
            }

            strBuilder.Append($"INSERT{(insertOrUpdate ? " OR REPLACE " : " ")}INTO ");
            strBuilder.Append(className);
            strBuilder.Append('(');

            PropertyInfo? last = allProperties.Last();
            foreach (PropertyInfo? property in allProperties)
            {
                if (property == primaryKeyProperty) continue;

                strBuilder.Append(property.Name);
                if (property != last) strBuilder.Append(',');
            }

            strBuilder.Append(") VALUES(");
            foreach (PropertyInfo? property in allProperties)
            {
                if (property == primaryKeyProperty) continue;

                strBuilder.Append($"@{property.Name}");
                if (property != last) strBuilder.Append(',');
            }

            strBuilder.Append(')');

            if (includeReturning)
            {
                strBuilder.Append(" RETURNING ");
                strBuilder.Append(className);
                strBuilder.Append('.');
                strBuilder.Append(primaryKeyProperty.Name);
            }

            strBuilder.Append(';');

            string query = strBuilder.ToString();
            _cachedInsertQueries.Add(type, query);
            return query;
        }

        public static string GetInsertQuery<T>(bool includeReturning = true, bool insertOrUpdate = false)
        {
            return GetInsertQuery(typeof(T), includeReturning, insertOrUpdate);
        }

        public static string GetDeleteQuery(Type type)
        {
            if (_cachedDeleteQueries.TryGetValue(type, out string val))
            {
                return val;
            }

            StringBuilder strBuilder = new StringBuilder(512);

            string className = type.Name;

            PropertyInfo? primaryKeyProperty = type.GetProperties().Where(n => n.MemberType is MemberTypes.Property && Attribute.IsDefined(n, typeof(DatabasePrimaryKey))).FirstOrDefault();
            if (primaryKeyProperty is null)
            {
                throw new ArgumentException("Unable to generate delete query for class with Undefined Primary Key Property");
            }

            strBuilder.Append("DELETE FROM ");
            strBuilder.Append(className);
            strBuilder.Append(" WHERE ");
            strBuilder.Append(className);
            strBuilder.Append('.');
            strBuilder.Append(primaryKeyProperty.Name);
            strBuilder.Append(" = @ID");

            string query = strBuilder.ToString();
            _cachedDeleteQueries.Add(type, query);
            return query;
        }

        public static string GetDeleteQuery<T>()
        {
            return GetDeleteQuery(typeof(T));
        }

        public static string GetSelectQuery<T>(params string[] names)
        {
            return GetSelectQuery(typeof(T), names);
        }

        public static string GetSelectQuery(Type type, params string[] names)
        {
            if (_cachedDefSelectQueries.TryGetValue(type, out string val))
            {
                return val;
            }

            StringBuilder strBuilder = new StringBuilder(512);

            string className = type.Name;
            IEnumerable<PropertyInfo?> allProperties = type.GetProperties().Where(n => n.MemberType is MemberTypes.Property && !Attribute.IsDefined(n, typeof(DatabaseIgnore)));

            if (!allProperties.Any())
            {
                throw new ArgumentException("Unable to generate select query for class with 0 properties");
            }

            bool selectAll = names.Length == 0;
            if (names.Length == allProperties.Count())
            {
                selectAll = names.All(n => allProperties.Any(m => m?.Name == n));
            }

            strBuilder.Append("SELECT ");
            if (selectAll)
            {
                strBuilder.Append(" * ");
            }
            else
            {
                PropertyInfo? last = allProperties.Last();
                foreach (PropertyInfo? property in allProperties)
                {
                    strBuilder.Append(property.DeclaringType.Name);
                    strBuilder.Append('.');
                    strBuilder.Append(property.Name);
                    if (property != last)
                    {
                        strBuilder.Append(',');
                    }
                }
            }

            strBuilder.Append(" FROM ");
            strBuilder.Append(className);
            strBuilder.Append(';');

            string query = strBuilder.ToString();
            _cachedDefSelectQueries.Add(type, query);
            return query;
        }
    }
}