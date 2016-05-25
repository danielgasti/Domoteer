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


namespace Re_Do_Do
{
    class DomoteerWebServer
    {
        EthernetJ11D eth;
        MulticolorLED led;
        DisplayT35 display;
        string ipAddress;
        private Hashtable WebPageList;
        
        public DomoteerWebServer(EthernetJ11D eth, MulticolorLED led, DisplayT35 display) 
        {
            this.eth = eth;
            this.led = led;
            this.display = display;
        }

        /// <summary>
        /// setup della connessione
        /// </summary>
        public void initConnection()
        {
            eth.UseThisNetworkInterface();
            eth.NetworkSettings.EnableDhcp();
            
            ///homegateway CICCIO
            //eth.NetworkSettings.EnableStaticIP("192.168.0.222", "255.255.255.0", "192.168.0.1");
            ///homegateway FABRI
            eth.NetworkSettings.EnableStaticIP("192.168.1.222", "255.255.255.0", "192.168.1.254");
            
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

            #region GESTIONE PAGINA WEB
            WebPageList = new Hashtable();
            HomePageData = new WebPage(Resources.GetString(Resources.StringResources.index), "index.htm", "text/html");
            PlotsPageData = new WebPage(Resources.GetString(Resources.StringResources.plots), "plots.htm", "text/html");
            CssPageData = new WebPage(Resources.GetString(Resources.StringResources.bootstrap_min), "bootstrap.min.css", "text/css");
            JsElaboration = new WebPage(Resources.GetString(Resources.StringResources.jquery_2_2_4_min), "jquery-2.2.4.min.js", "text/javascript");
            JsPlots = new WebPage(Resources.GetString(Resources.StringResources.Chart_bundle_min), "Chart.bundle.min.js", "text/javascript");
            JsBootstrap = new WebPage(Resources.GetString(Resources.StringResources.bootstrap_min1), "bootstrap.min.js", "text/javascript");
            WebPageList.Add("index.htm", HomePageData);
            WebPageList.Add("plots.htm", PlotsPageData);
            WebPageList.Add("bootstrap.min.css", CssPageData);
            WebPageList.Add("jquery-2.2.4.min.js", JsElaboration);
            WebPageList.Add("Chart.bundle.min.js", JsPlots);
            WebPageList.Add("bootstrap.min.js", JsBootstrap);

            foreach (string key in WebPageList.Keys)
            {
                WebServer.SetupWebEvent(key).WebEventReceived += new WebEvent.ReceivedWebEventHandler(WebEventHandler);
            }
            #endregion


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
        public void WebEventHandler(string path, WebServer.HttpMethod method, Responder responder)
        {
            try
            {
                WebPage PageData = (WebPage)WebPageList[path];
                string content = PageData.Content;

                if (PageData.Url.ToLower() == "index.htm")
                {
                    if (method == WebServer.HttpMethod.POST)
                    {
                        content = fixResponderText(responder);
                        if (content == null)
                        {
                            content = "responder.Body.Text is null, cannot be fixed";
                        }
                    }
                    /*else if (method == WebServer.HttpMethod.GET)
                    {
                        if ((netData != null) && (systemData != null))
                        {
                            //replace placeholder text vertical bar in web page with the run-time text
                            //regular expressions are much too slow, use split function instead
                            string[] parts = content.Split(new char[] { '|' });
                            if (parts.Length == 9)
                            {
                                content = parts[0] + netData.MacAddress.ToHex()
                                        + parts[1] + netData.StaticIP
                                        + parts[2] + netData.NetMask
                                        + parts[3] + netData.GatewayAddress
                                        + parts[4] + netData.CloudIP
                                        + parts[5] + systemData.ModelNumber
                                        + parts[6] + systemData.SerialNumber
                                        + parts[7] + systemData.DeviceHiveId.ToString()
                                        + parts[8];
                            }
                        }
                        else
                        {
                            content = "memory cannot be read";
                        }
                    }*/
                }

                byte[] data = Encoding.UTF8.GetBytes(content);
                responder.Respond(data, PageData.MimeType);
            }
            catch (Exception ex)
            {
                Debug.Print("WebEventReceived(): " + ex.ToString());
            }
        }

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


}
