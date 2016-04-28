using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Networking;
namespace Re_Do_Do
{
    class DomoteerWebServer
    {
        EthernetJ11D eth;
        MulticolorLED led;
        GT.Networking.WebEvent sayHello;
        string ipAddress;
        
        public DomoteerWebServer(EthernetJ11D eth, MulticolorLED led) 
        {
            this.eth = eth;
            this.led = led;
        }

        public void initConnection(EthernetJ11D eth, MulticolorLED led)
        {
            
            eth.NetworkSettings.EnableDhcp();
            eth.UseDHCP();
            Debug.Print("indirizzo IP acquisito: " + ipAddress);
            eth.NetworkDown += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkDown);
            eth.NetworkUp += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkUp);

        }


        //    eth.UseDHCP();
        //    eth.NetworkUp += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkUp);
        //}

        void ethernet_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            //debug visuale
            led.TurnGreen();
            string ipAddress = eth.NetworkSettings.IPAddress;
            Debug.Print("indirizzo IP acquisito: " + ipAddress);
            WebServer.StartLocalServer(ipAddress, 80);
            sayHello = WebServer.SetupWebEvent("test");
            sayHello.WebEventReceived += new WebEvent.ReceivedWebEventHandler(sayHello_WebEventReceived);
        }

        void sayHello_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            string content = "<html><body><h1>Vediamo se funziona</h1></body></html>";
            byte[] bytes = new System.Text.UTF8Encoding().GetBytes(content);
            responder.Respond(bytes, "text/html");
        }

        void ethernet_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            led.TurnRed();
        }


    }
}
