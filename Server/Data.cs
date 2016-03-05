using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server
{
    public static class DataBase
    {
        //static bool debug = false;
        private static DataSet data;
        private static string nameDatabase = "Shop";
        //private static SqlConnectionStringBuilder conn;

        public static DataColumn CreateColumn ( string nameColumn, Type type, bool readOnly,
            string Caption, bool allowNull, bool unique, bool autoIncr, int seed, int step )
        {
            DataColumn tmpColumn = new DataColumn(nameColumn, type);
            tmpColumn.ReadOnly = readOnly;
            tmpColumn.Caption = Caption;
            tmpColumn.AllowDBNull = allowNull;
            tmpColumn.Unique = unique;
            tmpColumn.AutoIncrement = autoIncr;
            if( autoIncr )
            {
                tmpColumn.AutoIncrementSeed = seed;
                tmpColumn.AutoIncrementStep = step;
            }
            return tmpColumn;
        }

        public static List <SqlDataAdapter> adapterList = new List<SqlDataAdapter>();

        public static DataSet LoadFromDataBase ()
        {
            string conn = GetConnectionString(); //это важно! при вызове из конструктора SqlDataAdapter стирается DataSet (
            SqlDataAdapter ad = new SqlDataAdapter("Use " +  nameDatabase + " SELECT Name FROM dbo.sysobjects  WHERE (xtype = 'U')",
               conn);
            DataSet dt = new DataSet();
            ad.Fill( dt );

            for( int i = 0; i < dt.Tables [ 0 ].Rows.Count; i++ )
            {
                string currName = dt.Tables [ 0 ].Rows [ i ].ItemArray [ 0 ].ToString();

                if( currName == "sysdiagrams" )
                    continue;

                SqlDataAdapter dAdapt = new SqlDataAdapter( string.Format("Select * from {0}", currName), conn );
                adapterList.Add( dAdapt );
                //SqlCommandBuilder dataCommand = new SqlCommandBuilder( dAdapt );
                dAdapt.Fill( data, currName );
            }

            return data;
        }

        public static DataTable LoadTableFromDataBase ( string tableNames )
        {

            SqlDataAdapter dAdapt = new SqlDataAdapter( "Select * from " + tableNames, GetConnectionString() );
            SqlCommandBuilder dataCommand = new SqlCommandBuilder( dAdapt );
            dAdapt.Fill( data, tableNames );

            return data.Tables [ 0 ];
        }
        public static void UpdateTableIntoDataBase ( DataTable table )
        {
            SqlDataAdapter dAdapt = new SqlDataAdapter( "Select * from " + table, GetConnectionString() );
            SqlCommandBuilder dataCommand = new SqlCommandBuilder( dAdapt );

            try 
            {
                dAdapt.Update( table );
            }
            catch( Exception /*e*/ )
            {
                //MessageBox.Show( "Не удалось обновить базу данных: " + e.Message );
            }
        }

        public static string GetConnectionString ()
        {
            data = new DataSet( nameDatabase );
            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
            conn.DataSource = "(local)\\SQLEXPRESS";
            conn.InitialCatalog = nameDatabase;
            conn.IntegratedSecurity = true;
            conn.ConnectTimeout = 15;
            conn.Encrypt = false;
            conn.TrustServerCertificate = false;
            conn.ApplicationIntent = ApplicationIntent.ReadWrite;
            conn.MultiSubnetFailover = false;

            return conn.ConnectionString;
        }

        public static DataTable SelectTable ( string selectCommand )
        {
            DataSet tmpDataSet = new DataSet();
            SqlDataAdapter dAdapt = new SqlDataAdapter( selectCommand, GetConnectionString() );
            SqlCommandBuilder dataCommand = new SqlCommandBuilder( dAdapt );
            dAdapt.Fill( tmpDataSet );

            return tmpDataSet.Tables [ 0 ];
        }
        public static DataRow [] SelectInTable(DataTable datatable, string slctString)
        {
            try {
                DataRow [] tmp = datatable.Select( slctString );
                if(tmp.Length == 0 /*&& debug*/)
                    return new DataRow[] { };
                //MessageBox.Show( "Не найдено данных в таблице " + datatable.TableName );

                return tmp;
            }
            catch(Exception /*e*/)
            {
                //MessageBox.Show( "Не удалось выбрать из таблицы: " + e.Message );
                return new DataRow [] { };
            }

            //return new DataRow[] { };
        }

    }
}
