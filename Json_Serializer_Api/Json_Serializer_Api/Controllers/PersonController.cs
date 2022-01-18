using System.Text;
using BLL.Interfaces;
using DAL.Models;
using Json_Serializer_Api.Common.JsonSerializer;
using Json_Serializer_Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Json_Serializer_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : Controller
{
    private readonly IPersonService _personService;

    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }
    [HttpPost]
    public async Task<ActionResult<IEnumerable<string>>> SaveAsync(string input)
    {
        var result = JsonSerializer<Person>.DeserializeObject(input);
        return result.Keys;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAllAsync(GetAllRequest request)
    {
        var persons = await _personService.GetAllPersonsAsync();

        var result = persons.Where(x => x.LastName == request.LastName 
                                        && x.FirstName == request.FirstName 
                                        && x.Address.City == request.City);
        var sb = new StringBuilder();
        foreach (var item in result)
        {
            sb.Append(JsonSerializer<Person>.SerializeValue(item));
        }

        return sb.ToString();
    }
}