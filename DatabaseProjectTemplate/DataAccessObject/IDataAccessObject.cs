
using System.Data;
using System.Reflection;

namespace DatabaseProjectTemplate.DataAccessObject
{
    public interface IDataAccessObject
    {
        public abstract void Save();
        public abstract void Delete();

        public static T? LoadFromRow<T>(DataRow row) where T : IDataAccessObject
        {
            IEnumerable<PropertyInfo?> allProperties = typeof(T).GetProperties().Where(n => n.MemberType is MemberTypes.Property && !Attribute.IsDefined(n, typeof(DatabaseIgnore)));

            if (!allProperties.Any())
            {
                throw new ArgumentException("Unable to load into empty object");
            }

            T? obj = (T)Activator.CreateInstance(typeof(T));
            foreach (PropertyInfo property in allProperties)
            {
                object? val = row[property.Name];
                if (val != null)
                {
                    property.SetValue(obj, val);
                }
            }

            return obj;
        }
    }
}