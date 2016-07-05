
using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Networking;
using System.Threading;
using Gadgeteer;
using System.Text;
using System.IO;
using System.Collections;
using GHI.Processor;
using Microsoft.SPOT.Hardware;



namespace Re_Do_Do
{
    class DomoteerWebServer
    {
        EthernetJ11D eth;
        MulticolorLED led;
        DisplayT35 display;
        string ipAddress;
        private Hashtable WebPageList;
        Sensore_Temperatura_43 temperatureSensor;
        private string temperature;
        private string lpg;
        private string co;
        private string smoke;
        private string timestamp;

        private string IPAddress = "http://192.168.0.8/Domoteer/";

        Stopwatch sw = new Stopwatch();
        
        public DomoteerWebServer(EthernetJ11D eth, MulticolorLED led, DisplayT35 display, Sensore_Temperatura_43 s) 
        {
            this.eth = eth;
            this.led = led;
            this.display = display;
            temperatureSensor = s;
        }

        /// <summary>
        /// setup della connessione
        /// </summary>
        public void initConnection()
        {
            eth.UseThisNetworkInterface();
            eth.NetworkSettings.EnableDhcp();
            
            ///homegateway CICCIO
            eth.NetworkSettings.EnableStaticIP("192.168.0.222", "255.255.255.0", "192.168.0.1");
            ///homegateway FABRI
            //eth.NetworkSettings.EnableStaticIP("192.168.1.222", "255.255.255.0", "192.168.1.254");
            
            //ListNetworkInterfaces();

            eth.NetworkUp += ethernet_NetworkUp;
            eth.NetworkDown += ethernet_NetwokDown;
        }


        public void RunWebServer()
        {
            new Thread(_RunWebServer).Start();
        }


        private void _RunWebServer()
        {
            // Wait for the network...
            while (eth.IsNetworkUp == false)
            {
                Debug.Print("Waiting...");
                Thread.Sleep(1000);
            }

            // Start the server
            WebServer.StartLocalServer(eth.NetworkSettings.IPAddress, 80);
            WebServer.DefaultEvent.WebEventReceived += DefaultEvent_WebEventReceived;

            //#region GESTIONE PAGINA WEB
            //WebPageList = new Hashtable();
            //HomePageData = new WebPage(HtmlIndex.buildPage(), "index.htm", "text/html");
            //PlotsPageData = new WebPage(Resources.GetString(Resources.StringResources.plots), "plots.htm", "text/html");
            //CssPageData = new WebPage(Resources.GetString(Resources.StringResources.bootstrap_min), "bootstrap.min.css", "text/css");
            //JsElaboration = new WebPage(Resources.GetString(Resources.StringResources.jquery_2_2_4_min), "jquery-2.2.4.min.js", "text/javascript");
            //JsPlots = new WebPage(Resources.GetString(Resources.StringResources.Chart_bundle_min), "Chart.bundle.min.js", "text/javascript");
            //JsBootstrap = new WebPage(Resources.GetString(Resources.StringResources.bootstrap_min1), "bootstrap.min.js", "text/javascript");
            //WebPageList.Add("index.htm", HomePageData);
            //WebPageList.Add("plots.htm", PlotsPageData);
            //WebPageList.Add("bootstrap.min.css", CssPageData);
            //WebPageList.Add("jquery-2.2.4.min.js", JsElaboration);
            //WebPageList.Add("Chart.bundle.min.js", JsPlots);
            //WebPageList.Add("bootstrap.min.js", JsBootstrap);

            //foreach (string key in WebPageList.Keys)
            //{
            //    WebServer.SetupWebEvent(key).WebEventReceived += new WebEvent.ReceivedWebEventHandler(WebEventHandler);
            //}
            //#endregion


            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// la funzione stampa a video tutte le caratteristiche della connessione
        /// </summary>
        void ListNetworkInterfaces()
        {
            var settings = eth.NetworkSettings;
            Debug.Print("------------------------------------------------");
            Debug.Print("MAC: " + ByteExtensions.ToHexString(settings.PhysicalAddress, "-"));
            Debug.Print("IP Address:   " + settings.IPAddress);
            Debug.Print("DHCP Enabled: " + settings.IsDhcpEnabled);
            Debug.Print("Subnet Mask:  " + settings.SubnetMask);
            Debug.Print("Gateway:      " + settings.GatewayAddress);
            Debug.Print("------------------------------------------------");
        }

        #region EVENT HANDLERS
        /// <summary>
        /// evento scatenato a seguito dell'avvenuta connessione ad internet
        /// </summary>
        void ethernet_NetworkUp(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
        {
            //Set the URI for the resource
            Debug.Print("Network up.");
            led.TurnGreen();
            ListNetworkInterfaces();
            getServerTime();
        }

        /// <summary>
        /// evento scatenato dal fallimento nella connessione ad internet
        /// </summary>
        void ethernet_NetwokDown(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down.");
            led.TurnRed();
        }

        /// <summary>
        /// handler di defalut
        /// TODO Testare che la pagina venga visualizzata correttamente (con i css)
        /// </summary>
        void DefaultEvent_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            string str = Resources.GetString(Resources.StringResources.index);
            byte[] HTML = System.Text.Encoding.UTF8.GetBytes(str);
            // We always send the same page back
            responder.Respond(HTML, "index/html;charset=utf-8");
            // If a button was clicked
            if (method == WebServer.HttpMethod.POST)
            {
                Debug.Print("Path: " + path);  
            }
        }

        /// <summary>
        /// handler per l'accesso alla pagina "index.htm"
        /// la parte commentata corrisponde alla gestione delle richieste di tipo GET
        /// </summary>
        //public void WebEventHandler(string path, WebServer.HttpMethod method, Responder responder)
        //{
        //     try
        //    {
        //        WebPage PageData = (WebPage)WebPageList[path];
        //        string content = PageData.Content;

        //        if (PageData.Url.ToLower() == "index.htm")
        //        {
        //            Temperatura t = temperatureSensor.getTemp();
        //            double temperatureValor = t.BinToCelsius();
        //            HtmlIndex.temperatura = temperatureValor.ToString();
        //            WebPageList.Remove("index.htm");
        //            HomePageData = new WebPage(HtmlIndex.buildPage(), "index.htm", "text/html");
        //            WebPageList.Add("index.htm", HomePageData);

        //            //TEMPI
        //            sw.Stop();
        //            Debug.Print("tempo impiegato: LETTURA SENSORE -> " + sw.Elapsed);
        //            sw.Start();

        //            if (method == WebServer.HttpMethod.POST)
        //            {
        //                content = fixResponderText(responder);
        //                if (content == null)
        //                {
        //                    content = "responder.Body.Text is null, cannot be fixed";
        //                }
        //            }
        //            /*else if (method == WebServer.HttpMethod.GET)
        //            {
        //                if ((netData != null) && (systemData != null))
        //                {
        //                    //replace placeholder text vertical bar in web page with the run-time text
        //                    //regular expressions are much too slow, use split function instead
        //                    string[] parts = content.Split(new char[] { '|' });
        //                    if (parts.Length == 9)
        //                    {
        //                        content = parts[0] + netData.MacAddress.ToHex()
        //                                + parts[1] + netData.StaticIP
        //                                + parts[2] + netData.NetMask
        //                                + parts[3] + netData.GatewayAddress
        //                                + parts[4] + netData.CloudIP
        //                                + parts[5] + systemData.ModelNumber
        //                                + parts[6] + systemData.SerialNumber
        //                                + parts[7] + systemData.DeviceHiveId.ToString()
        //                                + parts[8];
        //                    }
        //                }
        //                else
        //                {
        //                    content = "memory cannot be read";
        //                }
        //            }*/
        //        }

        //        //TEMPI
        //        sw.Stop();
        //        Debug.Print("tempo impiegato: FIX RESPONDER -> " + sw.Elapsed);
        //        sw.Start();
        //        byte[] data = Encoding.UTF8.GetBytes(content);

        //        //TEMPI
        //        sw.Stop();
        //        Debug.Print("tempo impiegato: encoding -> " + sw.Elapsed);
        //        sw.Start();

        //        responder.Respond(data, PageData.MimeType);

        //        //TEMPI
        //        sw.Stop();
        //        Debug.Print("tempo impiegato: RESPOND -> " + sw.Elapsed);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Print("WebEventReceived(): " + ex.ToString());
        //    }
        //}

        /// <summary>
        /// gestione del responder.Body.Text (che può essere null)
        /// </summary>
        public string fixResponderText(Responder responder)
        {
            string content = null;
            if (responder.Body != null)
            {
                if (responder.Body.Text != null)
                {
                    Debug.Print("responder.Body.Text is valid");
                    content = responder.Body.Text;
                }
                else
                {
                    //random bug here - responder.Body.Text is null, but the responder.Body.RawContent byte array contains the text offset by many zeros
                    int len = responder.Body.RawContent.Length;
                    byte[] raw = new byte[len];
                    for (int i = 0, j = 0; i < len; i++)
                    {
                        if (responder.Body.RawContent[i] > 0)
                        {
                            raw[j] = responder.Body.RawContent[i];
                            j++;
                        }
                    }
                    if (raw.Length > 0)
                    {
                        content = new string(Encoding.UTF8.GetChars(raw));
                        Debug.Print("responder.Body.Text is null, I fixed it");
                    }
                    else
                    {
                        Debug.Print("responder.Body.Text is null, cannot be fixed");
                    }
                }
            }
            return content;
        }

        #endregion

        public WebPage HomePageData { get; set; }

        public WebPage CssPageData { get; set; }

        public WebPage JsElaboration { get; set; }

        public WebPage JsPlots { get; set; }

        public WebPage PlotsPageData { get; set; }

        public WebPage JsBootstrap { get; set; }


        #region Service

        public void GetTemperatures(string n)
        {
            // Create the form values
            var formValues = "n=" + n;

            
            // Create GET content
            var content = Gadgeteer.Networking.POSTContent.CreateTextBasedContent(formValues);
            POSTContent emptyPost = new POSTContent();

            // Create the request
            var request = Gadgeteer.Networking.HttpHelper.CreateHttpPostRequest(
                IPAddress + @"RestService.svc/getTemperatures?n=" + n // the URL to post to
                , emptyPost // the form values
                , null // the mime type for an HTTP form
            );

            request.ResponseReceived += new HttpRequest.ResponseHandler(GetTemperatures_ResponseReceived);

            // Post the form
            request.SendRequest();
        }

        public void PutCross(string t)
        {
            // Create the form values
            var formValues = "date=" + t;

            Debug.Print("Cross timestamp: " + t);
            // Create GET content
            //var content = Gadgeteer.Networking.POSTContent.CreateTextBasedContent(formValues);
            POSTContent emptyPost = new POSTContent();

            // Create the request
            var request = Gadgeteer.Networking.HttpHelper.CreateHttpPostRequest(
                IPAddress + @"RestService.svc/putCross?" + formValues // the URL to post to
                , emptyPost // the form values
                , null // the mime type for an HTTP form
            );

            request.ResponseReceived += new HttpRequest.ResponseHandler(PutCross_ResponceRecevided);

            // Post the form
            request.SendRequest();
        }

        private void PutCross_ResponceRecevided(HttpRequest sender, HttpResponse response)
        {
            Debug.Print("Movimento Salvato\n");
        }

        private void GetTemperatures_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            Debug.Print("Response received:\n");
            Debug.Print("- response status = " + response.StatusCode);
            Debug.Print("- response content =\n" + response.Text);
        }

        public void PutTemperatures(string t, string date)
        {
            // Create the form values
            var formValues = "temperature=" + t + "&date=" + date;


            // Create GET content
            //var content = Gadgeteer.Networking.POSTContent.CreateTextBasedContent(formValues);
            POSTContent emptyPost = new POSTContent();

            // Create the request
            var request = Gadgeteer.Networking.HttpHelper.CreateHttpPostRequest(
                IPAddress + @"RestService.svc/putTemperatures?" + formValues // the URL to post to
                , emptyPost // the form values
                , null // the mime type for an HTTP form
            );

            request.ResponseReceived += new HttpRequest.ResponseHandler(PutTemperatures_ResponseReceived);

            // Post the form
            request.SendRequest();
        }

        private void PutTemperatures_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            Debug.Print("Response received:\n");
            Debug.Print("- response status = " + response.StatusCode);
            Debug.Print("- response content =\n" + response.Text);
            PutGas(lpg, co, smoke, timestamp);
        }

        public void GetGas(string n)
        {
            // Create the form values
            var formValues = "n=" + n;


            // Create GET content
            var content = Gadgeteer.Networking.POSTContent.CreateTextBasedContent(formValues);
            POSTContent emptyPost = new POSTContent();

            // Create the request
            var request = Gadgeteer.Networking.HttpHelper.CreateHttpPostRequest(
                IPAddress + @"RestService.svc/getGas?n=" + n // the URL to post to
                , emptyPost // the form values
                , null // the mime type for an HTTP form
            );

            request.ResponseReceived += new HttpRequest.ResponseHandler(GetGas_ResponseReceived);

            // Post the form
            request.SendRequest();
        }

        private void GetGas_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            Debug.Print("Response received:\n");
            Debug.Print("- response status = " + response.StatusCode);
            Debug.Print("- response content =\n" + response.Text);
        }

        public void PutGas(String lpg, String co, String smoke, String date)
        {
            // Create the form values
            var formValues = "lpg=" + lpg + "&co=" + co + "&smoke=" + smoke + "&date=" + date;


            // Create GET content
            //var content = Gadgeteer.Networking.POSTContent.CreateTextBasedContent(formValues);
            POSTContent emptyPost = new POSTContent();

            // Create the request
            var request = Gadgeteer.Networking.HttpHelper.CreateHttpPostRequest(
                IPAddress + @"RestService.svc/putGas?" + formValues // the URL to post to
                , emptyPost // the form values
                , null // the mime type for an HTTP form
            );

            request.ResponseReceived += new HttpRequest.ResponseHandler(PutGas_ResponseReceived);

            // Post the form
            request.SendRequest();
        }

        private void PutGas_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            Debug.Print("Response received:\n");
            Debug.Print("- response status = " + response.StatusCode);
            Debug.Print("- response content =\n" + response.Text);
        }

        #endregion


        public void pushData(string temperature, string lpg, string co, string smoke, string timestamp)
        {
            this.temperature = temperature;
            this.lpg = lpg;
            this.co = co;
            this.smoke = smoke;
            this.timestamp = timestamp;
            PutTemperatures(temperature, timestamp);
        }


        public void getServerTime()
        {
            POSTContent emptyPost = new POSTContent();

            // Create the request
            var request = Gadgeteer.Networking.HttpHelper.CreateHttpPostRequest(
                IPAddress + @"RestService.svc/getServerTime" // the URL to post to
                , emptyPost // the form values
                , null // the mime type for an HTTP form
            );

            request.ResponseReceived += new HttpRequest.ResponseHandler(getServerTime_ResponseReceived);

            // Post the form
            request.SendRequest();
        }

        private void getServerTime_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            Debug.Print("Response received:\n");
            Debug.Print("- response status = " + response.StatusCode);
            Debug.Print("- response content =\n" + response.Text);
            if (response.StatusCode == "200")
            {
                DateTime dt;

                String timestamp = response.Text.Substring(24, 14);
                Debug.Print("timestamp acquired: " + timestamp);
                int yyyy = Int32.Parse(timestamp.Substring(0, 4));
                int MM = Int32.Parse(timestamp.Substring(4, 2));
                int dd = Int32.Parse(timestamp.Substring(6, 2));
                int hh = Int32.Parse(timestamp.Substring(8, 2));
                int mm = Int32.Parse(timestamp.Substring(10, 2));
                int ss = Int32.Parse(timestamp.Substring(12, 2));

                dt = new DateTime(yyyy, MM, dd, hh, mm, ss);
                //Utility.SetLocalTime(dt);

                if(dt.Year < 2011){
                    return;
                }

                RealTimeClock.SetDateTime(dt);
            }
        }





    }





    public class WebPage
    {
        public string Content;
        public string MimeType;
        public string Url;

        public WebPage(string _Content, string _Url, string _MimeType)
        {
            Content = _Content;
            Url = _Url;
            MimeType = _MimeType;
        }

        

    }



//    public static class HtmlIndex {
//        public static string temperatura { get; set; }
//        public static string gas { get; set; }
//        public static string passaggi { get; set; }

//        static string head = @"<!DOCTYPE html> 
//                        <html lang='en'>
//                        <head>
//                          <title>Domoteer</title>
//                            <meta charset='utf-8'>
//                            <meta name='viewport' content='width=device-width, initial-scale=1'>
//                            <link rel='stylesheet' href='bootstrap.min.css'>
//                            <script src='jquery-2.2.4.min.js'></script>
//                            <script src='bootstrap.min.js'></script>
//                            <script src='Chart.bundle.min.js'></script>
//                          
//                                <style>
//                                h1{text-align: center;}
//                                h3{text-align: center;}
//                                h5{text-align: center;}
//                                h6{
//	                                text-align: center;
//	                                color: #555;
//                                }
//
//                                .navbar-inverse .navbar-nav > li > a{
//	                                color: black;
//                                }
//
//                                .navbar-inverse{
//	                                background-color: hsla(230, 100%, 75%, 0.6);
//	                                border-color: hsla(230, 100%, 75%, 0.6);	
//                                }
//
//                                .azure{
//	                                background-color: hsl(200, 28%, 90%);
//	                                border-color: hsl(200, 28%, 90%);
//                                }
//
//                                .white{
//	                                background-color: white;
//	                                border-color: white;
//	                                height: 100%;
//                                }
//
//                                footer{
//	                                background-color: hsla(230, 100%, 75%, 0.6);
//	                                border-color: hsla(230, 100%, 75%, 0.6);
//	                                color: white;
//	                                padding: 15px;
//                                }
//
//                                .navbar {
//	                                margin-bottom: 0;
//	                                border-radius: 0;
//                                }
//
//                                .navbar-brand-domoteer{
//	                                float: left;
//                                    height: 50px;
//                                    padding: 1px 15px;
//                                    font-size: 18px;
//                                    line-height: 20px;
//                                }    
//   
//
//                           /* Set height of the grid so .sidenav can be 100% (adjust as needed) */
//                            .row.content {height: 450px}
//    
//                            /* On small screens, set height to 'auto' for sidenav and grid */
//                            @media screen and (max-width: 767px) {
//                              .sidenav {
//                                height: auto;
//                                padding: 15px;
//                              }
//                              .row.content {height:auto;} 
//                            }
//                          </style>
//                        </head>";


//        static String[] body = new String[] {
//            @"<body>
//                <nav class='navbar navbar-inverse'>
//                  <div class='container-fluid'>
//                    <div class='navbar-header'>
//                      <button type='button' class='navbar-toggle' data-toggle='collapse' data-target='#myNavbar'>
//                        <span class='icon-bar'></span>
//                        <span class='icon-bar'></span>
//                        <span class='icon-bar'></span>                        
//                      </button>
//                      <a class='navbar-brand-domoteer' href='#'>
//		                <img src='domoteer.ico'>
//	                  </a>
//                    </div>
//                    <div class='collapse navbar-collapse' id='myNavbar'>
//                      <ul class='nav navbar-nav'>
//                       <li><a class='head_bar' href='index.htm'>Home</a></li>
//                        <li><a class='head_bar' href='plots.htm'>Plots</a></li>
//                      </ul>
//                      <ul class='nav navbar-nav navbar-right'>
//                        <!--<li><a href='#'><span class='glyphicon glyphicon-log-in'></span> Login</a></li>-->
//                      </ul>
//                    </div>
//                  </div>
//                </nav>
//  
//                <div class='container-fluid text-center azure'>    
//                  <div class='row content'>
//                    <div class='col-sm-2'>
//                    </div>
//                    <div class='col-sm-8 text-left white'> 
//                      <h1>Welcome to Domoteer</h1>
//                      <h5>the domotic monitoring system</h5>
//                      <hr>
//		                <div class='row' >
//                            <div class='col-sm-4' id='temperature'>
//                                <h3>Rilevamento temperatura</h3>
//                                <br><br>
//                                <h1>",

//            //parte per la gestione del rilevamento gas
//            @"<h1>
//            </div>
//			<div class='col-sm-4'>
//				<h3>Rilevamento gas</h3>
//				<br><br>
//				<h1>",
            
//            //parte per la gestione del rilevamento passaggi
//            @"<h1>
//			                </div>
//			                <div class='col-sm-4'>
//				                <h3>Rilevamento passaggi</h3>
//				                <br><br>
//				                <h1>",
        
//            // 
//            @"<h1>
//			                </div>
//		                </div>
//                    </div>
//                    <div class='col-sm-2'>
//                    </div>
//                  </div>
//                </div>
//
//                <footer class='container-fluid text-center'>
//                <h6>Developers: Gastinelli Daniel, Oddera Fabrizio, Ventura Francesco 2016.</h6>
//                </footer>
// 
//                </body>
//                </html>"
//        };

//        public static String buildPage(){
//            return head + body[0] + temperatura + body[1] + gas + body[2] + passaggi + body[3];
//        }

//    }



}
