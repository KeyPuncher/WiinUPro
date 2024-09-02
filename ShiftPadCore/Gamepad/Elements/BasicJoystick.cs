namespace ShiftPad.Core.Gamepad.Elements
{
    public class BasicJoystick : IGamepadElement
    {
        /// <summary>
        /// Normalized [-1.0,1.0] value for horizontal movement.
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Normalized [-1.0,1.0] value for vertical movement.
        /// </summary>
        public double Y { get; set; }

        public BasicJoystick()
        {

        }
    }
}
