using System;
using System.Configuration;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;


namespace ConsoleServerWCF
{
    class Database
    {
        #region Instance of Logger and Database
        public Logger errornote = new Logger();
        private static readonly Database dbase = new Database();
        public static SqlConnection connection;
        #endregion
        #region Class for Database
        public static Database Dbase
        {
            get { return dbase; }
        }
        public static SqlConnection Connection
        {
            get { return connection; }
        }
        private Database()
        {
            connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
        }
        ///<summary>
        /// Select command 
        /// </summary>
        /// <param name="command">SELECT command!</param>
        /// <returns>Return the ID of table (i.e. an integer) </returns>
        public int SelectID(SqlCommand command)
        {
            DataTable datatable = new DataTable();
            command.Connection = connection;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(datatable);
                    command.Connection.Close();
                }
            }
            catch (Exception e)
            {
                if (command.Connection.State == ConnectionState.Open)
                {
                    errornote.Error("Error occurred while executing SELECT method " + DateTime.Now + e.Message);
                    command.Connection.Close();
                }
            }
            int a = 0;
            int.TryParse(datatable.Rows[0]["id"].ToString(), out a);
            return a;
        }
        /// <summary>
        /// Execute a SELECT command
        /// </summary>
        /// <param name="command">SELECT command!</param>
        /// <returns>The result is database table!</returns>
        public DataTable Select(SqlCommand command)
        {
            DataTable datatable = new DataTable();
            command.Connection = connection;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlDataAdapter sda = new SqlDataAdapter(command);
                sda.Fill(datatable);
                command.Connection.Close();
            }
            catch (Exception e)
            {
                errornote.Error("Error occurred while executing internal SELECT query!" + DateTime.Now + "Error code: " + e.Message);
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return datatable;
        }
        /// <summary>
        /// An UPDATE command has been executed!
        /// </summary>
        /// <param name="command">UPDATE command!</param>
        /// <returns>BOOL value depends on executing command was successuful or not!</returns>
        public bool Update(SqlCommand command)
        {
            int result = Execute(connection, command);
            if (result == 1)
            {
                errornote.Log("The UPDATE command has executed successfully!");
            }
            else if (result == -1)
            {
                errornote.Error("An ERROR occurred while executing UPDATE command!");
            }
            else
            {
                errornote.Error("Unexpected ERROR occurred. Please, contact with your system administrator!");
            }
            if (result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int Insert(SqlCommand command)
        {
            int result = Execute(connection, command);
            if (result == 1)
            {
                errornote.Log("The INSERT command has executed successful!" + "(" + command.CommandText + ") Time: " + DateTime.Now);
            }
            else if (result == -1)
            {
                errornote.Log("An ERROR occurred while executed INSERT command!" + "(" + command.CommandText + ") Time: " + DateTime.Now);
            }
            else
            {
                errornote.Log("Unexpected ERROR occurred! Please contact with your system administrator!" + "(" + command.CommandText + ") Time: " + DateTime.Now);
            }
            return result;
        }
        /// <summary>
        /// Method for executing INSERT and UPDATE queries
        /// </summary>
        /// <param name="connection">Database connection and an UPDATE or INSERT command!</param>
        /// <param name="command"> 1: successful, -1: unsuccessful</param>
        /// <returns></returns>
        public int Execute(SqlConnection connection, SqlCommand command)
        {
            command.Connection = connection;
            try
            {
                lock (typeof(Database))
                {
                    if (command.Connection.State != ConnectionState.Open)
                    {
                        command.Connection.Open();
                    }
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                    return -1;
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }
        /// <summary>
        /// Method for writing into connectiontable of database!
        /// </summary>
        /// <param name="userId">Provided Id by SelectID() of actual Id of user</param>
        /// <param name="castleId">Provided Id by SelectID of actual id of castle</param>
        /// <returns>BOOL value; true: successful, false: unsuccessful!</returns>
        public bool Buy_Castle(int userId, int castleId)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO OwnerCastle (OwnerId, CastleId) VALUES ('" + userId + "','" + castleId + "')";
            if (Insert(command) == 1)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Show us if SELECT command return value or not!!
        /// </summary>
        /// <param name="connection">Database connection!</param>
        /// <param name="command">SELECT command!</param>
        /// <returns>BOOL value; true: return value, false: not return value!</returns>
        private bool Exist_in_Database(SqlConnection connection, SqlCommand command)
        {
            DataSet ds = new DataSet();
            int rows = 0;
            command.Connection = connection;
            bool exist = false;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                lock (typeof(Database))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(ds);
                    command.Connection.Close();
                    rows = ds.Tables[0].Rows.Count;
                }
                if (rows > 0)
                {
                    exist = true;
                }
            }
            catch (Exception e)
            {
                errornote.Error("Error occurred while connecting to Exist_in_Database function! Time: " + DateTime.Now + "Error caused by: " + e.Message);
            }
            return exist;
        }
    }
    #endregion
    /// <summary>
    /// Check all of methods which not related to database engine directly but they used them!
    /// </summary>
    #region Checker class
    class Checker
    {
        Database database = Database.Dbase;
        SqlConnection connection = Database.Connection;
        /// <summary>
        /// Show us if name of castle has already used or not!
        /// </summary>
        /// <param name="n">Name of castle!</param>
        /// <returns>BOOL value: true: already used, false: not used!</returns>
        public bool CastleNameChecker(string n)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            return database.Exist_in_Database(connection, command);
        }
        /// <summary>
        /// Show us if username has already used or not!
        /// </summary>
        /// <param name="n">Username!</param>
        /// <returns>BOOL value; true: already used, false: not used!</returns>
        public bool UserNameChecker(string n)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Owners WHERE Owners.name ='" + n + "'";
            return database.Exist_in_Database(connection, command);
        }
        /// <summary>
        /// Show if username has already been used or not!
        /// </summary>
        /// <param name="n">Username returned by NevChecker()!</param>
        /// <param name="p">Character chain written into password field!</param>
        /// <returns>BOOL value; true: valid password, false: wrong password!</returns>
        public bool PasswordChecker(string n, string p)
        {
            DataTable datatable = new DataTable();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT name, password FROM Owners (name='" + n + "') AND (password='" + p + "')";
            return database.Exist_in_Database(connection, command);
        }
        /// <summary>
        /// Provide the castles of user! 
        /// </summary>
        /// <returns>STRING:Name of castles!</returns>
        public string ShowCastleName()
        {
            string name = Owners.Ownername;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT Castlename FROM Castle INNER JOIN OwnersCastles ON Castle.Id=OwnersCastles.Castle.Id INNER JOIN Owners ON OwnersCastles.Owners.Id=Owners.Id WHERE Owners.Name = '" + name + "'";

            DataTable datatable = database.Select(command);
            string Castlename = datatable.Rows[0]["Castlename"].ToString();
            return Castlename;
        }
        /// <summary>
        /// Show us if user has already had castles or not!
        /// </summary>
        /// <returns>BOOL value; true: user has castles, false: user doesn't has got castle!</returns>
        public bool Exist_CastleOfUser()
        {
            string owner = Owners.Ownername;
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Owners WHERE Owner.Name = '" + owner + "'";

            int result = database.SelectID(command);
            SqlCommand command2 = new SqlCommand();
            command2.CommandType = CommandType.Text;
            command2.CommandText = "SELECT * FROM OwnersCastles WHERE Owner.Id = " + result + "";

            if (database.Exist_in_Database(connection, command) != true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Are there castles in database?
        /// </summary>
        /// <returns>BOOL values; true: yes, false: no!</returns>
        public bool Exist_CastleInDatabase()
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Castle";
            return database.Exist_in_Database(connection, command);
        }
        /// <summary>
        /// To assign table to client using WCF, we need to help with Serialization! In practice, it is forced upon do it. In absence of it, an excption will be displayed!
        /// </summary>
        /// <param name="datatable">Datatable instance that we want to serialize and method text of TableName (unique tablename)</param>
        /// <returns>Serialized DataTable Instance</returns>
        public DataTable Serializer(DataTable datatable, string datatablename)
        {
            datatable.TableName = datatablename;
            new DataContractSerializer(typeof(DataTable)).WriteObject(new System.IO.MemoryStream(), datatable);
            return datatable;
        }
    }
    #endregion
}