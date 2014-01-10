using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.RSL
{
    public class Variable
    {
        public string name;
        private object value;
        public bool baseValue = false;

        public Variable(string name, object value, bool baseval)
        {
            this.name = name;
            this.value = value;
            this.baseValue = baseval;
        }

        public object getValue()
        {
            return this.value;
        }

        public void setValue(object value)
        {
            this.value = value;
            
        }

        public string getSaveData()
        {
            return "SETVAL " + name + " " + value;
        }
    }
}
