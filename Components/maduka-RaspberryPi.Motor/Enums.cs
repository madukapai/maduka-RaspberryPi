using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maduka_RaspberryPi.Motor
{
    public class Enums
    {
        public enum DrivingMethod
        {
            WaveDrive,
            FullStep,
            HalfStep,
        }

        public enum TurnDirection
        {
            Left,
            Right
        }
    }
}