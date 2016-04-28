using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using System.Text;


namespace Re_Do_Do
{

        public partial class Program
        {

            GT.Networking.WebEvent sayHello;

            byte[] HTML = Encoding.UTF8.GetBytes(
                "<html><body>" +
                "<h1>Hosted on .NET Gadgeteer</h1>" +
                "<p>Lets scare someone!</p>" +
                "<form action=\"\" method=\"post\">" +
                "<input type=\"submit\" value=\"Toggle LED!\">" +
                "</form>" +
                "</body></html>");

            void ProgramStarted()
            {
                //The other option is UseStaticIP
                //ethernetJ11D.NetworkSettings.EnableDhcp();
                //ethernetJ11D.NetworkSettings.EnableDynamicDns();
                
                ethernetJ11D.UseThisNetworkInterface();
                //ethernetJ11D.UseDHCP();

                ethernetJ11D.NetworkSettings.EnableStaticIP("192.168.0.222", "255.255.255.0", "192.168.0.1");
                //ethernetJ11D.NetworkSettings.PhysicalAddress = ByteExtensions.ToHexByte("002103804AF0");

                //ethernetJ11D.UseStaticIP("192.168.1.222", "255.255.255.0", "192.168.1.254");

                //Set a handler for when the network is available
                ethernetJ11D.NetworkUp += ethernet_NetworkUp;
                ethernetJ11D.NetworkDown += ethernet_NetwokDown;
                multicolorLED.TurnBlue();
                Debug.Print("Ip address: " + ethernetJ11D.NetworkSettings.IPAddress);
                Debug.Print("MAC: " + ethernetJ11D.NetworkSettings.PhysicalAddress);
                while (ethernetJ11D.NetworkSettings.IPAddress == "0.0.0.0")
                {
                    Debug.Print("Waiting for DHCP");
                    Thread.Sleep(250);
                }

                new Thread(RunWebServer).Start();
            }

            void RunWebServer()
            {
                // Wait for the network...
                while (ethernetJ11D.IsNetworkUp == false)
                {
                    Debug.Print("Waiting...");
                    Thread.Sleep(1000);
                }
                // Start the server
                WebServer.StartLocalServer(ethernetJ11D.NetworkSettings.IPAddress, 80);
                WebServer.DefaultEvent.WebEventReceived += DefaultEvent_WebEventReceived;
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }

            void DefaultEvent_WebEventReceived(string path, WebServer.HttpMethod method, Responder
                responder)
            {
                // We always send the same page back
                responder.Respond(HTML, "text/html;charset=utf-8");
                // If a button was clicked
                if (method == WebServer.HttpMethod.POST)
                {
                    multicolorLED.TurnWhite();
                }
            }

            void ListNetworkInterfaces()
            {
                var settings = ethernetJ11D.NetworkSettings;

                Debug.Print("------------------------------------------------");
                Debug.Print("MAC: " + ByteExtensions.ToHexString(settings.PhysicalAddress, "-"));
                Debug.Print("IP Address:   " + settings.IPAddress);
                Debug.Print("DHCP Enabled: " + settings.IsDhcpEnabled);
                Debug.Print("Subnet Mask:  " + settings.SubnetMask);
                Debug.Print("Gateway:      " + settings.GatewayAddress);
                Debug.Print("------------------------------------------------");
            }

            

            void ethernet_NetworkUp(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
            {
                //Set the URI for the resource

                multicolorLED.TurnGreen();
                ListNetworkInterfaces();
                string ipAddress = ethernetJ11D.NetworkSettings.IPAddress;
                Debug.Print("indirizzo IP acquisito: " + ipAddress);
                //WebServer.StartLocalServer(ipAddress, 80);
                //sayHello = Gadgeteer.Networking.WebServer.SetupWebEvent("test");
                //sayHello.WebEventReceived += new WebEvent.ReceivedWebEventHandler(sayHello_WebEventReceived);
            }

            void sayHello_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
            {
                string content = "<html><body><h1>Vediamo se funziona</h1></body></html>";
                byte[] bytes = new System.Text.UTF8Encoding().GetBytes(content);
                //responder.Respond(bytes, "text/html");
                responder.Respond("Hello world");
            }

            void ethernet_NetwokDown(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
            {
                Debug.Print("IP " + ethernetJ11D.NetworkSettings.IPAddress);
                multicolorLED.TurnRed();
            }

            void request_ResponseReceived(HttpRequest sender, HttpResponse response)
            {
                if (response.StatusCode == "200")
                {
                    //This only works if the bitmap is the 
                    //same size as the screen it's flushing to
                    response.Picture.MakeBitmap().Flush();
                }
                else
                {
                    //Show a helpful error message
                    displayT35.SimpleGraphics.DisplayText("Request failed with status code " + response.StatusCode
                        , Resources.GetFont(Resources.FontResources.NinaB), Color.White, 0, 0);

                    displayT35.SimpleGraphics.DisplayText("Response text: " + response.Text
                           , Resources.GetFont(Resources.FontResources.NinaB), Color.White, 0, 50);

                }
            }
        }

        //    void ethernetENC28_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        //    {
        //        Debug.Print("Network Down");
        //    }

        //    void ethernetENC28_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        //    {
        //        Debug.Print("Network Up: " + ethernetJ11D.NetworkInterface.IPAddress);
        //    }

        //    void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        //    {
        //        lock (_lock)
        //        {
        //            Debug.Print("==============================================================");
        //            Debug.Print("NetworkChange_NetworkAddressChanged");
        //            Debug.Print("_isNetworkAvailable: " + _isNetworkAvailable);

        //            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        //            {
        //                Debug.Print("Interface   : " + GetNetworkInterfaceTypeName(item.NetworkInterfaceType));
        //                Debug.Print("IP          : " + item.IPAddress);
        //                Debug.Print("..............................................................");
        //            }

        //            Debug.Print("==============================================================");
        //        }
        //    }

        //    void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        //    {
        //        lock (_lock)
        //        {
        //            Debug.Print("--------------------------------------------------------------");
        //            Debug.Print("NetworkChange_NetworkAvailabilityChanged: " + e.IsAvailable);
        //            _isNetworkAvailable = e.IsAvailable;

        //            if (e.IsAvailable)
        //            {
        //                foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        //                {
        //                    Debug.Print("Interface   : " + GetNetworkInterfaceTypeName(item.NetworkInterfaceType));
        //                    Debug.Print("IP          : " + item.IPAddress);
        //                    Debug.Print("Mask        : " + item.SubnetMask);
        //                    Debug.Print("Gateway     : " + item.GatewayAddress);
        //                    Debug.Print("DCHP Enable : " + item.IsDhcpEnabled);
        //                    foreach (var dns in item.DnsAddresses)
        //                    {
        //                        Debug.Print("DNS         : " + dns);
        //                    }
        //                    Debug.Print("..............................................................");
        //                }
        //            }

        //            Debug.Print("--------------------------------------------------------------");
        //        }
        //    }

        //    private string GetNetworkInterfaceTypeName(NetworkInterfaceType t)
        //    {
        //        switch (t)
        //        {
        //            case NetworkInterfaceType.Unknown:
        //                return "Unknown";
        //            case NetworkInterfaceType.Ethernet:
        //                return "Ethernet";
        //            case NetworkInterfaceType.Wireless80211:
        //                return "Wireless80211";
        //        }

        //        return "NULL";
        //    }
        //}
        //public partial class Program
        //{
        //    // This method is run when the mainboard is powered up or reset.   
        //    void ProgramStarted()
        //    {
        //        Debug.Print("Program Started");
        //        DomoteerWebServer Ws = new DomoteerWebServer(ethernetJ11D, multicolorLED);
        //        Ws.initConnection(ethernetJ11D, multicolorLED);

        //        /*******************************************************************************************
        //        Modules added in the Program.gadgeteer designer view are used by typing 
        //        their name followed by a period, e.g.  button.  or  camera.

        //        Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
        //            button.ButtonPressed +=<tab><tab>

        //        If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
        //            GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
        //            timer.Tick +=<tab><tab>
        //            timer.Start();
        //        *******************************************************************************************/

        //        // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
        //        Debug.Print("Program Started");
        //    }
        //}
    }

