using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridCLR.Runtime
{
    public enum MethodInjectMode
    {
        None,
        Proxy,
    }

    public class MethodInjectAttribute : Attribute
    {
        public MethodInjectMode Mode { get; }

        public MethodInjectAttribute(MethodInjectMode mode)
        {
            Mode = mode;
        }
    }
}
