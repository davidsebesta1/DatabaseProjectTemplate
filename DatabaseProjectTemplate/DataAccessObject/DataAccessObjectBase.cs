using DatabaseProjectTemplate.DatabaseConnection;
using System.Data.SqlClient;

namespace DatabaseProjectTemplate.DataAccessObject
{
    public abstract class DataAccessObjectBase : IDataAccessObject
    {
        public int ID { get; set; }

        public virtual void Delete()
        {
            SqlDatabaseConnection.Instance.ExecuteNonQuery(DAOQueryCacher.GetDeleteQuery(this.GetType()), new SqlParameter("@ID", ID));
        }
        public virtual void Save()
        {
            ID = SqlDatabaseConnection.Instance.ExecuteScalarInt(DAOQueryCacher.GetInsertQuery(this.GetType()));
            DataCacherService.AddOrUpdate(this);
        }
    }
}