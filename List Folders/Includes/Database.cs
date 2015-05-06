using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ListFolders.Includes {
  public class Database {
    
    string options_table="options";
    string config_table="config";
    string table;
    string sql;
    string res;

    bool connected;

    private MySqlConnection conn;
    private string server;
    private string database;
    private string uid;
    private string password;

    public Database() {
      if (conn == null)
        conn = createConnection();
      connected=OpenConnection();
    }

    private MySqlConnection createConnection() {
      server = "localhost";
      database = "list_folders";
      uid = "root";
      password = "";
      string connectionString;
      connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

      conn = new MySqlConnection(connectionString);

      return conn;
    }

    private bool OpenConnection() {
      try {
        conn.Open();
        return true;
      }
      catch (MySqlException ex) {
        return false;
      }
    }

    //Close conn
    public bool CloseConnection() {
      try {
        conn.Close();
        return true;
      }
      catch (MySqlException ex) {
        return false;
      }
    }
    
    public bool Exists(string table, string name) {
      sql = "select count(*) from " + table + " where name=@name";
      int count=0;
      if (!connected) return false;

      MySqlCommand cmd = new MySqlCommand(sql, conn);
      cmd.Prepare();
      cmd.Parameters.AddWithValue("@name", name);
      count = Convert.ToInt32(cmd.ExecuteScalar());
      
      if(count==0) return false;
      return true;
    }
    
    /*
     * General update method
     */
    public void updateOption(string name, string value, string dbtable){
      if (!connected) return;

      bool res;
      table=options_table;
      if(dbtable.Length!=0) table=dbtable;
      
      res=Exists(table, name);
      
      sql="update "+table+" set value=@value where name=@name";
      if(!res){
        sql="insert into "+table+" (name,value) values(@name, @value)";
      }
      
      MySqlCommand cmd = new MySqlCommand(sql, conn);
      
      cmd.Prepare();
      cmd.Parameters.AddWithValue("@name", name);
      cmd.Parameters.AddWithValue("@value", value);
      
      cmd.ExecuteNonQuery();
    }
    
    /*
     * Adds or updates option in the 'options' table
     */
    public void updateOption(string name, string value){
      updateOption(name, value, options_table);
    }
    
    /*
     * Adds or updates last option in the 'config' table
     * Redirects to the updateOption()
     */
    public void updateConfig(string name, string value){
      updateOption(name,value,config_table);
    }
    
    /*
     * Loads last options from the database to assign them to the form fields
     */
    public string loadLastOptions(){
      return getOption("last", config_table);
    }
    
    public string getOption(string name){
      return getOption(name, options_table);
    }
    
    /*
     * Retrieves option from the database when an item is selected in the dropdown
     * to load options set into the form fields
     */
    public string getOption(string name, string table){
      sql="select value from "+table+" where name=@name";

      if (!connected) return null;
      
      MySqlCommand cmd = new MySqlCommand(sql, conn);
      
      cmd.Prepare();
      cmd.Parameters.AddWithValue("@name", name);
      
      MySqlDataReader dataReader = cmd.ExecuteReader();
      if (!dataReader.Read()) {
        dataReader.Close();
        return null;
      }

      res=(string) dataReader["value"];
      dataReader.Close();

      return res;
    }
    
    
    
    
    

    //Insert statement
    public void Insert() {
      string query = "INSERT INTO tableinfo (name, age) VALUES('John Smith', '33')";

      //open conn
      if (this.OpenConnection() == true) {
        //create command and assign the query and conn from the constructor
        MySqlCommand cmd = new MySqlCommand(query, conn);

        //Execute command
        cmd.ExecuteNonQuery();

        //close conn
        this.CloseConnection();
      }
    }

    //Update statement
    public void Update() {
      string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";

      //Open conn
      if (this.OpenConnection() == true) {
        //create mysql command
        MySqlCommand cmd = new MySqlCommand();
        //Assign the query using CommandText
        cmd.CommandText = query;
        //Assign the conn using Connection
        cmd.Connection = conn;

        //Execute query
        cmd.ExecuteNonQuery();

        //close conn
        this.CloseConnection();
      }
    }

    //Delete statement
    public void Delete() {
      string query = "DELETE FROM tableinfo WHERE name='John Smith'";

      if (this.OpenConnection() == true) {
        MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        this.CloseConnection();
      }
    }

    //Select statement
    public List<string>[] Select() {
      string query = "SELECT * FROM tableinfo";

      //Create a list to store the result
      List<string>[] list = new List<string>[3];
      list[0] = new List<string>();
      list[1] = new List<string>();
      list[2] = new List<string>();

      //Open conn
      if (this.OpenConnection() == true) {
        //Create Command
        MySqlCommand cmd = new MySqlCommand(query, conn);
        //Create a data reader and Execute the command
        MySqlDataReader dataReader = cmd.ExecuteReader();

        //Read the data and store them in the list
        while (dataReader.Read()) {
          list[0].Add(dataReader["id"] + "");
          list[1].Add(dataReader["name"] + "");
          list[2].Add(dataReader["age"] + "");
        }

        //close Data Reader
        dataReader.Close();

        //close Connection
        this.CloseConnection();

        //return list to be displayed
        return list;
      }
      else {
        return list;
      }
    }
    
    //Count statement
    public int Count()
    {
        string query = "SELECT Count(*) FROM tableinfo";
        int Count = -1;

        //Open Connection
        if (this.OpenConnection() == true)
        {
            //Create Mysql Command
            MySqlCommand cmd = new MySqlCommand(query, conn);

            //ExecuteScalar will return one value
            Count = int.Parse(cmd.ExecuteScalar()+"");
            
            //close Connection
            this.CloseConnection();

            return Count;
        }
        else
        {
            return Count;
        }
    }

  }
}
