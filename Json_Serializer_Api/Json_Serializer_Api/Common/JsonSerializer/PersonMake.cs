using DAL.Models;

namespace Json_Serializer_Api.Common.JsonSerializer;

public static class PersonMake
{
    public static Person ReturnPerson(Dictionary<string, string> dict)
    {
        var person = new Person();
        foreach (var (key, value) in dict)
        {
            if (key.Replace("\"", "") == "firstName")
            {
                person.FirstName = value.Replace("\"", "");;
            }
        
            if (key.Replace("\"", "") == "lastName")
            {
                person.LastName = value.Replace("\"", "");
            }
        
            if (key.Replace("\"", "") == "address")
            {
                var deser = JsonSerializer.DeserializeObject(value);
                person.Address = new Address()
                {
                    AddressLine = deser[deser.Keys.FirstOrDefault(x => x.Replace("\"", "") == "addressLine")]
                        .Replace("\"", ""),
                    City = deser[deser.Keys.FirstOrDefault(x => x.Replace("\"", "") == "city")]
                        .Replace("\"", "")
                };
            }
            
        }

        return person;
    }
    
}