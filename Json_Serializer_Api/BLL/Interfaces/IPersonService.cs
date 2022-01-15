using DAL.Models;

namespace BLL.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<Person>> GetAllPersonsAsync();
    Task<Person> GetPersonByIdAsync(long personId);
    void InsertPerson(Person person);
    void DeletePerson(long personId);
    void UpdatePerson(Person person);
    Task<long> Add(Person person);
    
}