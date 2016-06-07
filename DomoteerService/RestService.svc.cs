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

        //private String connectionString = @"Data Source=PC-FRA\SQLEXPRESS;Initial Catalog=DomoteerDatabase;Integrated Security=True";

        private String connectionString = @"Data Source=FABRIZIO\SQLEXPRESS2;Initial Catalog=Domoteer;Persist Security Info=True;User ID=sa;Password=root123123";


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
                            t.temperature = temperaturesReader.GetString(1);
                            t.timestamp = temperaturesReader.GetString(2);

                            log.Debug("- temperature: " + t.temperature + " - timestamp: " + t.timestamp);

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
            log.Debug("Put Temperatures request");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand insertTemp = new SqlCommand("insert into Temperature(temperature, T_timestamp) values (@temp,@date)", conn);

                    insertTemp.Parameters.AddWithValue("@temp", t == null ? "" : t);
                    insertTemp.Parameters.AddWithValue("@date", date == null ? "" : date);
                    log.Debug("- temperature: " + t + " - timestamp: " + date);
                    insertTemp.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                    Console.WriteLine("Error on query: " + ex.ToString());
                    return "error";
                }




                return "put: " + t + " at: " + date;
            }
        }

        public List<Gas> getGas(String n)
        {
            log.Debug("Get Gas request");

            List<Gas> gasses = new List<Gas>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand selectTemp = new SqlCommand("select * from Gas order by T_timestamp", conn);
                    SqlDataReader temperaturesReader = selectTemp.ExecuteReader();

                    if (temperaturesReader.HasRows)
                    {

                        int i = 0;
                        while (temperaturesReader.Read() && i < int.Parse(n))
                        {
                            Gas g = new Gas();
                            g.lpg = temperaturesReader.GetString(1);
                            g.CO = temperaturesReader.GetString(2);
                            g.SMOKE = temperaturesReader.GetString(3);
                            g.timestamp = temperaturesReader.GetString(4);

                            log.Debug("- lpg: " + g.lpg + " - CO: " + g.CO +  " - SMOKE: " + g.SMOKE + " - timestamp: " + g.timestamp);

                            gasses.Add(g);
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


            return gasses;
        }

        public String putGas(String lpg, String co, String smoke, String date)
        {
            log.Debug("Put Gas request");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    log.Debug("Pre QUERY - lpg: " + lpg + " - CO: " + co + " - SMOKE: " + smoke + " - timestamp: " + date);
                    conn.Open();
                    SqlCommand insertTemp = new SqlCommand("insert into Gas(lpg, co, smoke, T_timestamp) values (@lpg, @co, @smoke ,@date)", conn);
                    insertTemp.Parameters.AddWithValue("@lpg", lpg == null ? "" : lpg);
                    insertTemp.Parameters.AddWithValue("@co", co == null ? "" : co);
                    insertTemp.Parameters.AddWithValue("@smoke", smoke == null ? "" : smoke);
                    insertTemp.Parameters.AddWithValue("@date", date == null ? "" : date);
                    log.Debug("- lpg: " + lpg + " - CO: " + co + " - SMOKE: " + smoke + " - timestamp: " + date);
                    insertTemp.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                    Console.WriteLine("Error on query: " + ex.ToString());
                    return "error";
                }




                return "put: - lpg: " + lpg + " - CO: " + co + " - SMOKE: " + smoke  + " at: " + date;
            }
        }
    }
}
