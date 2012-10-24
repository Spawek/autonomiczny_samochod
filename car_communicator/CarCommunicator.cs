using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace car_communicator
{
    static class Const
    {
        public const int GEARBOX_CHANNEL = 0;
        public const int THROTTLE_CHANNEL = 1;
        public const int BREAK_CHANNEL = 0;
        public const int WHEEL_CHANNEL = 1;

        public const int MAX_THROTTLE = 7000;
        public const int MIN_THROTTLE = 3900;
        public const double MAX_BRAKE = 5.00;
        public const double MIN_BRAKE = 0.00;
        public const double WHEEL_MIN = 1.5;
        public const double WHEEL_MID = 2.5;
        public const double WHEEL_MAX = 3.5;
        public const int GEAR_P = 4000;
        public const int GEAR_R = 5500;
        public const int GEAR_N = 6500;
        public const int GEAR_D = 7800;
    }

    class Program
    {
        static public int starting_throttle_pos = 3900; //servo min value
        static public char starting_gear = 'n'; //servo
        static public double starting_wheel_pos = 2.5; // kierownica
        static public double starting_break_pos = 0; // hamulec

        static public USB4702 ExtendCard = new USB4702();
        static public ServoDriver Servo = new ServoDriver();

        static void Initialize()
        {
            //initialize all devices
            ExtendCard.Initialize();
            Servo.Initialize();

            //STARTING VALUES
            set_speed(starting_throttle_pos); // it will be the minimum value of speed
            set_wheel_position(starting_wheel_pos);
            set_gear(starting_gear);
            set_break_position(starting_break_pos);

        }


        static void set_gear(char gearToSet)
        {
            switch (gearToSet)
            {
                case 'p':
                    Servo.setTarget(0, Const.GEAR_P);
                    starting_gear = gearToSet;
                    break;

                case 'r':
                    Servo.setTarget(0, Const.GEAR_R);
                    starting_gear = gearToSet;
                    break;

                case 'n':
                    Servo.setTarget(0, Const.GEAR_N);
                    starting_gear = gearToSet;
                    break;

                case 'd':
                    Servo.setTarget(0, Const.GEAR_D);
                    starting_gear = gearToSet;
                    break;
            }
            //bieg = gear;
        }
        static void set_wheel_position(double pos)
        {
            // PID REGULATOR
            ExtendCard.setPortAO(Const.WHEEL_CHANNEL, pos);
        }

        static void set_break_position(double pod)
        {
            // PID REGULATOR
            ExtendCard.setPortDO(0, 0); // enable
            ExtendCard.setPortDO(1, 0); // direction
            ExtendCard.setPortAO(1, 0); //value
        }
        static void set_speed(int speed)
        {
            if (speed > Const.MAX_THROTTLE)
            {
                Servo.setTarget(Const.THROTTLE_CHANNEL, (ushort)Const.MAX_THROTTLE);
            }
            else
            {
                if (speed < Const.MIN_THROTTLE)
                {
                    Servo.setTarget(Const.THROTTLE_CHANNEL, (ushort)Const.MIN_THROTTLE);
                }
                else
                {
                    Servo.setTarget(Const.THROTTLE_CHANNEL, (ushort)speed);
                }
            }
        }
        /*
        //############################################################################//
        // for simulation
        static void StartSimulator()
        {
            Status();

            ConsoleKeyInfo key;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;
            do
            {
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.S)
                {
                    //max left
                    steer_left(50);
                }
                if (key.Key == ConsoleKey.D)
                {
                    //mid left 
                    steer_left(20);
                    
                }
                if (key.Key == ConsoleKey.F)
                {
                    // left
                    steer_left(7);
                }
                if (key.Key == ConsoleKey.H)
                {
                    //rigt
                    steer_right(7);
                }
                if (key.Key == ConsoleKey.J)
                {
                    //mid right
                    steer_right(20);
                }
                if (key.Key == ConsoleKey.K)
                {
                    //max right
                    steer_right(50);
                }
                if (key.Key == ConsoleKey.G)
                {
                    // kierownica prosto
                    first_ao = Const.WHEEL_MID;
                }
                if (key.Key == ConsoleKey.Y)
                {
                    // speed up light
                    speed_up(3);
                }
                if (key.Key == ConsoleKey.T)
                {
                    speed_down(3);
                    // speed up 
                }
                if (key.Key == ConsoleKey.B)
                {
                    // slow down light
                    speed_up(20);
                }
                if (key.Key == ConsoleKey.V)
                {
                    // slow down
                    speed_down(20);
                }
                if (key.Key == ConsoleKey.Spacebar)
                {
                    speed_down(1);
                    // stop
                }
                if (key.Key == ConsoleKey.D1)
                {
                    change_gear('r');
                }
                if (key.Key == ConsoleKey.D2)
                {
                    change_gear('n');
                }
                if (key.Key == ConsoleKey.D3)
                {
                    change_gear('d');
                }
                Console.Clear();
                Status();

            } while (key.Key != ConsoleKey.Escape);
            stop();
        }
        static void speed_up(int step)
        {
            // Check if servo is connected to the first channel
            int change = (Const.MAX_THROTTLE - Const.MIN_THROTTLE) / 100 * step;
            if (przepustnica + change > Const.MAX_THROTTLE)
            {
                przepustnica = Const.MAX_THROTTLE;
            }
            else
            {
                przepustnica += change;
               Servo.setTarget(1,(ushort) przepustnica);
            }
       
        }
        static void speed_down(int step)
        {
            // Check if servo is connected to the first channel
            int change = (Const.MAX_THROTTLE - Const.MIN_THROTTLE)/100 * step;
            
            if (przepustnica - change < Const.MIN_THROTTLE)
            {
                przepustnica = Const.MIN_THROTTLE;
            }
            else
            {
                przepustnica -=(int) change;
                Servo.setTarget(1, (ushort)przepustnica);
            }
        }
        
        static void steer_left(double step)
        {
            
            double change = (Const.WHEEL_MID - Const.WHEEL_MIN) / 100 * step;
            if (first_ao - change < Const.WHEEL_MIN)
            {
                first_ao = Const.WHEEL_MIN;
                ExtendCard.setPortAO(0, first_ao);
            }
            else
            {
                first_ao -= change;
                ExtendCard.setPortAO(0, first_ao);
            }
        }       
        static void steer_right(int step)
        {
            double change = (Const.WHEEL_MAX - Const.WHEEL_MID) / 100 * step;
            if (first_ao + change > Const.WHEEL_MAX)
            {
                first_ao = Const.WHEEL_MAX;
                ExtendCard.setPortAO(0, first_ao);
            }
            else
            {
                first_ao += change;
                ExtendCard.setPortAO(0, first_ao);
            }
        }
        static void Status()
        {
            int i;
            Console.WriteLine("                WHEEL POSITION");
            double wheel_pos = (first_ao - 1.5) / (Const.WHEEL_MAX - Const.WHEEL_MIN) * 100 / 2;
            Console.WriteLine("########################^#########################");

            for (i = 0; i < wheel_pos - 1; i++)
            {
                Console.Write(" ");
            }
            Console.WriteLine("0");

            Console.WriteLine("########################^#########################");


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(przepustnica);
            Console.WriteLine("                     THROTTLE");
            double throttle_pos = ((double)przepustnica - Const.MIN_THROTTLE) / ((double)Const.MAX_THROTTLE - Const.MIN_THROTTLE) * 100 / 2;
            //Console.WriteLine(((double) przepustnica) / ((double) Const.MAX_THROTTLE) * 100 / 2);
            Console.WriteLine(throttle_pos);
            Console.WriteLine("__________________________________________________");

            for (i = 0; i < throttle_pos - 1; i++)
            {
                Console.Write("#");
            }
            Console.WriteLine("V");

            Console.WriteLine("__________________________________________________");
            Console.WriteLine("0%        25%          50%         75%        100%");

        }
        //############################################################################//
        */


        static void Main(string[] args)
        {
            Initialize();

            while (true)
            {
                ExtendCard.RestartCounter();
                System.Threading.Thread.Sleep(1000);
                //droga=ExtendCard.getCounterStatus()*0.37;
                //Console.Write(droga * 2 * 36 /10);
                //Console.WriteLine("km / h");
                Console.WriteLine(ExtendCard.getCounterStatus());
            }

        }
    }

}
