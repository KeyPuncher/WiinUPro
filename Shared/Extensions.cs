using System;
using System.Collections.Generic;
using System.Text;
using NintrollerLib;

namespace Shared
{
    public static class Extensions
    {
        public static string ToName(this ControllerType controllerType)
        {
            switch (controllerType)
            {
                case ControllerType.Unknown:
                    return "Unknown";
                case ControllerType.Wiimote:
                    return "Wiimote";
                case ControllerType.ProController:
                    return "Pro Controlller";
                case ControllerType.BalanceBoard:
                    return "Balance Board";
                case ControllerType.Nunchuk:
                    return "Nunchuk";
                case ControllerType.NunchukB:
                    return "Nunchuk";
                case ControllerType.ClassicController:
                    return "Classic Controller";
                case ControllerType.ClassicControllerPro:
                    return "Classic Controller Pro";
                case ControllerType.MotionPlus:
                    return "Wiimote Plus";
                case ControllerType.Guitar:
                    return "Guitar";
                case ControllerType.Drums:
                    return "Drums";
                case ControllerType.TaikoDrum:
                    return "Taiko Drum";
                case ControllerType.PartiallyInserted:
                    return "Unknown";
            }

            return "Unknown";
        }
    }
}
