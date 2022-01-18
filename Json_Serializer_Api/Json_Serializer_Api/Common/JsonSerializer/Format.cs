namespace Json_Serializer_Api.Common.JsonSerializer;

public static class Format
{
    public static string RemoveFormatting(string jsonData)
    {
        var escape = false;

        jsonData = jsonData.Replace(Convert.ToChar(10).ToString(), null).Replace(Convert.ToChar(13).ToString(), null).Replace(Convert.ToChar(9).ToString(), null);

        for (int i = 0; i < jsonData.Length; i++)
        {
            var character = jsonData[i];

            if (character == '"')
            {
                escape = !escape;
            }

            if (escape)
            {
                continue;
            }

            switch (character)
            {
                case '/':
                    i++;
                    continue;
                case ':' when i + 1 < jsonData.Length:
                {
                    if (jsonData[i + 1] == ' ')
                    {
                        i++;
                        continue;
                    }

                    jsonData = jsonData.Insert(i + 1, " ");
                    i++;
                    continue;
                }
                case ' ':
                    jsonData = jsonData.Remove(i, 1);
                    i--;
                    break;
            }
        }

        return jsonData;
    }
}