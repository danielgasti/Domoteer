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
            private object GetTemperaturesWS;

            void ProgramStarted()
            {
                #region SENSORE TEMPERATURA
                Sensore_Temperatura_43 s = new Sensore_Temperatura_43();
                s.setup();
                Temperatura t = s.getTemp();
                double temperatureValor = t.BinToCelsius();
                HtmlIndex.temperatura = temperatureValor.ToString();
                #endregion

                #region SENSORE GAS
                double gasValor = 10;
                HtmlIndex.gas = gasValor.ToString();
                #endregion

                #region SENSORE INFRAROSSI
                double passValor = 10;
                HtmlIndex.passaggi = passValor.ToString();
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
                button.ButtonPressed += new GTM.GHIElectronics.Button.ButtonEventHandler(GetTemperatures);

            }

            private void GetTemperatures(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
            {
                Debug.Print("Button pressed");
                server.GetTemperatures("4");
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

