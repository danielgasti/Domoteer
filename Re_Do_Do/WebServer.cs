using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Networking;
using System.Threading;
using Gadgeteer;
using System.Text;
namespace Re_Do_Do
{
    class DomoteerWebServer
    {
        EthernetJ11D eth;
        MulticolorLED led;
        DisplayT35 display;

        byte[] HTML = Encoding.UTF8.GetBytes(
                "<html><body>" +
                "<h1>Hosted on .NET Gadgeteer</h1>" +
                "<p>Lets scare someone!</p>" +
                "<form action=\"\" method=\"post\">" +
                "<input type=\"submit\" value=\"Toggle LED!\">" +
                "</form>" +
                "</body></html>");

        GT.Networking.WebEvent sayHello;
        string ipAddress;
        
        public DomoteerWebServer(EthernetJ11D eth, MulticolorLED led, DisplayT35 display) 
        {
            this.eth = eth;
            this.led = led;
            this.display = display;
        }

        public void initConnection()
        {

            //NEW
            //The other option is UseStaticIP
            //ethernetJ11D.NetworkSettings.EnableDhcp();
            //ethernetJ11D.NetworkSettings.EnableDynamicDns();
            //eth.UseDHCP();

            //eth.NetworkSettings.EnableStaticIP("192.168.0.222", "255.255.255.0", "192.168.0.1");
            //eth.NetworkSettings.PhysicalAddress = ByteExtensions.ToHexByte("002103804AF0");

            //eth.UseStaticIP("192.168.1.222", "255.255.255.0", "192.168.1.254");

            //Set a handler for when the network is available
            eth.UseThisNetworkInterface();
            eth.NetworkSettings.EnableDhcp();
            //eth.UseDHCP();
            eth.NetworkSettings.EnableStaticIP("192.168.0.222", "255.255.255.0", "192.168.0.1");
            //ListNetworkInterfaces();

            eth.NetworkUp += ethernet_NetworkUp;
            eth.NetworkDown += ethernet_NetwokDown;
            led.TurnBlue();

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
            
            while (true)
            {
                Thread.Sleep(1000);
            }
        }


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

        void ethernet_NetworkUp(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
        {
            //Set the URI for the resource
            Debug.Print("Network up.");
            led.TurnGreen();
            ListNetworkInterfaces();
            
        }

        

        void ethernet_NetwokDown(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down.");
            led.TurnRed();
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
                display.SimpleGraphics.DisplayText("Request failed with status code " + response.StatusCode
                    , Resources.GetFont(Resources.FontResources.NinaB), Color.White, 0, 0);

                display.SimpleGraphics.DisplayText("Response text: " + response.Text
                       , Resources.GetFont(Resources.FontResources.NinaB), Color.White, 0, 50);

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
                led.TurnWhite();
               
            }
        }

        

    }
}
