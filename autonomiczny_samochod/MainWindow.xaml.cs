using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace autonomiczny_samochod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CarController Controller { get; private set; }

        private System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();
        private const int TIMER_INTERVAL_IN_MS = 10;
        private TimeSpan timeFromProgramBeginning = TimeSpan.Zero;

        public MainWindow()
        {
            Controller = new CarController(this);

            InitializeComponent();

            Controller.Model.evTargetSpeedChanged += new TargetSpeedChangedEventHandler(Model_evTargetSpeedChanged);
            Controller.Model.CarComunicator.evSpeedInfoReceived += new SpeedInfoReceivedEventHander(CarComunicator_evSpeedInfoReceived);

            Controller.Model.evTargetSteeringWheelAngleChanged += new TargetSteeringWheelAngleChangedEventHandler(Model_evTargetSteeringWheelAngleChanged);
            Controller.Model.CarComunicator.evSteeringWheelAngleInfoReceived += new SteeringWheelAngleInfoReceivedEventHandler(CarComunicator_evSteeringWheelAngleInfoReceived);

            Controller.Model.SpeedRegulator.evNewSpeedSettingCalculated += new NewSpeedSettingCalculatedEventHandler(SpeedRegulator_evNewSpeedSettingCalculated);
            Controller.Model.SteeringWheelAngleRegulator.evNewSteeringWheelSettingCalculated += new NewSteeringWheelSettingCalculatedEventHandler(SteeringWheelAngleRegulator_evNewSteeringWheelSettingCalculated);

            //initialize timer
            mTimer.Interval = TIMER_INTERVAL_IN_MS;
            mTimer.Tick += new EventHandler(mTimer_Tick);
            mTimer.Start();
        }

        void mTimer_Tick(object sender, EventArgs e)
        {
            timeFromProgramBeginning += TimeSpan.FromMilliseconds(TIMER_INTERVAL_IN_MS);
            textBlock_time.Text = String.Format(@"{0:mm\:ss\:ff}", timeFromProgramBeginning);
        }

        void SteeringWheelAngleRegulator_evNewSteeringWheelSettingCalculated(object sender, NewSteeringWheelSettingCalculateddEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_steeringAngle,
                        String.Format("{0:0.###}", args.getSteeringWheelAngleSetting())
            );
        }

        void SpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_steeringSpeed,
                        String.Format("{0:0.###}", args.getSpeedSetting())
            );
        }

        void CarComunicator_evSteeringWheelAngleInfoReceived(object sender, SteeringWheelAngleInfoReceivedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_currentAngle,
                        String.Format("{0:0.###}", args.GetAngle())
            );
        }

        void Model_evTargetSteeringWheelAngleChanged(object sender, TargetSteeringWheelAngleChangedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_targetAngle,
                        String.Format("{0:0.###}", args.GetTargetWheelAngle())
            );
        }

        void CarComunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val) 
                    => textBox.Text = val), 
                        textBlock_currentSpeed, 
                        String.Format("{0:0.###}", args.GetSpeedInfo())
            );
        }

        void Model_evTargetSpeedChanged(object sender, TargetSpeedChangedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_targetSpeed,
                        String.Format("{0:0.###}", args.GetTargetSpeed())
            );
        }
    }
}
