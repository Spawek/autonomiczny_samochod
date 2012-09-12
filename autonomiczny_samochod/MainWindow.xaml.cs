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
        }

        void SteeringWheelAngleRegulator_evNewSteeringWheelSettingCalculated(object sender, NewSteeringWheelSettingCalculateddEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_steeringAngle,
                        Convert.ToString(args.getSteeringWheelAngleSetting())
            );
        }

        void SpeedRegulator_evNewSpeedSettingCalculated(object sender, NewSpeedSettingCalculatedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_steeringSpeed,
                        Convert.ToString(args.getSpeedSetting())
            );
        }

        void CarComunicator_evSteeringWheelAngleInfoReceived(object sender, SteeringWheelAngleInfoReceivedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_currentAngle,
                        Convert.ToString(args.GetAngle())
            );
        }

        void Model_evTargetSteeringWheelAngleChanged(object sender, TargetSteeringWheelAngleChangedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_targetAngle,
                        Convert.ToString(args.GetTargetWheelAngle())
            );
        }

        void CarComunicator_evSpeedInfoReceived(object sender, SpeedInfoReceivedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val) 
                    => textBox.Text = val), 
                        textBlock_currentSpeed, 
                        Convert.ToString(args.GetSpeedInfo())
            );
        }

        void Model_evTargetSpeedChanged(object sender, TargetSpeedChangedEventArgs args)
        {
            this.Dispatcher.Invoke(
                new Action<TextBlock, string>((textBox, val)
                    => textBox.Text = val),
                        textBlock_targetSpeed,
                        Convert.ToString(args.GetTargetSpeed())
            );
        }


    }
}
