using System.Collections.Generic;

namespace WiinUPro
{
    public class AssignmentProfile
    {
        public List<AssignmentPair> MainAssignments;
        public List<AssignmentPair> RedAssignments;
        public List<AssignmentPair> BlueAssignments;
        public List<AssignmentPair> GreenAssignments;
        public AssignmentProfile SubProfile;
        public string SubName;
        public bool[] RumbleDevices = new bool[4];

        public AssignmentProfile()
        {
            MainAssignments = new List<AssignmentPair>();
            RedAssignments = new List<AssignmentPair>();
            BlueAssignments = new List<AssignmentPair>();
            GreenAssignments = new List<AssignmentPair>();
        }

        public AssignmentProfile(Dictionary<string, AssignmentCollection>[] assignments) : this()
        {
            FromAssignmentArray(assignments);
        }

        public Dictionary<string, AssignmentCollection>[] ToAssignmentArray(IDeviceControl device = null)
        {
            Dictionary<string, AssignmentCollection>[] result = new[] {
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>()
            };

            AddAssignments(result[0], MainAssignments, device);
            AddAssignments(result[1], RedAssignments, device);
            AddAssignments(result[2], BlueAssignments, device);
            AddAssignments(result[3], GreenAssignments, device);

            return result;
        }

        public void FromAssignmentArray(Dictionary<string, AssignmentCollection>[] assignments)
        {
            if (assignments.Length > 0)
            {
                foreach (var a in assignments[0])
                    MainAssignments.Add(new AssignmentPair(a.Key, a.Value));
            }

            if (assignments.Length > 1)
            {
                foreach (var a in assignments[1])
                    RedAssignments.Add(new AssignmentPair(a.Key, a.Value));
            }

            if (assignments.Length > 2)
            {
                foreach (var a in assignments[2])
                    BlueAssignments.Add(new AssignmentPair(a.Key, a.Value));
            }

            if (assignments.Length > 3)
            {
                foreach (var a in assignments[3])
                    GreenAssignments.Add(new AssignmentPair(a.Key, a.Value));
            }
        }

        private void AddAssignments(Dictionary<string, AssignmentCollection> mapping, List<AssignmentPair> assignmentPairs, IDeviceControl device = null)
        {
            foreach (var pair in assignmentPairs)
            {
                var collection = pair.GetCollection();

                foreach (var assignment in collection)
                {
                    if (assignment is ShiftAssignment)
                    {
                        (assignment as ShiftAssignment).SetControl(device);
                    }
                }

                mapping.Add(pair.input, pair.GetCollection());
            }
        }
    }

    public class AssignmentPair
    {
        public string input;
        public List<AssignmentInfo> collection;

        public AssignmentPair()
        {
            input = "";
            collection = new List<AssignmentInfo>();
        }

        public AssignmentPair(string inputName, AssignmentCollection assignments) : this()
        {
            input = inputName;

            foreach (var a in assignments)
            {
                AssignmentInfo info = new AssignmentInfo();

                switch (a)
                {
                    case KeyboardAssignment _:
                        info.type = AssignmentType.Keyboard;
                        info.keyboardAssignment = (KeyboardAssignment)a;
                        break;
                    case MouseAssignment _:
                        info.type = AssignmentType.Mouse;
                        info.mouseAssignment = (MouseAssignment)a;
                        break;
                    case MouseButtonAssignment _:
                        info.type = AssignmentType.MouseButton;
                        info.mouseButtonAssignment = (MouseButtonAssignment)a;
                        break;
                    case MouseScrollAssignment _:
                        info.type = AssignmentType.MouseScroll;
                        info.mouseScrollAssignment = (MouseScrollAssignment)a;
                        break;
                    case ShiftAssignment _:
                        info.type = AssignmentType.Shift;
                        info.shiftAssignment = (ShiftAssignment)a;
                        break;
                    case XInputAxisAssignment _:
                        info.type = AssignmentType.XboxAxis;
                        info.xinputAxisAssignment = (XInputAxisAssignment)a;
                        break;
                    case XInputButtonAssignment _:
                        info.type = AssignmentType.XboxButton;
                        info.xinputButtonAssignment = (XInputButtonAssignment)a;
                        break;
                    case VJoyButtonAssignment _:
                        info.type = AssignmentType.VJoyButton;
                        info.vjoyButtonAssignment = (VJoyButtonAssignment)a;
                        break;
                    case VJoyAxisAssignment _:
                        info.type = AssignmentType.VJoyAxis;
                        info.vjoyAxisAssignment = (VJoyAxisAssignment)a;
                        break;
                    case VJoyPOVAssignment _:
                        info.type = AssignmentType.VJoyPOV;
                        info.vjoyPOVAssignment = (VJoyPOVAssignment)a;
                        break;
                    default:
                        info.type = AssignmentType.Undefined;
                        break;
                }

                collection.Add(info);
            }
        }

        public AssignmentCollection GetCollection()
        {
            AssignmentCollection result = new AssignmentCollection();

            foreach (var c in collection)
            {
                switch (c.type)
                {
                    case AssignmentType.Keyboard:    result.Add(c.keyboardAssignment); break;
                    case AssignmentType.Mouse:       result.Add(c.mouseAssignment); break;
                    case AssignmentType.MouseButton: result.Add(c.mouseButtonAssignment); break;
                    case AssignmentType.MouseScroll: result.Add(c.mouseScrollAssignment); break;
                    case AssignmentType.Shift:       result.Add(c.shiftAssignment); break;
                    case AssignmentType.XboxAxis:    result.Add(c.xinputAxisAssignment); break;
                    case AssignmentType.XboxButton:  result.Add(c.xinputButtonAssignment); break;
                    case AssignmentType.VJoyButton:  result.Add(c.vjoyButtonAssignment); break;
                    case AssignmentType.VJoyAxis:    result.Add(c.vjoyAxisAssignment); break;
                    case AssignmentType.VJoyPOV:     result.Add(c.vjoyPOVAssignment); break;
                }
            }

            return result;
        }
    }

    public class AssignmentInfo
    {
        public AssignmentType type;
        public KeyboardAssignment keyboardAssignment;
        public MouseAssignment mouseAssignment;
        public MouseButtonAssignment mouseButtonAssignment;
        public MouseScrollAssignment mouseScrollAssignment;
        public ShiftAssignment shiftAssignment;
        public XInputAxisAssignment xinputAxisAssignment;
        public XInputButtonAssignment xinputButtonAssignment;
        public VJoyButtonAssignment vjoyButtonAssignment;
        public VJoyAxisAssignment vjoyAxisAssignment;
        public VJoyPOVAssignment vjoyPOVAssignment;
    }

    public enum AssignmentType
    {
        Undefined = -1,
        Keyboard,
        Mouse,
        MouseButton,
        MouseScroll,
        Shift,
        XboxAxis,
        XboxButton,
        VJoyButton,
        VJoyAxis,
        VJoyPOV
    }
}
