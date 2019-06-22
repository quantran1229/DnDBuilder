using System;
using System.IO;
using System.Web.Http;
using Mono.Data.Sqlite;

namespace DndBuilder
{
    public static class WebApiConfig
    {
        private const string DB_NAME = "DnbBuilderDbs.sqlite";
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.Insert(0, new System.Net.Http.Formatting.JsonMediaTypeFormatter());

            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings 
            { 
                Formatting = Newtonsoft.Json.Formatting.Indented, ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore };

            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

            //check if database exist? if not then create one
            createNewDatabase();
            createTable();
        }

        private static void createNewDatabase()
        {
            try 
            { 
                if(!File.Exists(DB_NAME))
                {
                    Console.WriteLine("Create new database");
                    SqliteConnection.CreateFile(DB_NAME);
                    if (File.Exists(DB_NAME))
                    {
                        Console.WriteLine("Finishing created new database");
                    }
                }
                else
                {
                    Console.WriteLine("database exist!!!");
                    //testing delete database
                    //File.Delete(DB_NAME);
                    //SqliteConnection.CreateFile(DB_NAME);
                }
            }
            catch (SqliteException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void createTable()
        {
            try
            {
                using (SqliteConnection db_conn = new SqliteConnection("Data Source=" + DB_NAME + ";Version=3;"))
                {

                    db_conn.Open();
                    string sql = "create table Characters (Name varchar(30) primary key,Age smallint,Gender varchar(10),Biography varchar(500),Level tinyint, Race varchar(20),Class varchar(20), Con tinyint,Dex tinyint,Str tinyint, Cha tinyint,Int tinyint,Wis tinyint )";
                    SqliteCommand command = new SqliteCommand(sql, db_conn);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Created Characters table");
                    db_conn.Close();
                }
            }
            catch (SqliteException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
