using System;
using System.IO.Ports;
using SerialPortExtension.SendReceive;

namespace ArduinoControl
{
    public enum PinState : int { Low, High }

    public static class PinStateExtension
    {
        public static bool ToBool(this PinState ps)
        {
            return ps == PinState.High;
        }
    }

    public class ArduinoUno : Arduino
    {
        public enum GPIOPin : int
        {
            p2 = 2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13,
            A0, A1, A2, A3, A4, A5
        }
        public enum PWMPin : int
        {
            p3 = 3, p5 = 5, p6 = 6, p9 = 9, p10 = 10, p11 = 11
        }
        public enum AnalogPin : int
        {
            A0, A1, A2, A3, A4, A5
        }

        public PinState DigitalRead(GPIOPin pin)
        {
            return DigitalRead((uint)pin);
        }

        public void DigitalWrite(GPIOPin pin, PinState value)
        {
            DigitalWrite((uint)pin, value);
        }

        public double AnalogRead(AnalogPin analogIn)
        {
            return AnalogRead((uint)analogIn);
        }

        public void PWMWrite(PWMPin pin, byte value)
        {
            PWMWrite((uint)pin, value);
        }
    }

    public class ArduinoMega : Arduino
    {
        public enum GPIOPin : uint
        {
            p2 = 2, p3, p4, p5, p6, p7, p8, p9,
            p10, p11, p12, p13, p14, p15, p16, p17, p18, p19,
            p20, p21, p22, p23, p24, p25, p26, p27, p28, p29,
            p30, p31, p32, p33, p34, p35, p36, p37, p38, p39,
            p40, p41, p42, p43, p44, p45, p46, p47, p48, p49,
            p50, p51, p52, p53,
            A0, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15
        }
        public enum PWMPin : uint
        {
            p2 = 2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13,
            p44 = 44, p45 = 45, p46 = 46
        }
        public enum AnalogPin : uint
        {
            A0, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15
        }

        public PinState DigitalRead(GPIOPin pin)
        {
            return DigitalRead((uint)pin);
        }

        public void DigitalWrite(GPIOPin pin, PinState value)
        {
            DigitalWrite((uint)pin, value);
        }

        public double AnalogRead(AnalogPin analogIn)
        {
            return AnalogRead((uint)analogIn);
        }

        public void PWMWrite(PWMPin pin, byte value)
        {
            PWMWrite((uint)pin, value);
        }
    }

    public abstract class Arduino
    {
        private SerialPort serialPort;
        private readonly string connectionResponse = "Arduino-C#-1.0.0";
        public bool isConnected { get; private set; } = false;

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
                serialPort = new SerialPort(port) { NewLine = "\r\n", ReadTimeout = 2000 };
                try
                {
                    serialPort.DtrEnable = true;
                    serialPort.Open();
                    serialPort.DtrEnable = false;
                    string response = serialPort.ReadLine();
                    if (response.Contains(connectionResponse))
                    {
                        isConnected = true;
                        serialPort.ReadTimeout = 200;
                        return;
                    }
                    else
                        Disconnect();
                }
                catch { }
            }
            throw new Exception("Arduino Not Found");
        }

        public void Disconnect()
        {
            isConnected = false;
            serialPort?.Close();
            serialPort?.Dispose();
            serialPort = null;
        }

        public void LedOn()
        {
            DigitalWrite(13, PinState.High);
        }

        public void LedOff()
        {
            DigitalWrite(13, PinState.Low);
        }

        public string SpecialRequest(string requestKey)
        {
            string request = "S" + requestKey;
            return serialPort.SendReceive(request);
        }

        protected PinState DigitalRead(uint pin)
        {
            string request = "D" + pin.ToString();
            string response = serialPort.SendReceive(request);
            int number = int.Parse(response);
            if (number > 0) { return PinState.High; }
            else { return PinState.Low; }
        }

        protected void DigitalWrite(uint pin, PinState value)
        {
            string request = "W" + pin.ToString() + "," + Convert.ToUInt16(value).ToString();
            serialPort.SendReceive(request);
        }

        protected double AnalogRead(uint analogIn)
        {
            string request = "A" + analogIn.ToString();
            string response = serialPort.SendReceive(request);
            double tenBitValue = double.Parse(response);
            double value = 5.0 * tenBitValue / 1023.0;
            return value;
        }

        protected void PWMWrite(uint pin, byte value)
        {
            string request = "P" + pin.ToString() + "," + value.ToString();
            string response = serialPort.SendReceive(request);
        }
    }
}