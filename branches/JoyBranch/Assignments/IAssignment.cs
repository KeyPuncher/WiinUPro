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
        public delegate void AssignDelegate(Dictionary<string, AssignmentCollection> collection);

        public List<IAssignment> Assignments { get; protected set; }

        public bool ShiftAssignment
        {
            get
            {
                return Assignments.Count == 1 && Assignments[0] is ShiftAssignment;
            }
        }

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
            foreach (var assignment in Assignments.ToArray())
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
}
