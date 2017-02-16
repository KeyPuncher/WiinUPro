using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiinUPro
{
    public class VJoyAxisAssignment : IAssignment
    {
        public uint DeviceId { get; set; }
        public HID_USAGES Axis { get; set; }

        public VJoyAxisAssignment() { }

        public VJoyAxisAssignment(HID_USAGES axis, uint device = 1)
        {
            Axis = axis;
            DeviceId = device;
        }

        public void Apply(float value)
        {
            VJoyDirector.Access.SetAxis(Axis, value, DeviceId);
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
    }
}
