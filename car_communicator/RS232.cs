using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
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
        private const int SLEEP_ON_FAILED_PORT_OPPENING_BEFORE_NEXT_TRY_AT_APP_INIT_IN_MS = 1000; //to dont spam so many messages when it fails anyway
        private const int SLEEP_ON_FAILED_PORT_OPPENING_BEFORE_NEXT_TRY_AT_APP_WORKING_IN_MS = 0; //needed ASAP

        //read values
        int steeringWheelRead;
        int brakeRead;

        //just buffer
        char[] buffer = new char[4];

        //transmission thread
        System.Threading.Thread transsmissionThread;
        
        //sync
        System.Threading.Mutex initializationDone = new System.Threading.Mutex();

        public void Initialize()
        {
            // Attach a method to be called when there
            // is data waiting in the port's buffer
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.ReadTimeout = READ_TIMEOUT_IN_MS;
            port.WriteTimeout = WRITE_TIMEOUT_IN_MS;

            //thread start
            transsmissionThread = new System.Threading.Thread(new System.Threading.ThreadStart(startDataTransmission));
            transsmissionThread.Start();
        }

        /// <summary>
        /// note: must work as a new thread
        /// Datasheet in repo: "autonomiczny_samochod\docs\Dane techniczne - czujnik kąta.pdf"
        /// in case of any problems contact "Korad Zawada" and Electronics group
        /// </summary>
        private void startDataTransmission()
        {
            TryOppeningPortUntilItSucceds(SLEEP_ON_FAILED_PORT_OPPENING_BEFORE_NEXT_TRY_AT_APP_INIT_IN_MS);

            int loopsFromLastDiagnosis = LOOPS_BETWEEN_DIAGNOSIS; //so it will make diagnose at start

            while (true)
            {
                try
                {
                    if (loopsFromLastDiagnosis++ < LOOPS_BETWEEN_DIAGNOSIS)
                    {
                        SensorsRead();
                    }
                    else
                    {
                        DiagnoseSensors();
                        loopsFromLastDiagnosis = 0;
                    }
                }
                catch (TimeoutException)
                {
                    Logger.Log(this, "RS232 timout has occured", 1);
                    //TODO: some timout handling???
                }
            }
        }

        private void SensorsRead()
        {
            try
            {
                if (!port.IsOpen)
                {
                    Logger.Log(this, "RS232 port is closed - trying to reinitialize");
                    TryOppeningPortUntilItSucceds(SLEEP_ON_FAILED_PORT_OPPENING_BEFORE_NEXT_TRY_AT_APP_WORKING_IN_MS);
                }

                ReadSteeringWheelSensor(); //there 2 methods should remain 2 methods, because they can be customed in some way in future
                ReadBrakesSensors();
            }
            catch (Exception e)
            {
                Logger.Log(this, String.Format("RS232 communication error: \nMsg:{0} \nStackTrace:{1}", e.Message, e.StackTrace), 2);
                //TODO: errors handling
            }
        }

        private void ReadBrakesSensors()
        {
            port.Write(giveMeBrakeAngleMsg, 0, giveMeBrakeAngleMsg.Length);

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
                    if (buffer[0] == 'E') //check that condition
                    {
                        Logger.Log(this, "RS232 received an errror from brakes", 2);
                        //TODO: some handling?
                    }

                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    Logger.Log(this, String.Format("BRAKE - RS232 is desonchronised! Read is not done. Msg received: {0} {1} {2} {3}", (byte)buffer[0], (byte)buffer[1], (byte)buffer[2], (byte)buffer[3]), 1);
                }
            }
            System.Threading.Thread.Sleep(SLEEP_PER_READ_LOOP);
        }

        private void ReadSteeringWheelSensor()
        {
            port.Write(giveMeSteeringWheelAngleMsg, 0, giveMeSteeringWheelAngleMsg.Length);

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
                    if (buffer[0] == 'E') //check that condition
                    {
                        Logger.Log(this, "RS232 received an errror from steering wheel", 2);
                        //TODO: some handling?
                    }

                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    Logger.Log(this, String.Format("STEERING WHEEL - RS232 is desonchronised! Read is not done. Msg received: {0} {1} {2} {3}", (byte)buffer[0], (byte)buffer[1], (byte)buffer[2], (byte)buffer[3]), 1);
                    System.Threading.Thread.Sleep(SLEEP_ON_RS232_DESYNC_IN_MS);
                }
            }
        }

        private void DiagnoseSensors()
        {
            try
            {
                DiagnoseSteeringWheel();
                DiagnoseBrakes();
            }
            catch (Exception e)
            {
                Logger.Log(this, String.Format("RS232 communication error: \nMsg:{0} \nStackTrace:{1}", e.Message, e.StackTrace), 2);
                //TODO: errors handling
            }
        }

        private void DiagnoseSteeringWheel()
        {
            port.Write(giveMeSteeringWheelDiagnosisMsg, 0, giveMeSteeringWheelDiagnosisMsg.Length);

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

        private void DiagnoseBrakes()
        {
            port.Write(giveMeBrakeDiagnosisMsg, 0, giveMeBrakeDiagnosisMsg.Length);

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

        /// <summary>
        /// tries to open RS232 port, retries after "waitBeforeNextTryInMs" if failed
        /// </summary>
        /// <param name="waitBeforeNextTryInMs"></param>
        private void TryOppeningPortUntilItSucceds(int waitBeforeNextTryInMs)
        {
            try
            {
                port.Open();
            }
            catch (Exception)
            {
                Logger.Log(this, String.Format("RS232 port oppening failed, waiting {0}ms before next try", waitBeforeNextTryInMs), 2);
                Thread.Sleep(waitBeforeNextTryInMs);
                TryOppeningPortUntilItSucceds(waitBeforeNextTryInMs);
            }
        }

        private volatile string inBuffer = string.Empty;
        private volatile string receivedWord = string.Empty;

        /// <summary>
        /// its NOT THREAD-SAFE - cant work on different threads
        /// uses volatile vars "inBuffer" and "receivedWord"
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

        //http://stackoverflow.com/questions/5848907/received-byte-never-over-127-in-serial-port <--- use just read();
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string temp = port.ReadExisting(); //<-------- to nie dziala -> powyzej 128 daje 63
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

    }
}
