using Json_Serializer_Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Json_Serializer_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : Controller
{
    [HttpPost]
    public Task<long> SaveAsync(string input)
    {
        
    }

    [HttpGet]
    public Task<string> GetAllAsync(GetAllRequest request)
    {
        
    }
}