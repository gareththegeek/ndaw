using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Core.Tests
{
    public class FloatComparer: System.Collections.IComparer
    {
        public const float Epsilon = 0.0001f;

        private readonly float epsilon;

        public FloatComparer()
        {
            this.epsilon = Epsilon;
        }

        public int Compare(object x, object y)
        {
            var a = (float)x;
            var b = (float)y;

            double delta = System.Math.Abs(a - b);
            if (delta < epsilon)
            {
                return 0;
            }
            return a.CompareTo(b);
        }
    }
}
