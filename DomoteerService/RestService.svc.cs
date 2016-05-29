using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace DomoteerService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "Service1" nel codice, nel file svc e nel file di configurazione contemporaneamente.
    // NOTA: per avviare il client di prova WCF per testare il servizio, selezionare Service1.svc o Service1.svc.cs in Esplora soluzioni e avviare il debug.
    
    public class RestService : IRestService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String connectionString = @"Data Source=PC-FRA\SQLEXPRESS;Initial Catalog=DomoteerDatabase;Integrated Security=True";

        //private String connectionString = @"Data Source=FABRIZIO;Initial Catalog=Domoteer;Integrated Security=True";
        

        string connStr = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        private readonly String localConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\DomoteerDatabase.mdf;Integrated Security=True";
        
        public List<Temperature> getTemperatures(String n)
        {
            log.Debug("Get Temperatures request");

            List<Temperature> temperatures = new List<Temperature>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand selectTemp = new SqlCommand("select * from Temperature order by T_timestamp", conn);
                    SqlDataReader temperaturesReader = selectTemp.ExecuteReader();

                    if (temperaturesReader.HasRows)
                    {
                        
                        int i = 0;
                        while (temperaturesReader.Read() && i < int.Parse(n))
                        {
                            Temperature t = new Temperature();
                            t.temperature = Double.Parse(temperaturesReader.GetString(1));
                            t.timestamp = temperaturesReader.GetInt32(2).ToString();

                            log.Debug("temperature: " + t.temperature + " - timestamp: " + t.timestamp);

                            temperatures.Add(t);
                            i++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                    Console.WriteLine("Error on query: " + ex.ToString());
                }
                


            }
            
            
            return temperatures;
        }

        public String putTemperatures(String t, String date)
        {
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();
            //    SqlCommand insert = new SqlCommand("insert into Temperature (timestamp,temperature) values( " + t + " , " + date + ");", conn);
            //    insert.ExecuteNonQuery();

                

            //}
            return "put: " + t + " at: " + date;
        }
    }
}
