using System.Text;
using BLL.Interfaces;
using DAL.Models;
using Json_Serializer_Api.Common.JsonSerializer;
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
    
    /// <summary>
    /// Adds the person to db
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// {
    ///     "firstName":"Ivan",
    ///     "lastName":"Petrov",
    ///     "address":{
    ///         "city":"Kiev",
    ///         "addressLine":prospect “Peremogy“ 28/7
    ///     }
    /// }
    /// </remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<long>> SaveAsync(string input)
    {
        var result = JsonSerializer.DeserializeObject(input);
        var person = PersonMake.ReturnPerson(result);
        return await _personService.Add(person);
    }
    /// <summary>
    /// Get All persons by filter
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="city"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<string>> GetAllAsync(string? firstName, string? lastName, string? city)
    {
        var result = await _personService.GetAllPersonsAsync();
        if (firstName is not null)
            result = result.Where(x => x.FirstName == firstName).ToList();
        if (lastName is not null)
            result = result.Where(x => x.LastName == lastName).ToList();
        if (city is not null)
            result = result.Where(x => x.Address.City == city).ToList();

        var sb = new StringBuilder();
        var enumerable = result as Person[] ?? result.ToArray();
        if (enumerable.Length > 1)
        {
            sb.Append('[');
            foreach (var item in enumerable)
            {
                sb.Append(JsonSerializer.Serialize(item) + ",");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
        }
        else
        {
            foreach (var item in enumerable)
            {
                sb.Append(JsonSerializer.Serialize(item));
            }
        }

        return sb.ToString();
    }
}