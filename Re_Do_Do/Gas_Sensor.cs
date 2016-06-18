using Gadgeteer.Modules.GHIElectronics;
using System;
using System.Threading;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace Re_Do_Do
{
    

    public enum gas_type
    {
        LPG = 1,
        CO = 2,
        SMOKE = 3,
    }

    public class Gas_Sensor
    {
        double[] LPGCurve = { 2.3, 0.21, -0.47 };
        double[] COCurve = { 2.3, 0.72, -0.34 };
        double[] SmokeCurve = { 2.3, 0.53, -0.44 };

        private readonly int RL = 25;
        public double R0;
        private readonly double maxV = 3.3;
        private readonly int CALIBRATION_SAMPLE_TIME = 10;
        private readonly int CALIBRATION_SAMPLE_INTERVAL = 500;
        private readonly int READ_SAMPLE_TIME = 10;
        private readonly int READ_SAMPLE_INTERVAL = 5;
        private readonly double R0_CLEAR_AIR_FACTOR = 9.83;
        private Gadgeteer.SocketInterfaces.AnalogInput a;

        public Gas_Sensor(Extender extender)
        {
            a = extender.CreateAnalogInput(GT.Socket.Pin.Five);
            R0 = GAS_Calibration();
        }


        public double GAS_Calibration()
        {
            double val = 0;
            for (int i = 0; i < CALIBRATION_SAMPLE_TIME; i++)
            {
                val += MQResistanceCalculation(a.ReadVoltage());
                Thread.Sleep(CALIBRATION_SAMPLE_INTERVAL);
            }

            val = val / CALIBRATION_SAMPLE_TIME;
            val = val / R0_CLEAR_AIR_FACTOR;
            return val;
        }

        private double MQResistanceCalculation(double raw_V)
        {
            return RL * ((maxV - raw_V) / raw_V);
        }


        public double MQRead()
        {
            double val = 0;
            for (int i = 0; i < READ_SAMPLE_TIME; i++)
            {
                val += MQResistanceCalculation(a.ReadVoltage());
                Thread.Sleep(READ_SAMPLE_INTERVAL);
            }

            val = val / READ_SAMPLE_TIME;

            return val;
        }


        public double MQGetGasPercentage(double rs_ro_ratio, gas_type gas_id)
        {
            switch (gas_id)
            {
                case gas_type.LPG:
                    return MQGetPercentage(rs_ro_ratio, LPGCurve);
                case gas_type.CO:
                    return MQGetPercentage(rs_ro_ratio, COCurve);
                case gas_type.SMOKE:
                    return MQGetPercentage(rs_ro_ratio, SmokeCurve);
                default:
                    return -1;
            }
        }

        private double MQGetPercentage(double rs_ro_ratio, double[] curve)
        {
            return (Math.Pow(10, (((Math.Log(rs_ro_ratio) - curve[1]) / curve[2]) + curve[0])));
        }


    }
}
