
///<Summary>
/// Temperatura
///</Summary>
using System;
public class Temperatura
{
    private byte MSB;
    private byte LSB;
    ///<Summary>
    /// Instanzia un oggetto Temperatura
    /// Di default mette a 0°C
    ///</Summary>
    public Temperatura()
    {
        MSB = 0x00;
        LSB = 0x00;
    }
    ///<Summary>
    /// Imposta l'MSB
    ///</Summary>
    public void setMSB(byte msb)
    {
        MSB = msb;
    }
    ///<Summary>
    /// Ritorna l'MSB
    ///</Summary>
    public byte getMSB()
    {
        return MSB;
    }
    ///<Summary>
    /// Imposta l'LSB
    ///</Summary>
    public void setLSB(byte lsb)
    {
        LSB = lsb;
    }
    ///<Summary>
    /// Ritorna l'LSB
    ///</Summary>
    public byte getLSB()
    {
        return LSB;
    }

    ///<Summary>
    /// Converte i bit in gradi Celsius
    ///</Summary>

    public double BinToCelsius()
    {
        int celsius = Convert.ToInt32(this.MSB.ToString());
        if (LSB == 0x80)
        {
            return celsius + 0.5;
        }
        return celsius;
    }

}