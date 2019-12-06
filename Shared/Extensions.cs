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
                case ControllerType.MotionPlusNunchuk:
                    return "Wiimote Plus & Nunchuk";
                case ControllerType.MotionPlusCC:
                    return "Wiimote Plus & Classic Controller";
                case ControllerType.Guitar:
                    return "Guitar";
                case ControllerType.Drums:
                    return "Drums";
                case ControllerType.TaikoDrum:
                    return "Taiko Drum";
                case ControllerType.TurnTable:
                    return "DJ Turn Table";
                case ControllerType.DrawTablet:
                    return "Drawsome Tablet";
                case ControllerType.PartiallyInserted:
                    return "Unknown";
            }

            return "Unknown";
        }
    }
}
