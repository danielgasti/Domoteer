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



            DomoteerWebServer server;

            void ProgramStarted()
            {

                //this.camera.CameraConnected += Camera_Connected;
                ////while (!camera.CameraReady)
                ////    Thread.Sleep(1000);
                //this.camera.PictureCaptured += Picture_Captured;
           

                server = new DomoteerWebServer(ethernetJ11D,multicolorLED,displayT35);
                server.initConnection();
                server.RunWebServer();

                Sensore_Temperatura_43 s = new Sensore_Temperatura_43();
                s.setup();
                Temperatura t = s.getTemp();
                double valor = t.BinToCelsius();
                


                

                
                
            }

            private void Picture_Captured(Camera sender, GT.Picture e)
            {
                displayT35.SimpleGraphics.Clear();
                displayT35.SimpleGraphics.DisplayImage(e, 0, 0);
            }

            private void Camera_Connected(Camera sender, EventArgs e)
            {
                sender.TakePicture();
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

