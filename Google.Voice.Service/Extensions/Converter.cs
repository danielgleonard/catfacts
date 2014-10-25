using System;

namespace Google.Voice.Extensions
{
    public class Converter
    {
        private object Data { get; set; }
        private object DefaultValue { get; set; }

        public static Converter Do(object data, object defaultValue = null)
        {
            return new Converter(data, defaultValue);
        }

        public Converter(object data, object defaultValue)
        {
            Data = data;
            DefaultValue = defaultValue;
        }

        public static implicit operator bool(Converter converter)
        {
            try
            {
                if (converter.DefaultValue != null)
                {
                    return converter.Data.ToBool(converter.DefaultValue.ToBool());
                }

                return converter.Data.ToBool(false);
            }
            catch { }

            return false;
        }

        public static implicit operator decimal(Converter converter)
        {
            try
            {
                if (converter.DefaultValue != null)
                {
                    return converter.Data.ToDecimal(converter.DefaultValue.ToDecimal());
                }

                return converter.Data.ToDecimal(0);
            }
            catch { }

            return 0;
        }

        public static implicit operator int(Converter converter)
        {
            try
            {
                if (converter.DefaultValue != null)
                {
                    return converter.Data.ToInt(converter.DefaultValue.ToInt());
                }

                return converter.Data.ToInt(0);
            }
            catch { }

            return 0;
        }

        public static implicit operator long(Converter converter)
        {
            try
            {
                if (converter.DefaultValue != null)
                {
                    return converter.Data.ToLong(converter.DefaultValue.ToLong());
                }

                return converter.Data.ToLong(0);
            }
            catch { }

            return 0;
        }

        public static implicit operator float(Converter converter)
        {
            try
            {
                if (converter.DefaultValue != null)
                {
                    return converter.Data.ToFloat(converter.DefaultValue.ToFloat());
                }

                return converter.Data.ToFloat(0);
            }
            catch { }

            return 0;
        }

        public static implicit operator double(Converter converter)
        {
            try
            {
                if (converter.DefaultValue != null)
                {
                    return converter.Data.ToDouble(converter.DefaultValue.ToDouble());
                }

                return converter.Data.ToDouble(0);
            }
            catch { }

            return 0;
        }

        public static implicit operator string(Converter converter)
        {
            string val = "";

            if (converter.Data == null)
            {
                val = "";
            }
            else
            {
                val = converter.Data.ToString();
            }

            if (converter.DefaultValue == null)
            {
                return val;
            }
            else
            {
                if (string.IsNullOrEmpty(converter.DefaultValue.ToString()))
                {
                    return val;
                }
                else
                {
                    return string.IsNullOrEmpty(val) ? converter.DefaultValue.ToString() : val;
                }
            }
        }

        public static implicit operator DateTime?(Converter converter)
        {
            try
            {
                if (converter.DefaultValue != null)
                {
                    return converter.Data.ToDateTime(converter.DefaultValue.ToDateTime());
                }
                return converter.Data.ToDateTime();
            }
            catch { }

            return null;
        }
    }
}