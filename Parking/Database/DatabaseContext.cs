using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;

namespace Parking.Database
{
    public class DatabaseContext
    {
        private SqlConnection _conexao;

        public DatabaseContext() 
        {
            var connStr = ConfigurationManager.ConnectionStrings["DBContextParking"].ConnectionString;
            _conexao = new SqlConnection(connStr);
        }


        public IList<T> ListarSql<T>(string sql)
        {
            _conexao.Open();

            SqlCommand _sql = new SqlCommand(sql, _conexao);

            SqlDataReader reader = _sql.ExecuteReader();

            List<T> list = new List<T>();
            T obj = default(T);

            while (reader.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(reader[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, reader[prop.Name], null);
                    }
                }
                list.Add(obj);
            }

            _conexao.Close();

            return list;
        }

        public int ExecuteSql(string sql)
        {
            _conexao.Open();

            SqlCommand _sql = new SqlCommand(sql, _conexao);

            var retorno = _sql.ExecuteNonQuery();

            _conexao.Close();

            return retorno;
        }

    }
}