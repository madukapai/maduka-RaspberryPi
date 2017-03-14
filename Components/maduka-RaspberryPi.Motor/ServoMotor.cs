using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maduka_RaspberryPi.Motor
{
    using Windows.Devices.Gpio;
    using Windows.System.Threading;
    using System.Diagnostics;
    using System.Threading;
    using Windows.Foundation;

    public class ServoMotor
    {
        public GpioPin pin;
        public GpioPinValue pinValue;

        private IAsyncAction workItemThread;
        public GpioController gpio;

        public ServoMotor(int servoPin)
        {
            gpio = GpioController.GetDefault();
            pin = gpio.OpenPin(servoPin);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void PWM_R(int intAngle)
        {
            var stopwatch = Stopwatch.StartNew();
            intAngle = (intAngle * 300 / 90);

            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                    (source) =>
                    {
                        // setup, ensure pins initialized
                        ManualResetEvent mre = new ManualResetEvent(false);
                        mre.WaitOne(1000);

                        ulong pulseTicks = ((ulong)(Stopwatch.Frequency) / 1000) * 2;
                        ulong delta;
                        var startTime = stopwatch.ElapsedMilliseconds;
                        while (stopwatch.ElapsedMilliseconds - startTime <= intAngle)
                        {
                            pin.Write(GpioPinValue.High);
                            ulong starttick = (ulong)(stopwatch.ElapsedTicks);
                            while (true)
                            {
                                delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                                if (delta > pulseTicks) break;
                            }
                            pin.Write(GpioPinValue.Low);
                            starttick = (ulong)(stopwatch.ElapsedTicks);
                            while (true)
                            {
                                delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                                if (delta > pulseTicks * 10) break;
                            }
                        }
                    }, WorkItemPriority.High);
        }

        public void PWM_L(int intAngle)
        {
            intAngle = (intAngle * 300 / 90);

            var stopwatch = Stopwatch.StartNew();
            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                    (source) =>
                    {
                        // setup, ensure pins initialized
                        ManualResetEvent mre = new ManualResetEvent(false);
                        mre.WaitOne(1000);

                        ulong pulseTicks = ((ulong)(Stopwatch.Frequency) / 1000) * 2;
                        ulong delta;
                        var startTime = stopwatch.ElapsedMilliseconds;
                        while (stopwatch.ElapsedMilliseconds - startTime <= intAngle)
                        {
                            pin.Write(GpioPinValue.High);
                            ulong starttick = (ulong)(stopwatch.ElapsedTicks);
                            while (true)
                            {
                                delta = starttick - (ulong)(stopwatch.ElapsedTicks);
                                if (delta > pulseTicks) break;
                            }
                            pin.Write(GpioPinValue.Low);
                            starttick = (ulong)(stopwatch.ElapsedTicks);
                            while (true)
                            {
                                delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                                if (delta > pulseTicks * 10) break;
                            }
                        }
                    }, WorkItemPriority.High);
        }
    }
}
