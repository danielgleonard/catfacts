using System;
using System.Collections.Generic;
using System.Text;
using Google.Voice.Extensions;

namespace Google.Voice.Entities
{
  
    public class Variable
    {
        private KeyValuePair<string, object> d = new KeyValuePair<string, object>();
        
        public Variable(string name, object value)
        {
            d = new KeyValuePair<string, dynamic>(name, value);
        }

        public static implicit operator string(Variable obj)
        {
            return Convert.ToString(obj.Value);
        }

        public static implicit operator int(Variable obj)
        {
            return Methods.ToInt(obj.Value, 0);
        }

        public static implicit operator long(Variable obj)
        {
            return Methods.ToLong(obj.Value, 0);
        }

        public static implicit operator decimal(Variable obj)
        {
            return Methods.ToDecimal(obj.Value, 0);
        }

        public static implicit operator bool(Variable obj)
        {
            return Methods.ToBool(obj.Value, false);
        }

        public static implicit operator ShallowEntity(Variable obj)
        {
            return (ShallowEntity)obj;
        }

        public string Name
        {
            get { return d.Key; }
            set
            {
                d = new KeyValuePair<string, object>(value, Value);
            }
        }

        public object Value
        {
            get { return d.Value; }
            set
            {
                d = new KeyValuePair<string, object>(Name, value);
            }
        }

        public override string ToString()
        {
            return new { Name, Value }.ToString();
        }
    }
}
