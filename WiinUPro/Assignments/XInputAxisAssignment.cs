using ScpControl;

namespace WiinUPro
{
    public class XInputAxisAssignment : IAssignment
    {
        /// <summary>
        /// The XInput Device to use
        /// </summary>
        public ScpDirector.XInput_Device Device { get; set; }

        /// <summary>
        /// The XInput Axis to be simulated.
        /// </summary>
        public X360Axis Axis { get; set; }

        public XInputAxisAssignment() { }

        public XInputAxisAssignment(X360Axis axis, ScpDirector.XInput_Device device = ScpDirector.XInput_Device.Device_A)
        {
            Axis = axis;
            Device = device;
        }

        public void Apply(float value)
        {
            ScpDirector.Access.SetAxis(Axis, value, Device);
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as XInputAxisAssignment;

            if (other == null)
            {
                return false;
            }

            bool result = true;

            result &= Axis == other.Axis;
            result &= Device == other.Device;

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as XInputAxisAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return Axis == other.Axis && Device == other.Device;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int)Axis + (int)Device;
            return hash;
        }

        public override string ToString()
        {
            return Axis.ToString();
        }

        public string GetDisplayName()
        {
            return $"X{ToString()}";
        }
    }
}
