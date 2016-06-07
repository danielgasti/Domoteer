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
            Font baseFont;
            Window window;
            Canvas canvas = new Canvas();
            bool first;
            Sensore_Temperatura_43 s;
            DomoteerWebServer server;
            private object GetTemperaturesWS;
            Gas_Sensor sens;
            GT.Timer timer;
            private string temp;
            private string lpg;
            private string smoke;
            private string co;
            public Text txtMsgTemp;
            public Text txtMsgLPG;
            public Text txtMsgCO;
            public Text txtMsgSMOKE;
            public Text txtMsg;

            void ProgramStarted()
            {
                first = true;

                #region SENSORE TEMPERATURA
                s = new Sensore_Temperatura_43();
                s.setup();
                Temperatura t = s.getTemp();
                double valor = t.BinToCelsius();
                #endregion

                #region SENSORE GAS
                sens = new Gas_Sensor(extender);
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

                #region TIMER

                timer = new GT.Timer(30000);
                timer.Tick += new GT.Timer.TickEventHandler(Timer_Tick);
                timer.Start();

                #endregion

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

                setupWindow();
            }

            private void Timer_Tick(GT.Timer timer)
            {
                Debug.Print("Button pressed");
                temp = s.getTemp().BinToCelsius().ToString();
                DateTime startDate = DateTime.Now;
                Debug.Print("Sending: " + temp + " - " + startDate.ToString("yyyyMMddHHmmss"));
                lpg = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.LPG).ToString();
                Debug.Print("Gpl: " + lpg);
                co = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.CO).ToString();
                Debug.Print("CO: " + co);
                smoke = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.SMOKE).ToString();
                Debug.Print("Smoke: " + smoke);

                //if (first)
                //{
                    updateValue();
                //    first = false;
                //}
                    
                
                
                server.pushData(temp, lpg, co, smoke, startDate.ToString("yyyyMMddHHmmss"));
            }

            private void GetTemperatures(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
            {


                Debug.Print("Button pressed");
                Temperatura t = s.getTemp();
                DateTime startDate = DateTime.Now;


                server.GetTemperatures("4");
                Debug.Print("Sending: " + t.BinToCelsius().ToString() + " - " + startDate.ToString("yyyyMMddHHmmss"));
                server.PutTemperatures(t.BinToCelsius().ToString(), startDate.ToString("yyyyMMddHHmmss"));

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


            private void setupWindow()
            {
                baseFont = Resources.GetFont(Resources.FontResources.NinaB);
                window = displayT35.WPFWindow;
                window.Child = canvas;
                txtMsgTemp = new Text(baseFont, "Starting…");
                txtMsgLPG = new Text(baseFont, "Starting…");
                txtMsgCO = new Text(baseFont, "Starting…");
                txtMsgSMOKE = new Text(baseFont, "Starting…");
                txtMsg = new Text(baseFont, "");
                canvas.SetMargin(5);
                txtMsgTemp.TextWrap = true;
                StackPanel stack = new StackPanel();
                stack.Children.Add(txtMsgTemp);
                stack.Children.Add(txtMsgLPG);
                stack.Children.Add(txtMsgCO);
                stack.Children.Add(txtMsgSMOKE);
                stack.Children.Add(txtMsg);
                canvas.Children.Add(stack);
                window.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(window_TouchDown);
            }

            //void window_TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
            //{
            //    if(!first)
            //        updateValue();
            //}

            private void updateValue()
            {
                txtMsgTemp.TextContent = "Temperature: " + temp + "°C";
                txtMsgLPG.TextContent = "LPG: " + lpg.Substring(0, 6) + " ppm";
                txtMsgCO.TextContent = "CO: " + co.Substring(0, 6) + " ppm";
                txtMsgSMOKE.TextContent = "Smoke: " + smoke.Substring(0, 6) + " ppm";
                txtMsg.TextContent = "Touch to Update values";
            }


            
        }
    }

