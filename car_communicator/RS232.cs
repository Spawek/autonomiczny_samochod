using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using Helpers;

namespace autonomiczny_samochod
{
    public class RS232Controller
    {
        // Create the serial port with basic settings 
        private SerialPort port = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One); //TODO: add choosing COM no from form

        //messages
        char[] giveMeSteeringWheelAngleMsg = new char[] { '1', 'P', (char)13 };
        char[] giveMeBrakeAngleMsg = new char[] { '2', 'P', (char)13 };
        char[] giveMeSteeringWheelDiagnosisMsg = new char[] { '1', 'D', (char)13 };
        char[] giveMeBrakeDiagnosisMsg = new char[] { '2', 'D', (char)13 };

        //consts
        private TimeSpan SLEEP_PER_READ_LOOP = new TimeSpan(0, 0, 0, 0, 5); //5ms
        private const int LOOPS_BETWEEN_DIAGNOSIS = 100; //mby go more
        private const int READ_TIMEOUT_IN_MS = 100;
        private const int WRITE_TIMEOUT_IN_MS = 100;
        private const int SLEEP_ON_RS232_DESYNC_IN_MS = 10;
        private const int IN_BUFFER_SIZE = 100;
        private const int SLEEP_WHILE_WAITING_FOR_READ_IN_MS = 2;

        //read values
        int steeringWheelRead;
        int brakeRead;

        //just buffer
        char[] buffer = new char[4];

        //transmission thread
        System.Threading.Thread transsmissionThread;

        public void Initialize()
        {
            // Attach a method to be called when there
            // is data waiting in the port's buffer
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.ReadTimeout = READ_TIMEOUT_IN_MS;
            port.WriteTimeout = WRITE_TIMEOUT_IN_MS;
            

            // Begin communications
            try
            {
                port.Open();
            }
            catch (Exception)
            {
                Logger.Log(this, "RS232 initialization failed", 2);
                //throw;
                //TODO //IMPORTANT: temporary, commented
            }

            //thread start
            transsmissionThread = new System.Threading.Thread(new System.Threading.ThreadStart(startDataTransmission));
            transsmissionThread.Start();
        }

        /// <summary>
        /// note: must work as a new thread
        /// note: Datasheet somewhere in repo //TODO: where?!?
        /// </summary>
        private void startDataTransmission()
        {
            int loopsFromLastDiagnosis = LOOPS_BETWEEN_DIAGNOSIS;

            while(true)
            {
                try
                {
                    if (loopsFromLastDiagnosis++ >= LOOPS_BETWEEN_DIAGNOSIS)
                    { 
                        DiagnoseSensors();
                        loopsFromLastDiagnosis = 0;
                    }
                    else
                    {
                        SensorsRead();
                    }
                }
                catch (TimeoutException)
                {
                    Logger.Log(this, "RS232 timout has occured", 1);
                }
            }
        }

        private void SensorsRead()
        {
            try
            {
                if (!port.IsOpen)
                    port.Open();

                { //sterring wheel
                    port.Write(giveMeSteeringWheelAngleMsg, 0, giveMeSteeringWheelAngleMsg.Length);
                    //port.Read(buffer, 0, 4);
                    buffer = readWordFromRS232().ToCharArray();
                    if (buffer.Length != 4)
                    {
                        Logger.Log(this, String.Format("wrong received message length: {0}", buffer.Length, 1));
                    }
                    else
                    {
                        if (buffer[0] == 'A' && buffer[3] == 13)
                        {
                            brakeRead = (Convert.ToInt32(buffer[1])) * 64 + Convert.ToInt32(buffer[2]);
                            Logger.Log(this, String.Format("Steering wheel possition received from RS232: {0}", steeringWheelRead));
                            Logger.Log(this, String.Format("buff[0]: {0}, buff[1]: {1}, buff[2]: {2}, buff[3]: {3}", (byte)buffer[0], (byte)buffer[1], (byte)buffer[2], (byte)buffer[3]));
                        }
                        else
                        {
                            port.DiscardInBuffer();
                            port.DiscardOutBuffer();
                            Logger.Log(this, String.Format("STEERING WHEEL - RS232 is desonchronised! Read is not done. Msg received: {0} {1} {2} {3}", (byte)buffer[0], (byte)buffer[1], (byte)buffer[2], (byte)buffer[3]), 1);
                            System.Threading.Thread.Sleep(SLEEP_ON_RS232_DESYNC_IN_MS); 
                        }
                    }
                }
               
            
                { //brake
                    port.Write(giveMeBrakeAngleMsg, 0, giveMeBrakeAngleMsg.Length);
                    //port.Read(buffer, 0, 4);
                    buffer = readWordFromRS232().ToCharArray();
                    if (buffer.Length != 4)
                    {
                        Logger.Log(this, String.Format("wrong received message length: {0}", buffer.Length, 1));
                    }
                    else
                    {
                        if (buffer[0] == 'A' && buffer[3] == 13)
                        {
                            brakeRead = (Convert.ToInt32(buffer[1])) * 64 + Convert.ToInt32(buffer[2]);
                            Logger.Log(this, String.Format("Brake possition received from RS232: {0}", brakeRead));
                            Logger.Log(this, String.Format("buff[0]: {0}, buff[1]: {1}, buff[2]: {2}, buff[3]: {3}", (byte)buffer[0], (byte)buffer[1], (byte)buffer[2], (byte)buffer[3]));
                        }
                        else
                        {
                            port.DiscardInBuffer();
                            port.DiscardOutBuffer();
                            Logger.Log(this, String.Format("BRAKE - RS232 is desonchronised! Read is not done. Msg received: {0} {1} {2} {3}", (byte)buffer[0], (byte)buffer[1], (byte)buffer[2], (byte)buffer[3]), 1);
                        }
                    }
                    System.Threading.Thread.Sleep(SLEEP_PER_READ_LOOP);
                }

            }
            catch (Exception e)
            {
                Logger.Log(this, String.Format("RS232 communication error: {0} {1}", e.Message, e.StackTrace), 2);
                System.Threading.Thread.Sleep(100); //TEMPORARY
                //throw;
                //TODO: IMPORTANT: temporary commented
            }
        }

        private void DiagnoseSensors()
        {
            try
            {
                {//sterring wheel
                    port.Write(giveMeSteeringWheelDiagnosisMsg, 0, giveMeSteeringWheelDiagnosisMsg.Length);
                    //port.Read(buffer, 0, 4);
                    buffer = readWordFromRS232().ToCharArray();
                    if (buffer.Length != 4)
                    {
                        Logger.Log(this, String.Format("wrong received message length: {0}", buffer, 1));
                    }
                    else
                    {
                        if (buffer[0] == 0)
                        {
                            Logger.Log(this, "RS232 sterring wheel diagnosis bit 0 error", 1);
                        }
                        if (buffer[1] == 1)
                        {
                            Logger.Log(this, "RS232 sterring wheel diagnosis bit 1 error", 1);
                        }
                        if (buffer[2] == 1)
                        {
                            Logger.Log(this, "RS232 sterring wheel diagnosis bit 2 error - magnet is too strong or too close", 1);
                        }
                        if (buffer[3] == 1)
                        {
                            Logger.Log(this, "RS232 sterring wheel diagnosis bit 3 error - magnet is too weak or too far", 1);
                        }
                    }
                    Logger.Log(this, "RS232 Diagnosis done!");
                }

                {//brake
                    port.Write(giveMeBrakeDiagnosisMsg, 0, giveMeBrakeDiagnosisMsg.Length);
                    //port.Read(buffer, 0, 4);
                    buffer = readWordFromRS232().ToCharArray();
                    if (buffer.Length != 4)
                    {
                        Logger.Log(this, String.Format("wrong received message length: {0}", buffer, 1));
                    }
                    else
                    {
                        if (buffer[0] == 0)
                        {
                            Logger.Log(this, "RS232 brake diagnosis bit 0 error", 1);
                        }
                        if (buffer[1] == 1)
                        {
                            Logger.Log(this, "RS232 brake diagnosis bit 1 error", 1);
                        }
                        if (buffer[2] == 1)
                        {
                            Logger.Log(this, "RS232 brake diagnosis bit 2 error - magnet is too strong or too close", 1);
                        }
                        if (buffer[3] == 1)
                        {
                            Logger.Log(this, "RS232 brake diagnosis bit 3 error - magnet is too weak or too far", 1);
                        }
                    }
                }

            }
            catch (Exception)
            {
                Logger.Log(this, "RS232 communication error", 2);
                System.Threading.Thread.Sleep(100); //TEMPORARY
                //throw;
                //TODO: IMPORTANT: temporary commented
            }
        }

        private volatile string inBuffer = string.Empty;
        private volatile string receivedWord = string.Empty;

        /// <summary>
        /// its NOT THREAD-SAFE - cant work on different threads
        /// </summary>
        /// <returns></returns>
        private string readWordFromRS232()
        {
            while (receivedWord == string.Empty)
                System.Threading.Thread.Sleep(SLEEP_WHILE_WAITING_FOR_READ_IN_MS);

            string temp = receivedWord;
            receivedWord = string.Empty;

            return temp;
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string temp = port.ReadExisting(); //<-------- to nie dziala
            //int temp = port.ReadChar();
            Console.WriteLine(temp);
            //inBuffer = inBuffer + Convert.ToChar(temp);
            inBuffer = inBuffer + temp;
            if (inBuffer != string.Empty)
            {
                if (inBuffer[inBuffer.Length - 1] == 13)
                //if(temp == 13)
                {
                    if (receivedWord != string.Empty)
                    {
                        Logger.Log(this, String.Format("message from RS232 was not read: {0}", receivedWord), 1);
                    }
                    receivedWord = inBuffer;
                    inBuffer = string.Empty;
                }
            }
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
