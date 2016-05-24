using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.SocketInterfaces;
using Gadgeteer.SocketInterfaces;
using Microsoft.SPOT.Hardware;
using Gadgeteer;

namespace Re_Do_Do
{
    /// <summary>
    /// A TMP100_Prova module for Microsoft .NET Gadgeteer
    /// </summary>

    public class Sensore_Temperatura_43
    {
        private const byte I2C_ADDRESS = 0x4B;
        private const byte CONFIGURATION_REGISTER = 0x01;
        private const byte CONFIGURATION_WORD = 0x9A;
        private const byte TEMPERATURE_REGISTER = 0x00;
        private const byte T_HIGH_REGISTER = 0x03;
        private const byte T_LOW_REGISTER = 0x02;
        private const byte RESET_WORD = 0x00;
        private I2CBus i2cs;   

        ///<Summary>
        /// Inizilizza in sensore
        ///</Summary>
        public bool setup()
        {
            i2cs = Gadgeteer.SocketInterfaces.I2CBusFactory.Create(Socket.GetSocket(4, false, null, null), I2C_ADDRESS, 400, null);

            byte[] outBuffer = new byte[2] {CONFIGURATION_REGISTER , CONFIGURATION_WORD};
            if (i2cs.Write(outBuffer) == 1)
                return true;
            return false;
        }

        ///<Summary>
        /// Ritorna un oggetto Temperatura
        ///</Summary>
        public Temperatura getTemp()
        {
            Temperatura t = new Temperatura();

            byte[] RegisterNum = new byte[1] { TEMPERATURE_REGISTER };
            byte[] RegisterValue = new byte[2];

            i2cs.WriteRead(RegisterNum, RegisterValue);
            Debug.Print("MSB: " + RegisterValue[0] + " LSB: " + RegisterValue[1]);
            t.setMSB(RegisterValue[0]); //Primo Byte MSB1
            t.setLSB(RegisterValue[1]);  //Secondo Byte LSB

            byte[] ResetValue = new byte[1] { RESET_WORD };
            i2cs.Write(ResetValue);
            return t;
        } 
    }
}
