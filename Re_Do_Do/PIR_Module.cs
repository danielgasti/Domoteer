using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using System;
using System.Threading;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace Re_Do_Do
{
    class PIR_Module
    {

        private Extender extender;
        private DomoteerWebServer server;
        private Gadgeteer.SocketInterfaces.InterruptInput interrupt;
        private Gadgeteer.SocketInterfaces.DigitalIO dig_io;
        private GT.Timer timer_cross;
        private int count;
        public PIR_Module(Extender extender, DomoteerWebServer server)
        {
            this.server = server;
            this.extender = extender;
            interrupt = extender.CreateInterruptInput(GT.Socket.Pin.Three, GT.SocketInterfaces.GlitchFilterMode.Off, GT.SocketInterfaces.ResistorMode.PullUp, GT.SocketInterfaces.InterruptMode.FallingEdge);
            interrupt.Interrupt += new GT.SocketInterfaces.InterruptEventHandler(mov_det);           

            timer_cross = new GT.Timer(20000);
            timer_cross.Tick += new GT.Timer.TickEventHandler(Timer_Cross_Tick);
        }


        public void mov_det(GT.SocketInterfaces.InterruptInput input, bool value)
        {

            if (!value)
            {


                if (count == 0)
                {
                    
                    timer_cross.Start();
                    count++;
                }
                else
                {
                    timer_cross.Restart();
                }


            }


        }

        private void Timer_Cross_Tick(GT.Timer timer)
        {
            
            count = 0;
            String[] times;
            String[] init_time;
            String[] end_time;
            Int32 ora_start;
            Int32 minuto_start;
            Int32 ora_fine;
            Int32 minuto_fine;
            int ora_vera = 1;
            int minuto_vero = 1;
            times = Program.time_sett.Split('/');
            init_time = times[0].Split('-');
            end_time = times[1].Split('-');
            ora_start = Int32.Parse(init_time[0]);
            minuto_start = Int32.Parse(init_time[1]);
            ora_fine = Int32.Parse(end_time[0]);
            minuto_fine = Int32.Parse(end_time[1]);

            DateTime startDate = DateTime.Now;


            if (Int32.Parse(init_time[0]) > Int32.Parse(end_time[0]))
            {

                //Ora tra init_time[0] e 23.59 e tra 00.00 e end_time[0]
                if (((ora_vera > ora_start) && (ora_vera < 23)) || (ora_fine > 0) && (ora_vera < ora_fine))
                {
                    Debug.Print("Movimento Rilevato");
                    server.PutCross(startDate.ToString("yyyyMMddHHmmss"));
                }
                else if (ora_vera == ora_start)
                {
                    if (minuto_vero > minuto_start)
                    {
                        Debug.Print("Movimento Rilevato");
                        server.PutCross(startDate.ToString("yyyyMMddHHmmss"));
                    }
                }

                else if (ora_vera == ora_fine)
                    if (minuto_vero < minuto_fine)
                    {
                        Debug.Print("Movimento Rilevato");
                        server.PutCross(startDate.ToString("yyyyMMddHHmmss"));
                    }
            }
            else
            {
                if (ora_vera > ora_start && ora_vera < ora_fine)
                {
                    Debug.Print("Movimento Rilevato");
                    server.PutCross(startDate.ToString("yyyyMMddHHmmss"));
                }
                else if (ora_vera == ora_start)
                {
                    if (minuto_vero > minuto_start)
                    {
                        Debug.Print("Movimento Rilevato");
                        server.PutCross(startDate.ToString("yyyyMMddHHmmss"));
                    }
                }
                else if (ora_vera == ora_fine)
                {
                    if (minuto_vero < minuto_fine)
                    {
                        Debug.Print("Movimento Rilevato");
                        server.PutCross(startDate.ToString("yyyyMMddHHmmss"));
                    }
                }
            }

            timer_cross.Stop();

        }
    }




}
