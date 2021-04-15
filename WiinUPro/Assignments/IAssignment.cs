using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WiinUPro
{
    public interface IAssignment
    {
        void Apply(float value);
        bool SameAs(IAssignment assignment);
        string GetDisplayName();
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

        public override string ToString()
        {
            if (Assignments.Count == 0)
                return "UNSET";

            StringBuilder sb = new StringBuilder(Assignments[0].GetDisplayName());

            for (int i = 1; i < Assignments.Count; ++i)
            {
                sb.Append("|");
                sb.Append(Assignments[i].GetDisplayName());
            }

            return sb.ToString();
        }
    }
}
