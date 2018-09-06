using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NLua;
using Mallet.Telegram;
using System.Threading;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Mallet.ILuaState
{
    public static class sql
    {

        private static Lua State;
        private static MySqlConnection sqConnection;
        private static string last_error = "";
        private static LuaTable Results;


        public static bool isconnected()
        {
            if (sqConnection == null | sqConnection.State == System.Data.ConnectionState.Closed)
            {
                return false;
            }
            return true;
        }

        public static bool connect(string host,string database, string user, string password )
        {
            var server = host;
   
            var uid = user;
           
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";SslMode=none;";

            sqConnection = new MySqlConnection(connectionString);
            try
            {
                sqConnection.Open();
            } catch (Exception E)
            {
                last_error = E.ToString();
                return false;
            }
            

            return true;
        }

        public static string getLastError()
        {
            return last_error;
        }

        public static LuaTable getResults()
        {
           


            return Results;
        }

        public static bool nonquery(string data)
        {
            if (!isconnected())
            {
                last_error = "Connection is not opened.";
                return false;
            }


            var tran = sqConnection.CreateCommand();
            tran.CommandText = data;
            try
            {
                tran.ExecuteNonQuery();
            } catch (Exception E) {
               last_error = E.ToString();
                return false;
            }
            
            return true;
        }

        public static bool query(string data)
        {
            if (!isconnected())
            {
                last_error = "Connection is not opened.";
                return false;
            }
          
            var tran = sqConnection.CreateCommand();
            tran.CommandText = data;
            tran.Prepare();
            LuaTable results = LuaUtil.EmptyTable(State);

            try
            {
                var resu =  tran.ExecuteReader();
                
                var colcount = resu.FieldCount;
                var i = 0;
            
                while (resu.Read())
                {
                 
                    i++;
                    LuaTable row = LuaUtil.EmptyTable(State);
                    for (int f = 0; f < colcount; f++)
                    {
                        row[resu.GetName(f)] = resu[f];
                    }

                    results[i] = row;

                }

                resu.Close();

                Results = results;
                
            }
            catch (Exception E)
            {
                last_error = E.ToString();
                return false;
            }
            return true;
        }

        public static void InitLibrary(KLuaState state)
        {
            Console.WriteLine("State C Library: SQL");

            state.runLua(" sql = {}");
            var ILuaInterface = state.getLuaState();
            State = ILuaInterface;



            ILuaInterface.RegisterFunction("sql.connect", null, typeof(sql).GetMethod("connect"));
            ILuaInterface.RegisterFunction("sql.lastError", null, typeof(sql).GetMethod("getLastError"));
            ILuaInterface.RegisterFunction("sql.query", null, typeof(sql).GetMethod("query"));
            ILuaInterface.RegisterFunction("sql.nonquery", null, typeof(sql).GetMethod("nonquery"));
            ILuaInterface.RegisterFunction("sql.getResults", null, typeof(sql).GetMethod("getResults"));
            ILuaInterface.RegisterFunction("sql.isConnected", null, typeof(sql).GetMethod("isconnected"));
        }
    }
}


