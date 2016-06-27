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
        private Gadgeteer.SocketInterfaces.InterruptInput interrupt;
        private Gadgeteer.SocketInterfaces.DigitalIO dig_io;
        public PIR_Module(Extender extender)
        {
            this.extender = extender;
            //Controllare BENE i pin, deve essere quello con interrupt
            interrupt = extender.CreateInterruptInput(GT.Socket.Pin.Three, GT.SocketInterfaces.GlitchFilterMode.Off, GT.SocketInterfaces.ResistorMode.Disabled, GT.SocketInterfaces.InterruptMode.RisingEdge);
            interrupt.Interrupt += new GT.SocketInterfaces.InterruptEventHandler(mov_det);
            //dig_io = extender.CreateDigitalIO(GT.Socket.Pin.Three, false, GT.SocketInterfaces.GlitchFilterMode.On, GT.SocketInterfaces.ResistorMode.PullUp);

            //    GT.Timer tim = new GT.Timer(10);
            //  tim.Tick += new GT.Timer.TickEventHandler(tim_tick);
            //tim.Start();
        }

        //private void tim_tick(GT.Timer timer)
        //{
        //    if (dig_io.Read())
        //        Debug.Print("Movimento Rilevato");
        //    dig_io.Write(false);
        //}

        public void mov_det(GT.SocketInterfaces.InterruptInput input, bool value)
        {

            if (value)
            {
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

                if (Int32.Parse(init_time[0]) > Int32.Parse(end_time[0]))
                {
                    //Ora tra init_time[0] e 23.59 e tra 00.00 e end_time[0]
                    if (((ora_vera > ora_start) && (ora_vera < 23)) || (ora_fine > 0) && (ora_vera < ora_fine))
                    {
                        Debug.Print("Movimento Rilevato");
                    }
                    else if (ora_vera == ora_start)
                    {
                        if (minuto_vero > minuto_start)
                        {
                            Debug.Print("Movimento Rilevato");
                        }
                    }

                    else if (ora_vera == ora_fine)
                        if (minuto_vero < minuto_fine)
                        {
                            Debug.Print("Movimento Rilevato");
                        }
                }
                else
                {
                    if (ora_vera > ora_start && ora_vera < ora_fine)
                    {
                        Debug.Print("Movimento Rilevato");
                    }
                    else if (ora_vera == ora_start)
                    {
                        if (minuto_vero > minuto_start)
                        {
                            Debug.Print("Movimento Rilevato");
                        }
                    }
                    else if (ora_vera == ora_fine)
                    {
                        if (minuto_vero < minuto_fine)
                        {
                            Debug.Print("Movimento Rilevato");
                        }
                    }
                }
            }


        }



    }
}
