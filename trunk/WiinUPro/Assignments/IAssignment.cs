using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    public interface IAssignment
    {
        void Apply(float value);
        bool SameAs(IAssignment assignment);
        // Don't forget to override Equals(object)
    }

    public class AssignmentCollection : IEnumerable<IAssignment>
    {
        public List<IAssignment> Assignments { get; protected set; }

        public AssignmentCollection()
        {
            Assignments = new List<IAssignment>();
        }

        public AssignmentCollection(List<IAssignment> assignments)
        {
            Assignments = assignments;
        }

        public bool Add(IAssignment assignment)
        {
            if (Assignments.Contains(assignment))
            {
                return false;
            }
            else
            {
                Assignments.Add(assignment);
                return true;
            }
        }

        public void ApplyAll(float value)
        {
            foreach (var assignment in Assignments)
            {
                assignment.Apply(value);
            }
        }

        public IEnumerator<IAssignment> GetEnumerator()
        {
            return Assignments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TestMouseAssignment : IAssignment
    {
        private bool xAxis;

        private static float x, x2;
        private static float y, y2;

        public TestMouseAssignment(bool isX)
        {
            xAxis = isX;
        }

        public void Apply(float value)
        {
            value = (float)Math.Round(value, 2);

            if (xAxis)
            {
                MouseDirector.Access.MouseMoveX((int)Math.Round(10 * value));
                //MouseDirector.Access.MouseMoveTo(value, 0);
                x = value;
            }
            else
            {
                MouseDirector.Access.MouseMoveY((int)Math.Round(-10 * value));
                // MouseDirector.Access.MouseMoveTo(0, value);
                y = value;
            }

            if (x != x2 || y != y2)
            {
                //MouseDirector.Access.MouseMoveTo(x, y);
                x2 = x;
                y2 = y;
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            return false;
        }
    }
}
