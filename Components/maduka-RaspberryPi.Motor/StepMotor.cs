using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maduka_RaspberryPi.Motor
{
    using Windows.Devices.Gpio;
    using static maduka_RaspberryPi.Motor.Enums;
    using static maduka_RaspberryPi.Motor.Consts;

    public class StepMotor
    {
        public int? Sleep { get; set; }

        private readonly GpioPin[] _gpioPins = new GpioPin[4];

        public StepMotor(int blueWireToGpio, int pinkWireToGpio, int yellowWireToGpio, int orangeWireToGpio)
        {
            var gpio = GpioController.GetDefault();

            _gpioPins[0] = gpio.OpenPin(blueWireToGpio);
            _gpioPins[1] = gpio.OpenPin(pinkWireToGpio);
            _gpioPins[2] = gpio.OpenPin(yellowWireToGpio);
            _gpioPins[3] = gpio.OpenPin(orangeWireToGpio);

            foreach (var gpioPin in _gpioPins)
            {
                gpioPin.Write(GpioPinValue.Low);
                gpioPin.SetDriveMode(GpioPinDriveMode.Output);
            }
        }

        public async Task TurnAsync(int degree, TurnDirection direction, DrivingMethod drivingMethod = DrivingMethod.FullStep)
        {
            if (this.Sleep == null)
                this.Sleep = 15;

            var steps = 0;
            GpioPinValue[][] methodSequence;
            switch (drivingMethod)
            {
                case DrivingMethod.WaveDrive:
                    methodSequence = ConstWaveDriveSequence;
                    steps = (int)Math.Ceiling(degree / 0.1767478397486253);
                    break;
                case DrivingMethod.FullStep:
                    methodSequence = ConstFullStepSequence;
                    steps = (int)Math.Ceiling(degree / 0.1767478397486253);
                    break;
                case DrivingMethod.HalfStep:
                    methodSequence = ConstHaveStepSequence;
                    steps = (int)Math.Ceiling(degree / 0.0883739198743126);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drivingMethod), drivingMethod, null);
            }

            var counter = 0;
            while (counter < steps)
            {
                for (var j = 0; j < methodSequence[0].Length; j++)
                {
                    for (var i = 0; i < 4; i++)
                        _gpioPins[i].Write(methodSequence[direction == TurnDirection.Left ? i : 3 - i][j]);

                    await Task.Delay((int)this.Sleep);
                    counter++;
                    if (counter == steps)
                        break;
                }
            }

            Stop();
        }

        public void Stop()
        {
            foreach (var gpioPin in _gpioPins)
            {
                gpioPin.Write(GpioPinValue.Low);
            }
        }

        public void Dispose()
        {
            foreach (var gpioPin in _gpioPins)
            {
                gpioPin.Write(GpioPinValue.Low);
                gpioPin.Dispose();
            }
        }
    }
}
