using System.Globalization;
using System.Text;

namespace Json_Serializer_Api.Common.JsonSerializer;

public static class JsonSerializer<T> where T : class
{
    // public static string Serialize(T model)
    // {
    //     var sb = new StringBuilder();
    //     
    //     return sb.ToString();
    // }

    // public static T DeSerialize(string json)
    // {
    //     
    // }
    public static string SerializeValue(object value)
    {
        
        var t = value.GetType();

        if (t == typeof(string))
        {
            return SerializeString((string)value);
        }

        if (t == typeof(bool))
        {
            return SerializeBoolean((bool)value);
        }

        if (t == typeof(int))
        {
            return SerializeNumber((int)value);
        }

        if (t == typeof(float))
        {
            return SerializeNumber((float)value);
        }

        if (t == typeof(double))
        {
            return SerializeNumber((double)value);
        }

        if (t == typeof(Dictionary<string, string>))
        {
            return SerializeObject((Dictionary<string, string>)value);
        }

        if (t == typeof(string[]))
        {
            return SerializeArray((string[])value);
        }


        return null;
    }


    public static string SerializeStringIfInvalid(string value)
    {
        if (value == null)
        {
            return SerializeNull();
        }

        return ValueTypes.CheckValueType(value) == ValueTypes.Values.Invalid ? SerializeString(value) : value;
    }

    public static string SerializeStringIfNotString(string value)
    {
        if (value == null)
        {
            return SerializeNull();
        }

        return ValueTypes.CheckValueType(value) != ValueTypes.Values.String ? SerializeString(value) : value;
    }

    public static string SerializeString(string stringValue)
    {
        if (stringValue == null)
        {
            return SerializeNull();
        }

        var result = stringValue
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\t", "\\t")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");


        return $"\"{result}\"";
    }

    public static string SerializeNumber(double value)
    {
        return value.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
    }

    public static string SerializeNumber(float value)
    {
        return value.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
    }

    public static string SerializeNumber(int value)
    {
        return value.ToString();
    }

    public static string SerializeBoolean(bool value)
    {
        return value ? "true" : "false";
    }

    public static string SerializeKeyValuePair(string key, string value)
    {
        key = SerializeStringIfNotString(key);
        value = SerializeStringIfInvalid(value);

        return $"{key}: {value}";
    }

    public static string SerializeKeyValuePair(KeyValuePair<string, string> keyValuePair)
    {
        return SerializeKeyValuePair(keyValuePair.Key, keyValuePair.Value);
    }

    public static string SerializeArray(string[] array)
    {
        if (array == null)
        {
            return SerializeNull();
        }

        if (array.Length == 0)
        {
            return "[]";
        }

        var entries = array.ToList();

        return $"[{String.Join(",", entries)}]";
    }

    public static string SerializeObject(Dictionary<string, string> dictionary)
    {
        if (dictionary == null)
        {
            return SerializeNull();
        }

        if (dictionary.Count == 0)
        {
            return "{}";
        }

        var entries = dictionary.Select(entry => SerializeKeyValuePair(entry)).ToList();

        return $"{{{String.Join(",", entries)}}}";
    }


    private static string SerializeNull()
    {
        return "null";
    }
    public static Dictionary<string, string> DeserializeObject(string jsonObject)
    {
        var result = new Dictionary<string, string>();
        var bracket = 0;
        var kvpIndexArray = new int[4];
        var arrayIndex = 0;
        var escape = false;

        jsonObject = Format.RemoveFormatting(jsonObject);

        if (jsonObject == "{}")
        {
            return result;
        }

        for (int i = 0; i < jsonObject.Length; i++)
        {
            var character = jsonObject[i];

            switch (character)
            {
                case '\\':
                    i++;
                    continue;
                case '"':
                {
                    escape = !escape;
                    if (bracket == 1 && arrayIndex < 2)
                    {
                        kvpIndexArray[arrayIndex] = i;
                        arrayIndex += 1;
                    }

                    break;
                }
            }

            if (escape)
            {
                continue;
            }

            switch (character)
            {
                case ':':
                {
                    if (bracket == 1 && arrayIndex == 2)
                    {
                        kvpIndexArray[arrayIndex] = i + 2;
                        arrayIndex += 1;
                    }

                    break;
                }
                case ',':
                {
                    if (bracket == 1 && arrayIndex == 3)
                    {
                        kvpIndexArray[arrayIndex] = i - 1;
                        arrayIndex += 1;
                    }

                    break;
                }
                case '{' or '[':
                    bracket += 1;
                    break;
                case '}' or ']':
                {
                    bracket -= 1;
                    if (bracket == 0)
                    {
                        kvpIndexArray[arrayIndex] = i - 1;
                        arrayIndex += 1;
                    }

                    break;
                }
            }

            if (arrayIndex == 4)
            {
                result.Add(jsonObject.Substring(kvpIndexArray[0], kvpIndexArray[1] - kvpIndexArray[0] + 1),
                    jsonObject.Substring(kvpIndexArray[2], kvpIndexArray[3] - kvpIndexArray[2] + 1));
                arrayIndex = 0;
            }
        }

        return result;
    }
}



