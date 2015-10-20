using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro.Assignments
{
    public class MouseButtonAssignment : IAssignment
    {
        public void Apply(float value)
        {
            throw new NotImplementedException();
        }

        public bool SameAs(IAssignment assignment)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
