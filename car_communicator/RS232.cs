using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

namespace RS232
{
    public class PidSensors
    {
        // Create the serial port with basic settings
        private SerialPort port = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);

        public void Initialize()
        {
            Console.WriteLine("Incoming Data:");

            // Attach a method to be called when there
            // is data waiting in the port's buffer
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

            // Begin communications
            port.Open();

            // Enter an application loop to keep this thread alive
            Application.Run();
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(port.ReadExisting());
        }

        /// <summary>
        /// TODO: odptytywanie przez RS w czasie rzeczywistym
        /// </summary>
        /// <returns></returns>
        public double get_wheel_position()
        //reutrns wheel pos from rs232 module
        {
            return 0.00;
        }

        /// <summary>
        /// TODO: the same
        /// </summary>
        /// <returns></returns>
        public double get_break_position()
        //reutrns break pos from rs232 module
        {
            return 0.00;
        }

    }
}
