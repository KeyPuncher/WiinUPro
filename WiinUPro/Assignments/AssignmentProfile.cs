using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiinUPro
{
    public class AssignmentProfile
    {
        public List<AssignmentPair> MainAssignments;
        public List<AssignmentPair> RedAssignments;
        public List<AssignmentPair> BlueAssignments;
        public List<AssignmentPair> GreenAssignments;

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

        public Dictionary<string, AssignmentCollection>[] ToAssignmentArray()
        {
            Dictionary<string, AssignmentCollection>[] result = new[] {
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>()
            };

            foreach (var ma in MainAssignments) result[0].Add(ma.input, ma.GetCollection());
            foreach (var ra in RedAssignments) result[1].Add(ra.input, ra.GetCollection());
            foreach (var ba in BlueAssignments) result[2].Add(ba.input, ba.GetCollection());
            foreach (var ga in GreenAssignments) result[3].Add(ga.input, ga.GetCollection());

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

                if (a is KeyboardAssignment)
                {
                    info.type = AssignmentType.Keyboard;
                    info.keyboardAssignment = (KeyboardAssignment)a;
                }
                else if (a is MouseAssignment)
                {
                    info.type = AssignmentType.Mouse;
                    info.mouseAssignment = (MouseAssignment)a;
                }
                else if (a is MouseButtonAssignment)
                {
                    info.type = AssignmentType.MouseButton;
                    info.mouseButtonAssignment = (MouseButtonAssignment)a;
                }
                else if (a is MouseScrollAssignment)
                {
                    info.type = AssignmentType.MouseScroll;
                    info.mouseScrollAssignment = (MouseScrollAssignment)a;
                }
                else if (a is ShiftAssignment)
                {
                    info.type = AssignmentType.Shift;
                    info.shiftAssignment = (ShiftAssignment)a;
                }
                else if (a is XInputAxisAssignment)
                {
                    info.type = AssignmentType.XboxAxis;
                    info.xinputAxisAssignment = (XInputAxisAssignment)a;
                }
                else if (a is XInputButtonAssignment)
                {
                    info.type = AssignmentType.XboxButton;
                    info.xinputButtonAssignment = (XInputButtonAssignment)a;
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
    }

    public enum AssignmentType
    {
        Keyboard,
        Mouse,
        MouseButton,
        MouseScroll,
        Shift,
        XboxAxis,
        XboxButton
    }
}
