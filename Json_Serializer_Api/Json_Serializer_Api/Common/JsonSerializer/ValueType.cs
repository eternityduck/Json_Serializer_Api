namespace Json_Serializer_Api.Common.JsonSerializer;

public static class ValueTypes
{
    public enum Values
    {
        Unspecified,
        String,
        Number,
        Array,
        Object,
        Boolean,
        Null,
        Invalid,
    }

    public static Values CheckValueType(string value)
    {
        if (value == null)
        {
            return Values.Null;
        }

        while (value.Length > 0 && value[0] == ' ')
        {
            value = value.Substring(1);
        }

        while (value.Length > 0 && value[^1] == ' ')
        {
            value = value.Substring(0, value.Length - 1);
        }

        if (value.Length > 1 && value[0] == '"' && value[^1] == '"')
        {
            return Values.String;
        }

        if (value.Length > 1 && value[0] == '{' && value[^1] == '}')
        {
            return Values.Object;
        }

        if (value.Length > 1 && value[0] == '[' && value[^1] == ']')
        {
            return Values.Array;
        }

        var d = 0d;
        if (TryDeserializeNumber(value, out d))
        {
            return Values.Number;
        }

        switch (value)
        {
            case "true": case "false": return Values.Boolean;
            case "null": return Values.Null;
        }

        return Values.Invalid;
    }
    public static bool TryDeserializeNumber(string stringValue, out double output)
    {
        if (double.TryParse(stringValue, out var result))
        {
            output = result;
            return true;
        };

        stringValue = stringValue.Replace(',', '.');

        if (double.TryParse(stringValue, out result))
        {
            output = result;
            return true;
        };

        output = 0;
        return false;
    }
}