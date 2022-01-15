using DAL.Models;

namespace DAL.Interfaces;

public interface IPersonRepository
{
    Task<IEnumerable<Person>> GetAllPersonsAsync();
    Task<Person> GetPersonByIdAsync(long personId);
    void InsertPerson(Person person);
    Task DeletePersonAsync(long personId);
    void UpdatePerson(Person person);
    void Save();
}