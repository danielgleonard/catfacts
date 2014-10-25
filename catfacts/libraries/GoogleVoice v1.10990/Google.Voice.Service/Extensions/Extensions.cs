using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Google.Voice.Extensions
{
    public static class Methods
    {

        /// <summary>
        /// Dumps object data into a string
        /// </summary>
        /// <param name="object"></param>
        /// <param name="recursion"></param>
        /// <returns></returns>
        public static string Dump(this object @object, int recursion = 4)
        {
            StringBuilder result = new StringBuilder();

            // Protect the method against endless recursion
            if (recursion < 5)
            {
                // Determine object type
                Type t = @object.GetType();

                // Get array with properties for this object
                PropertyInfo[] properties = t.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        // Get the property value
                        object value = property.GetValue(@object, null);

                        // Create indenting string to put in front of properties of a deeper level 
                        // We'll need this when we display the property name and value
                        string indent = String.Empty;
                        string spaces = "|   ";
                        string trail = "|...";

                        if (recursion > 0)
                        {
                            indent = new StringBuilder(trail).Insert(0, spaces, recursion - 1).ToString();
                        }

                        if (value != null)
                        {
                            // If the value is a string, add quotation marks
                            string displayValue = value.ToString();
                            if (value is string) displayValue = String.Concat('"', displayValue, '"');

                            // Add property name and value to return string
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, displayValue);

                            try
                            {
                                if (!(value is ICollection))
                                {
                                    // Call var_dump() again to list child properties
                                    // This throws an exception if the current property value 
                                    // is of an unsupported type (eg. it has not properties)
                                    result.Append(Methods.Dump(value, recursion + 1));
                                }
                                else
                                {
                                    // 2009-07-29: added support for collections
                                    // The value is a collection (eg. it's an arraylist or generic list)
                                    // so loop through its elements and dump their properties
                                    int elementCount = 0;
                                    foreach (object element in ((ICollection)value))
                                    {
                                        string elementName = String.Format("{0}[{1}]", property.Name, elementCount);
                                        indent = new StringBuilder(trail).Insert(0, spaces, recursion).ToString();

                                        // Display the collection element name and type
                                        result.AppendFormat("{0}{1} = {2}\n", indent, elementName, element.ToString());

                                        // Display the child properties
                                        result.Append(Methods.Dump(element, recursion + 2));
                                        elementCount++;
                                    }

                                    result.Append(Methods.Dump(value, recursion + 1));
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            // Add empty (null) property to return string
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, "null");
                        }
                    }
                    catch
                    {
                        // Some properties will throw an exception on property.GetValue() 
                        // I don't know exactly why this happens, so for now i will ignore them...
                    }
                }
            }

            return result.ToString();
        }

        #region Conversions

        public static T CType<T>(this object @object)
        {
            return (T)@object;
        }

        public static long ToLong(this object @object, long defaultValue = 0)
        {
            long result;

            try
            {
                if (long.TryParse(@object.ToString(), out result))
                {
                    return result;
                }
            }
            catch { }

            return defaultValue;
        }

        public static int ToInt(this object @object, int defaultValue = 0)
        {
            int result;

            try
            {
                if (int.TryParse(@object.ToString(), out result))
                {
                    return result;
                }
            }
            catch { }

            return defaultValue;
        }

        public static decimal ToDecimal(this object @object, decimal defaultValue = 0)
        {
            decimal result;

            try
            {
                if (decimal.TryParse(@object.ToString(), out result))
                {
                    return result;
                }
            }
            catch { }

            return defaultValue;
        }

        public static double ToDouble(this object @object, double defaultValue = 0)
        {
            double result;

            try
            {
                if (double.TryParse(@object.ToString(), out result))
                {
                    return result;
                }
            }
            catch { }

            return defaultValue;
        }

        public static float ToFloat(this object @object, float defaultValue = 0)
        {
            float result;

            try
            {
                if (float.TryParse(@object.ToString(), out result))
                {
                    return result;
                }
            }
            catch { }

            return defaultValue;
        }

        public static bool ToBool(this object @object, bool defaultValue = false)
        {
            bool result;

            try
            {
                if (bool.TryParse(@object.ToString(), out result))
                {
                    return result;
                }
            }
            catch { }

            return defaultValue;
        }

        public static DateTime? ToDateTime(this object @object, DateTime? defaultValue = null)
        {
            DateTime result;

            try
            {
                if (DateTime.TryParse(@object.ToString(), out result))
                {
                    return result;
                }
            }
            catch { }

            return defaultValue;
        }

        #endregion

        #region Strings

        public static string UrlEncode(this string @string)
        {
            try
            {
                return HttpUtility.UrlEncode(@string, Encoding.UTF8);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Generates the MD5 of the supplied text
        /// </summary>
        public static string MD5(this string @string)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(@string);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).ToLower().Replace("-", "");
        }

        /// <summary>
        /// Base64 decryption method
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Base64Decode(this string @string)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(@string);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }

        /// <summary>
        /// Base64 encryption method
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string Base64Encode(this string @string)
        {
            try
            {
                byte[] encData_byte = new byte[@string.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(@string);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

        /// <param name="string">Text to encrypt</param>
        /// <param name="passPhrase">Password to aid in encryption</param>
        /// <returns>Returns the encrypted value of a plaintext string</returns>
        public static string Encrypt(this string @string, string passPhrase, string iv, string salt, string hash, int iterations, int keySize)
        {
            @string = @string + "mUJdbXnYQBAf9Bagay0ttS5p";

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(iv);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(@string);

            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hash,
                                                            iterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor =
                symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream =
                new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }

        /// <param name="string">Text to decrypt</param>
        /// <param name="passPhrase">Password to aid in decryption</param>
        /// <returns>The decrypted representation of an encrypted string</returns>
        public static string Decrypt(this string @string, string passPhrase, string iv, string salt, string hash, int iterations, int keySize)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(iv);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);

            byte[] cipherTextBytes = Convert.FromBase64String(@string);

            PasswordDeriveBytes password =
                new PasswordDeriveBytes(passPhrase, saltValueBytes, hash, iterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform decryptor =
                symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream =
                new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount =
                cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            string plainText =
                Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            return plainText.Substring(0, plainText.Length - 24);
        }

        #endregion

        #region Json

        /// <summary>
        /// Deserializes a JSON string into an object of type T
        /// </summary>
        public static T ToObject<T>(this string @string)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(@string);
            }
            catch
            {
                var index = @string.LastIndexOf('}');
                var alt = @string.Substring(0, index);
                return JsonConvert.DeserializeObject<T>(alt);
            }
        }

        /// <summary>
        /// Deserializes a JSON string into an object of type T
        /// </summary>
        public static T ToObject<T>(this string @string, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(@string, settings);
        }

        /// <summary>
        /// Deserializes a JSON string into an object of type T
        /// </summary>
        public static T ToObject<T>(this string @string, params JsonConverter[] converters)
        {
            return JsonConvert.DeserializeObject<T>(@string, converters);
        }

        /// <summary>
        /// Serializes an object into a JSON string
        /// </summary>
        public static string ToJson(this object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        /// <summary>
        /// Serializes an object into a JSON string
        /// </summary>
        public static string ToJson(this object @object, Formatting formatting)
        {
            return JsonConvert.SerializeObject(@object, formatting);
        }

        /// <summary>
        /// Serializes an object into a JSON string
        /// </summary>
        public static string ToJson(this object @object, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(@object, converters);
        }

        /// <summary>
        /// Serializes an object into a JSON string
        /// </summary>
        public static string ToJson(this object @object, Formatting formatting, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(@object, formatting, converters);
        }

        /// <summary>
        /// Serializes an object into a JSON string
        /// </summary>
        public static string ToJson(this object @object, Formatting formatting, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(@object, formatting, settings);
        }

        #endregion
    }
}
