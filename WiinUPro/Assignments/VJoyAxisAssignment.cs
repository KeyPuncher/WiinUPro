namespace WiinUPro
{
    public class VJoyAxisAssignment : IAssignment
    {
        public uint DeviceId { get; set; }
        public HID_USAGES Axis { get; set; }
        public bool Positive { get; set; }

        public VJoyAxisAssignment() { }

        public VJoyAxisAssignment(HID_USAGES axis, bool positive = true, uint device = 1)
        {
            Axis = axis;
            Positive = positive;
            DeviceId = device;
        }

        public void Apply(float value)
        {
            VJoyDirector.Access.SetAxis(Axis, value, Positive, DeviceId);
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as VJoyAxisAssignment;

            if (other == null)
            {
                return false;
            }

            bool result = true;

            result &= Axis == other.Axis;
            result &= DeviceId == other.DeviceId;
            result &= Positive == other.Positive;

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as VJoyAxisAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return Axis == other.Axis && DeviceId == other.DeviceId;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int)Axis + (int)DeviceId;
            return hash;
        }

        public override string ToString()
        {
            return Axis.ToString();
        }

        public string GetDisplayName()
        {
            return ToString().Replace("HID_USAGE_", "Joy.");
        }
    }
}
