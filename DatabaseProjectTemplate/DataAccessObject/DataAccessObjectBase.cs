using DatabaseProjectTemplate.DatabaseConnection;
using System.Data.SqlClient;

namespace DatabaseProjectTemplate.DataAccessObject
{
    public abstract class DataAccessObjectBase : IDataAccessObject
    {
        public int ID { get; set; }

        public static string SelectAllQuery => throw new NotImplementedException();
        public static string InsertQuery => throw new NotImplementedException();
        public static string DeleteQuery => throw new NotImplementedException();

        public virtual void Delete()
        {
            SqlDatabaseConnection.Instance.ExecuteNonQuery(DeleteQuery, new SqlParameter("@ID", ID));
        }
        public virtual void Save()
        {
            ID = SqlDatabaseConnection.Instance.ExecuteScalarInt(InsertQuery);
            DataCacherService.AddOrUpdate(this);
        }
    }
}