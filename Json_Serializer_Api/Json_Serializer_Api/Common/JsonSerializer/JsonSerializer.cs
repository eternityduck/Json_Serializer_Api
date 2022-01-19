using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Json_Serializer_Api.Common.JsonSerializer;

public static class JsonSerializer
{
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
    

    private static bool IsNanOrInfinity(object obj)
    {
        return obj switch
        {
            double d => double.IsNaN(d) || double.IsInfinity(d),
            float f => float.IsNaN(f) || float.IsInfinity(f),
            _ => false
        };
    }

    public static string Serialize(object objectToSerialize)
    {
        if (objectToSerialize.GetType().IsPrimitive || objectToSerialize is decimal)
        {
            return IsNanOrInfinity(objectToSerialize)
                ? null
                : string.Format(CultureInfo.InvariantCulture, "{0}", objectToSerialize.ToString()?.ToLower());
        }

        switch (objectToSerialize)
        {
            case string or DateTime or TimeSpan or DateTimeOffset:
            {
                var serializableString = objectToSerialize.ToString()?.Replace("\"", "\\\"");
                return string.Concat("\"", serializableString, "\"");
            }
            case IDictionary serialize:
            {
                var jsonDictionaryBuilder = new StringBuilder();
                jsonDictionaryBuilder.Append("{");

                foreach (var i in serialize.Keys)
                {
                    var value = serialize[i];
                    jsonDictionaryBuilder.Append(string.Concat("\"", i, "\":", Serialize(value), ","));
                }

                var jsonEnumerableString = jsonDictionaryBuilder.ToString();
                if (jsonEnumerableString[^1] == ',')
                {
                    jsonEnumerableString = jsonEnumerableString.Substring(0, jsonEnumerableString.Length - 1);
                }

                return jsonEnumerableString + "}";
            }
            case IEnumerable serialize:
            {
                var jsonEnumerableBuilder = new StringBuilder();
                jsonEnumerableBuilder.Append("[");

                foreach (var i in serialize)
                {
                    if (i.GetType().IsSerializable)
                    {
                        jsonEnumerableBuilder.Append(Serialize(i) + ",");
                    }
                }

                var jsonEnumerableString = jsonEnumerableBuilder.ToString();
                if (jsonEnumerableString[^1] == ',')
                {
                    jsonEnumerableString = jsonEnumerableString[..^1];
                }

                return jsonEnumerableString + "]";
            }
        }

        var jsonBuilder = new StringBuilder();

        jsonBuilder.Append("{");

        var properties = objectToSerialize.GetType().GetProperties(BindingFlags.Public |
                                                                   BindingFlags.NonPublic
                                                                   | BindingFlags.Instance);
        foreach (var propertyInfo in properties)
        {
            var serializedField = Serialize(propertyInfo.GetValue(objectToSerialize) ?? throw new InvalidOperationException());
            
            jsonBuilder.Append(string.Concat("\"", propertyInfo.Name, "\":", serializedField, ","));
        }

        var serializedObjects = jsonBuilder.ToString();

        if (serializedObjects[^1] == ',')
        {
            serializedObjects = serializedObjects[..^1];
        }

        return serializedObjects + "}";
    }
}