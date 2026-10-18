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
                return Assignments.Exists(a => a is ShiftAssignment);
            }
        }

        /// <summary>
        /// Gets the first ShiftAssignment in the collection (if any).
        /// A collection can now contain a key assignment and a shift assignment together.
        /// </summary>
        public ShiftAssignment GetShift()
        {
            return Assignments.Find(a => a is ShiftAssignment) as ShiftAssignment;
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
            if (assignment == null || Assignments.Contains(assignment))
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
                assignment?.Apply(value);
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
                return Shared.Globalization.Translate("Input_Unset");

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Assignments.Count; ++i)
            {
                var assignment = Assignments[i];

                if (assignment != null)
                {
                    sb.Append(assignment.GetDisplayName());
                    sb.Append("|");
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }
    }
}
