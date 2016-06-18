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

        public PIR_Module(Extender extender)
        {
            this.extender = extender;
            //Controllare BENE i pin, deve essere quello con interrupt
            interrupt = extender.CreateInterruptInput(GT.Socket.Pin.Three, GT.SocketInterfaces.GlitchFilterMode.Off, GT.SocketInterfaces.ResistorMode.Disabled, GT.SocketInterfaces.InterruptMode.RisingAndFallingEdge);
            interrupt.Interrupt += new GT.SocketInterfaces.InterruptEventHandler(mov_det);
        }

        public void mov_det(GT.SocketInterfaces.InterruptInput input, bool value)
        {
            if (value)
                Debug.Print("Movimento Rilevato");

            
        }


    }
}
