using System;
using System.IO.Ports;
using SerialPortExtension.SendReceive;

namespace ArduinoControl
{
    public class Arduino
    {
        private SerialPort serialPort;
        public bool isConnected { get; private set; } = false;
        public enum GPIO : int { p2 = 2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13 };
        public enum PWM : int { p3 = 3, p5 = 5, p6 = 6, p9 = 9, p10 = 10, p11 = 11 };
        public enum AIN : int { A0, A1, A2, A3, A4, A5 };
        public enum PinState : int { Low, High }

        ~Arduino()
        {
            Disconnect();
        }
        
        public void Connect()
        {
            if (isConnected)
                return;
            foreach (string port in SerialPort.GetPortNames())
            {
                serialPort = new SerialPort(port) { NewLine = "\r\n" };
                try
                {
                    serialPort.Open();
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    string response = serialPort.SendReceive("Connect?");
                    if (response.Contains("Connect!"))
                    {
                        isConnected = true;
                        return;
                    }
                    else
                        Disconnect();
                } catch { }
            }
            throw new Exception("Arduino Not Found");
        }

        public void Disconnect()
        {
            isConnected = false;
            serialPort?.Close();
            serialPort?.Dispose();
        }

        public void LedOn()
        {
            DigitalWrite(GPIO.p13, PinState.High);
        }

        public void LedOff()
        {
            DigitalWrite(GPIO.p13, PinState.Low);
        }

        public PinState DigitalRead(GPIO pin)
        {
            string request = "D" + Convert.ToUInt16(pin).ToString();
            string response = serialPort.SendReceive(request);
            int number = int.Parse(response);
            if (number > 0) { return PinState.High; }
            else { return PinState.Low; }
        }

        public double AnalogRead(AIN analogIn)
        {
            string request = "A" + Convert.ToUInt16(analogIn).ToString();
            string response = serialPort.SendReceive(request);
            double tenBitValue = double.Parse(response);
            double value = 5.0 * tenBitValue / 1023.0;
            return value;
        }

        public void DigitalWrite(GPIO pin, PinState value)
        {
            string request = "W" + Convert.ToUInt16(pin).ToString() + "," + Convert.ToUInt16(value).ToString();
            serialPort.SendReceive(request);
        }

        public void PWMWrite(PWM pin, byte value)
        {
            string request = "P" + Convert.ToUInt16(pin).ToString() + "," + value.ToString();
            string response = serialPort.SendReceive(request);
        }

        public string SpecialRequest(string requestKey)
        {
            string request = "S" + requestKey;
            return serialPort.SendReceive(request);
        }
    }

    public static class PinStateExtension
    {
        public static bool ToBool(this Arduino.PinState ps)
        {
            if (ps == Arduino.PinState.High)
                return true;
            else
                return false;
        }
    }
}
