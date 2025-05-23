using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;


namespace Uniterm
{
    class DataBase
    {
        #region Variables

        string connectionString = "Server=localhost;Database=MASI;Trusted_Connection=True;";
        private SqlConnection conString;

        #endregion

        #region Builders and Finalizers

        public DataBase()
        {
            this.conString = new SqlConnection(this.connectionString);
        }

       /* public DataBase(string conStr)
        {
            this.conString = new SqlConnection(conStr);
        }*/

        #endregion

        #region Properties

        public SqlConnection ConnectionString
        {
            get
            {
                return this.conString;
            }
            set
            {
                this.conString = value;
            }
        }

        #endregion

        #region Methods

        public void Connect()
        {
            try
            {
                if (this.conString.State != ConnectionState.Open)
                {
                    this.conString.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd połączenia z bazą danych: " + ex.Message);
            }
        }

        public void Disconnect()
        {
            if (this.conString.State == ConnectionState.Open)
            {
                this.conString.Close();
            }
        }

        public void AddData(string sA, string sB, string sOp,
                            string eA, string eB, string eC,
                            string fontFamily, int fontSize, char oper,
                            string name, string description)
        {

            DeleteDataByName(name); // Usunięcie danych o tej samej nazwie przed dodaniem nowych



            string query = @"
        INSERT INTO Data 
        (sA, sB, sOp, eA, eB, eC, fontFamily, fontSize, oper, name, description) 
        VALUES 
        (@sA, @sB, @sOp, @eA, @eB, @eC, @fontFamily, @fontSize, @oper, @name, @description)";

            using (SqlCommand cmd = new SqlCommand(query, this.conString))
            {
                cmd.Parameters.AddWithValue("@sA", sA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@sB", sB ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@sOp", sOp ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@eA", eA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@eB", eB ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@eC", eC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@fontFamily", fontFamily ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@fontSize", fontSize);
                cmd.Parameters.AddWithValue("@oper", oper.ToString()); // char jako string
                cmd.Parameters.AddWithValue("@name", name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);

                Connect();
                cmd.ExecuteNonQuery();
                Disconnect();
            }
        }

        public DataTable GetData()
        {
            string query = "SELECT * FROM DATA";
            DataTable dataTable = new DataTable();

            using (SqlCommand cmd = new SqlCommand(query, this.conString))
            {
                Connect();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
                Disconnect();
            }

            return dataTable;
        }

        public DataTable GetDataByName(string name)
        {
            string query = "SELECT * FROM DATA WHERE name = @name";
            DataTable dataTable = new DataTable();

            using (SqlCommand cmd = new SqlCommand(query, this.conString))
            {
                cmd.Parameters.AddWithValue("@name", name);

                Connect();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
                Disconnect();
            }

            return dataTable;
        }

        public void DeleteDataByName(string name)
        {
            string query = "DELETE FROM Data WHERE name = @name";

            using (SqlCommand cmd = new SqlCommand(query, this.conString))
            {
                cmd.Parameters.AddWithValue("@name", name);

                Connect();
                cmd.ExecuteNonQuery();
                Disconnect();
            }
        }


        #endregion
    }
}
