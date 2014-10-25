using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Google.Voice.Entities;

namespace Google.Voice
{
    public class VariableCollection : List<Variable>
    {
        public VariableCollection(int capacity)
            : base(capacity) { }

        public VariableCollection()
            : base(1) { }

        public IEnumerable<string> AllKeys
        {
            get
            {
                return this.Select<Variable, string>(v => v.Name);
            }
        }

        public Variable this[string name]
        {
            get
            {
                return GetVariable(name);
            }

            set
            {
                SetVariable(name, value);
            }
        }

        public bool ContainsName(string name)
        {
            foreach (Variable x in this)
            {
                if (x.Name == name) return true;
            }
            return false;
        }

        private Variable GetVariable(string name)
        {
            if (this.Where(v => v.Name == name).Count() == 0)
            {
                this.Add(new Variable(name, null));
            }

            return this.Where(v => v.Name == name).First();
        }

        private void SetVariable(string name, object value)
        {
            if (this.Where(v => v.Name == name).Count() == 0)
            {
                this.Add(new Variable(name, value));
            }
            else
            {
                this.Where(v => v.Name == name).First().Value = value;
            }
        }
    }
}
