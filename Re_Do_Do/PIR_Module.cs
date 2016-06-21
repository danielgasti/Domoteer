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

        private void tim_tick(GT.Timer timer)
        {
            if (dig_io.Read())
                Debug.Print("Movimento Rilevato");
            dig_io.Write(false);
        }

        public void mov_det(GT.SocketInterfaces.InterruptInput input, bool value)
        {
            if (value)
                Debug.Print("Movimento Rilevato");

        }



    }
}
