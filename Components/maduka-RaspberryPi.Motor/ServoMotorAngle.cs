using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maduka_RaspberryPi.Motor
{
    using Windows.Devices;
    using Windows.Devices.Pwm;
    using Microsoft.IoT.Lightning.Providers;

    public class ServoMotorAngle : IDisposable
    {
        public ServoMotorAngle(int servoPin)
        {
            if (LightningProvider.IsLightningEnabled)
            {
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
            }

            ServoPin = servoPin;
        }

        public int Frequency { get; set; } = 50;

        public double MaximumDutyCycle { get; set; } = 0.1;

        public double MinimumDutyCycle { get; set; } = 0.05;

        public int ServoPin { get; set; }

        public int SignalDuration { get; set; }

        private PwmPin ServoGpioPin { get; set; }

        public async Task Connect()
        {
            var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());

            if (pwmControllers != null)
            {
                // use the on-device controller
                var pwmController = pwmControllers[1];

                // Set the frequency, defaulted to 50Hz
                pwmController.SetDesiredFrequency(Frequency);

                ServoGpioPin = pwmController.OpenPin(ServoPin);
            }
        }

        public void Dispose()
        {
            ServoGpioPin?.Stop();
        }

        public void Go()
        {
            ServoGpioPin.Start();
            Task.Delay(SignalDuration).Wait();
            ServoGpioPin.Stop();
        }

        public void SetPosition(int degree)
        {
            ServoGpioPin?.Stop();

            // For example:
            // minimum duty cycle = 0.03 (0.6ms pulse in a period of 20ms) = 0 degrees
            // maximum duty cycle = 0.12 (2.4ms pulse in a period of 20ms) = 180 degrees
            // degree is between 0 and 180
            // => 0.0005 per degree [(0.12 - 0.03) / 180]

            var pulseWidthPerDegree = (MaximumDutyCycle - MinimumDutyCycle) / 180;

            var dutyCycle = MinimumDutyCycle + pulseWidthPerDegree * degree;
            ServoGpioPin.SetActiveDutyCyclePercentage(dutyCycle);
        }

        public void AllowTimeToMove(int pauseInMs)
        {
            this.SignalDuration = pauseInMs;
        }
    }
}
