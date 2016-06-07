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

            Sensore_Temperatura_43 s;
            DomoteerWebServer server;
            private object GetTemperaturesWS;
            Gas_Sensor sens;

            void ProgramStarted()
            {
                

                #region SENSORE TEMPERATURA
                s = new Sensore_Temperatura_43();
                s.setup();
                Temperatura t = s.getTemp();
                double valor = t.BinToCelsius();
                #endregion

                #region SENSORE GAS
                #endregion

                #region SENSORE INFRAROSSI
                #endregion

                #region CAMERA
                //this.camera.CameraConnected += Camera_Connected;
                ////while (!camera.CameraReady)
                ////    Thread.Sleep(1000);
                //this.camera.PictureCaptured += Picture_Captured;
                #endregion

                #region SERVER
                server = new DomoteerWebServer(ethernetJ11D, multicolorLED, displayT35, s);
                server.initConnection();
                server.RunWebServer();


                #endregion

                sens = new Gas_Sensor(extender);

                //while (true)
                //{
                //    double val = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.LPG);
                //    Debug.Print("Gpl: " + val);
                //    val = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.CO);
                //    Debug.Print("CO: " + val);
                //    val = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.SMOKE);
                //    Debug.Print("Smoke: " + val);
                //    Thread.Sleep(1000);
                //}
                
                


                button.ButtonPressed += new GTM.GHIElectronics.Button.ButtonEventHandler(GetTemperatures);

            }

            private void GetTemperatures(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
            {


                Debug.Print("Button pressed");
                Temperatura t = s.getTemp();
                DateTime startDate = DateTime.Now;


                //server.GetTemperatures("4");
                //Debug.Print("Sending: " + t.BinToCelsius().ToString() + " - " + startDate.ToString("yyyyMMddHHmmss"));
                //server.PutTemperatures(t.BinToCelsius().ToString(), startDate.ToString("yyyyMMddHHmmss"));

                double lpg = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.LPG);
                Debug.Print("Gpl: " + lpg);
                double co = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.CO);
                Debug.Print("CO: " + co);
                double smoke = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.SMOKE);
                Debug.Print("Smoke: " + smoke);


                server.PutGas(lpg.ToString(), co.ToString(), smoke.ToString(), startDate.ToString("yyyyMMddHHmmss"));
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
    }

