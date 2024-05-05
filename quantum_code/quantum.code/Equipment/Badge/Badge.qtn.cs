using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public abstract unsafe partial class Badge
    {
        public abstract void OnApply(Frame f, EntityRef user);
        public virtual void OnUpdate(Frame f, EntityRef user) { }
        public abstract void OnRemove(Frame f, EntityRef user);
    }
}
