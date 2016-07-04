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
        private Font baseFont;
        private Window window;
        private Canvas canvas = new Canvas();
        private Canvas c_sett = new Canvas();
        private bool first;
        private Sensore_Temperatura_43 s;
        private DomoteerWebServer server;
        private object GetTemperaturesWS;
        private Gas_Sensor sens;
        private PIR_Module pir;
        private GT.Timer timer_gas;
        private GT.Timer timer_PIR;
        private GT.Timer timer_display;
        private string temp;
        private string lpg;
        private string smoke;
        private string co;
        //public Text settingsMessage;
        public Text txtMsgTemp;
        public Text txtMsgLPG;
        public Text txtMsgCO;
        public Text txtMsgSMOKE;
        public Text txtMsg;
        public Text inithour;
        public Text initminute;
        public Text initHourEnd;
        public Text initMinuteEnd;
        public static String time_sett;


        void ProgramStarted()
        {
            first = true;
            time_sett = Resources.GetString(Resources.StringResources.PIR_Sensor);

            #region SENSORE TEMPERATURA
            s = new Sensore_Temperatura_43();
            s.setup();
            Temperatura t = s.getTemp();
            double valor = t.BinToCelsius();
            #endregion

            #region SERVER
            server = new DomoteerWebServer(ethernetJ11D, multicolorLED, displayT35, s);
            server.initConnection();
            server.RunWebServer();


            #endregion

            #region PIR

            pir = new PIR_Module(extender, server);

            #endregion

            #region SENSORE GAS
            sens = new Gas_Sensor(extender);
            #endregion

            #region TIMER_GAS
            timer_gas = new GT.Timer(20000);
            timer_gas.Tick += new GT.Timer.TickEventHandler(Timer_Gas_Tick);
            timer_gas.Start();

            #endregion

            #region TIMER_AGGIORNAMENTO_SCHERMO
            timer_display = new GT.Timer(5000);
            timer_display.Tick += new GT.Timer.TickEventHandler(Timer_Display_Tick);
            timer_display.Start();
            #endregion

            button.ButtonPressed += new GTM.GHIElectronics.Button.ButtonEventHandler(GetTemperatures);

            setupWindow();


        }

        private void Timer_Display_Tick(GT.Timer timer)
        {
            updateValue();
        }


        private void Timer_Gas_Tick(GT.Timer timer)
        {

            temp = s.getTemp().BinToCelsius().ToString();
            DateTime startDate = DateTime.Now;
            Debug.Print("Sending: " + temp + " - " + startDate.ToString("yyyyMMddHHmmss"));
            lpg = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.LPG).ToString();
            Debug.Print("Gpl: " + lpg);
            co = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.CO).ToString();
            Debug.Print("CO: " + co);
            smoke = sens.MQGetGasPercentage(sens.MQRead() / sens.R0, gas_type.SMOKE).ToString();
            Debug.Print("Smoke: " + smoke);

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

        private void setupWindow()
        {
            baseFont = Resources.GetFont(Resources.FontResources.NinaB);
            StackPanel stack = new StackPanel();
            canvas.SetMargin(5);

            StackPanel temperaturePanel = new StackPanel(Orientation.Horizontal);
            Image tempIcon = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.temperatura_ridotto), Bitmap.BitmapImageType.Jpeg));
            txtMsgTemp = new Text(baseFont, "Starting...");
            txtMsgTemp.TextWrap = true;
            txtMsgTemp.SetMargin(20, 20, 0, 0);
            tempIcon.SetMargin(12, 0, 0, 0);
            temperaturePanel.Children.Add(tempIcon);
            temperaturePanel.Children.Add(txtMsgTemp);
           

            StackPanel gasPanel = new StackPanel(Orientation.Horizontal);
            StackPanel gasData = new StackPanel(Orientation.Vertical);
            Image gasIcon = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.gas), Bitmap.BitmapImageType.Jpeg));
            txtMsgLPG = new Text(baseFont, "Starting...");
            txtMsgCO = new Text(baseFont, "Starting...");
            txtMsgSMOKE = new Text(baseFont, "Starting...");
            txtMsg = new Text(baseFont, "");
            gasIcon.SetMargin(7, 20, 0, 0);
            txtMsgLPG.SetMargin(12, 22, 0, 0);
            txtMsgCO.SetMargin(12, 2, 0, 0);
            txtMsgSMOKE.SetMargin(12, 2, 0, 0);
            
            gasData.Children.Add(txtMsgLPG);
            gasData.Children.Add(txtMsgCO);
            gasData.Children.Add(txtMsgSMOKE);
            
            gasPanel.Children.Add(gasIcon);
            gasPanel.Children.Add(gasData);
           


            StackPanel settingsPanel = new StackPanel(Orientation.Horizontal);
            Image settings = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.settings), Bitmap.BitmapImageType.Jpeg));
            //settingsMessage = new Text(baseFont, "Change Settings: ");
            settings.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(settings_navigator);
            //settingsMessage.SetMargin(5, 40, 0, 0);
            settings.SetMargin(8, 30, 0, 0);
            //settingsPanel.Children.Add(settingsMessage);
            settingsPanel.Children.Add(settings);
            
            
            
            window = displayT35.WPFWindow;
            window.Child = canvas;

            stack.Children.Add(txtMsg);
            stack.Children.Add(temperaturePanel);
            stack.Children.Add(gasPanel);
            stack.Children.Add(settingsPanel);

            canvas.Children.Add(stack);
        }


        private void updateValue()
        {
            txtMsgTemp.TextContent = "Temperature: " + temp + "°C";
            txtMsgLPG.TextContent = "LPG: " + lpg.Substring(0, 6) + " ppm";
            txtMsgCO.TextContent = "CO: " + co.Substring(0, 6) + " ppm";
            txtMsgSMOKE.TextContent = "Smoke: " + smoke.Substring(0, 6) + " ppm";
        }


        private void setSetupWindow()
        {
            String res;
            String[] times;
            String[] init_time;
            String[] end_time;
            if (time_sett == null)
                res = Resources.GetString(Resources.StringResources.PIR_Sensor);
            else
                res = time_sett;
            times = res.Split('/');
            init_time = times[0].Split('-');
            end_time = times[1].Split('-');
            window = displayT35.WPFWindow;
            window.Child = c_sett;
            baseFont = Resources.GetFont(Resources.FontResources.NinaB);

            StackPanel settings = new StackPanel(Orientation.Vertical);
            Text impostazioni = new Text(baseFont, "Settings");
            impostazioni.SetMargin(130, 2, 0, 10);
            settings.Children.Add(impostazioni);

            StackPanel content = new StackPanel(Orientation.Horizontal);


            #region inizio zona critica

            StackPanel left = new StackPanel(Orientation.Vertical);

            StackPanel labelStart = new StackPanel(Orientation.Vertical);
            Text labelS = new Text(baseFont, "Start");
            labelS.SetMargin(70, 25, 0, 5);
            left.Children.Add(labelS);

            StackPanel plus = new StackPanel(Orientation.Horizontal);
            Image plushour = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.plus), Bitmap.BitmapImageType.Jpeg));
            Image plusminute = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.plus), Bitmap.BitmapImageType.Jpeg));
            plushour.SetMargin(50, 20, 10, 0);
            plusminute.SetMargin(5, 20, 10, 0);
            plushour.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(increase_touch_hour);
            plusminute.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(increase_touch_minute);
            plus.HorizontalAlignment = HorizontalAlignment.Center;
            plus.Children.Add(plushour);
            plus.Children.Add(plusminute);


            StackPanel textBoxes = new StackPanel(Orientation.Horizontal);
            textBoxes.HorizontalAlignment = HorizontalAlignment.Center;
            inithour = new Text(baseFont, "Starting…");
            inithour.SetMargin(50, 20, 10, 0);
            initminute = new Text(baseFont, "Starting…");
            initminute.SetMargin(5, 20, 10, 0);
            inithour.TextContent = init_time[0];
            initminute.TextContent = init_time[1];
            textBoxes.Children.Add(inithour);
            textBoxes.Children.Add(initminute);


            StackPanel minus = new StackPanel(Orientation.Horizontal);
            minus.HorizontalAlignment = HorizontalAlignment.Center;
            Image minhour = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.minus), Bitmap.BitmapImageType.Jpeg));
            Image minminute = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.minus), Bitmap.BitmapImageType.Jpeg));
            minhour.SetMargin(50, 20, 10, 0);
            minminute.SetMargin(5, 20, 10, 0);
            minhour.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(decrease_touch_hour);
            minminute.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(decrease_touch_minute);
            minus.Children.Add(minhour);
            minus.Children.Add(minminute);


            StackPanel navigator_sx = new StackPanel(Orientation.Horizontal);
            Image save = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.save), Bitmap.BitmapImageType.Jpeg));
            save.SetMargin(42, 14, 10, 0);
            save.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(saveSettings);
            navigator_sx.Children.Add(save);

            #endregion

            left.Children.Add(plus);
            left.Children.Add(textBoxes);
            left.Children.Add(minus);
            left.Children.Add(navigator_sx);
            content.Children.Add(left);

            #region fine zona critica
            StackPanel right = new StackPanel(Orientation.Vertical);

            StackPanel labelEnd = new StackPanel(Orientation.Vertical);
            Text labelE = new Text(baseFont, "End");
            labelE.SetMargin(68, 25, 0, 5);
            right.Children.Add(labelE);

            StackPanel increaseEnd = new StackPanel(Orientation.Horizontal);
            Image increaseHourEnd = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.plus), Bitmap.BitmapImageType.Jpeg));
            Image increaseMinuteEnd = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.plus), Bitmap.BitmapImageType.Jpeg));
            increaseHourEnd.SetMargin(22, 20, 10, 0);
            increaseMinuteEnd.SetMargin(5, 20, 10, 0);
            increaseHourEnd.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(increase_touch_hour_end);
            increaseMinuteEnd.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(increase_touch_minute_end);
            increaseEnd.HorizontalAlignment = HorizontalAlignment.Center;
            increaseEnd.Children.Add(increaseHourEnd);
            increaseEnd.Children.Add(increaseMinuteEnd);


            StackPanel textBoxesEnd = new StackPanel(Orientation.Horizontal);
            textBoxesEnd.HorizontalAlignment = HorizontalAlignment.Center;
            initHourEnd = new Text(baseFont, "Starting…");
            initHourEnd.SetMargin(22, 20, 10, 0);
            initMinuteEnd = new Text(baseFont, "Starting…");
            initMinuteEnd.SetMargin(5, 20, 10, 0);
            initHourEnd.TextContent = end_time[0];
            initMinuteEnd.TextContent = end_time[1];
            textBoxesEnd.Children.Add(initHourEnd);
            textBoxesEnd.Children.Add(initMinuteEnd);


            StackPanel decreaseEnd = new StackPanel(Orientation.Horizontal);
            decreaseEnd.HorizontalAlignment = HorizontalAlignment.Center;
            Image decreaseHourEnd = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.minus), Bitmap.BitmapImageType.Jpeg));
            Image decreaseMinuteEnd = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.minus), Bitmap.BitmapImageType.Jpeg));
            decreaseHourEnd.SetMargin(22, 20, 10, 0);
            decreaseMinuteEnd.SetMargin(5, 20, 10, 0);
            decreaseHourEnd.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(decrease_touch_hour_end);
            decreaseMinuteEnd.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(decrease_touch_minute_end);
            decreaseEnd.Children.Add(decreaseHourEnd);
            decreaseEnd.Children.Add(decreaseMinuteEnd);

            StackPanel navigator_dx = new StackPanel(Orientation.Horizontal);
            Image back = new Image(new Bitmap(Resources.GetBytes(Resources.BinaryResources.back), Bitmap.BitmapImageType.Jpeg));
            back.SetMargin(30, 14, 10, 0);
            back.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(goBack);
            navigator_dx.Children.Add(back);
            #endregion

            right.Children.Add(increaseEnd);
            right.Children.Add(textBoxesEnd);
            right.Children.Add(decreaseEnd);
            right.Children.Add(navigator_dx);
            content.Children.Add(right);

            c_sett.Children.Add(settings);
            c_sett.Children.Add(content);


        }



        void increase_touch_hour(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            Int32 hour = Int32.Parse(inithour.TextContent) + 1;
            if(hour >= 10)
                inithour.TextContent = hour % 24 + "";
            else
                inithour.TextContent = "0" + hour % 24;
        }


        void increase_touch_minute(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            Int32 minute = Int32.Parse(initminute.TextContent) + 1;
            if(minute%60 >= 10)
                initminute.TextContent = minute % 60 + "";
            else
                initminute.TextContent = "0" + minute % 60;

        }


        void decrease_touch_hour(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            if (Int32.Parse(inithour.TextContent) == 0)
                inithour.TextContent = 23 + "";
            else
            {
                Int32 hour = Int32.Parse(inithour.TextContent) - 1;
                if (hour >= 10)
                    inithour.TextContent = hour + "";
                else
                    inithour.TextContent = "0" + hour;
            }
        }


        void decrease_touch_minute(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            if (Int32.Parse(initminute.TextContent) == 0)
                initminute.TextContent = 59 + "";
            else
            {
                Int32 hour = Int32.Parse(initminute.TextContent) - 1;
                if(hour < 10)
                    initminute.TextContent = "0" + hour;
                else
                    initminute.TextContent = hour + "";
            }
        }


        void increase_touch_hour_end(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            Int32 hour = Int32.Parse(initHourEnd.TextContent) + 1;
            if(hour%24 >= 10)
                initHourEnd.TextContent = hour % 24 + "";
            else
                initHourEnd.TextContent = "0" + hour % 24;
        }


        void increase_touch_minute_end(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            Int32 minute = Int32.Parse(initMinuteEnd.TextContent) + 1;
            if(minute%60 >= 10)
                initMinuteEnd.TextContent = minute % 60 + "";
            else
                initMinuteEnd.TextContent = "0" + minute % 60;
        }


        void decrease_touch_hour_end(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            if (Int32.Parse(initHourEnd.TextContent) == 0)
                initHourEnd.TextContent = 23 + "";
            else
            {
                Int32 hour = Int32.Parse(initHourEnd.TextContent) - 1;
                if(hour >= 10)
                    initHourEnd.TextContent = hour + "";
            }
        }


        void decrease_touch_minute_end(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            if (Int32.Parse(initMinuteEnd.TextContent) == 0)
                initMinuteEnd.TextContent = 59 + "";
            else
            {
                Int32 hour = Int32.Parse(initMinuteEnd.TextContent) - 1;
                if(hour >= 10)
                    initMinuteEnd.TextContent = hour + "";
                else
                    initMinuteEnd.TextContent = "0" + hour;
            }
        }



        void settings_navigator(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            setSetupWindow();
        }


        private void goBack(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            setupWindow();
        }


        private void saveSettings(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {

            //Tipo di salvataggio ---> 11-00/23-50
            time_sett = inithour.TextContent + "-" + initminute.TextContent + "/" + initHourEnd.TextContent + "-" + initMinuteEnd.TextContent;
            setupWindow();
        }


    }
}

