using BLL.Interfaces;
using DAL.Models;
using DAL.Repositories;

namespace BLL.Services;

public class PersonService : IPersonService
{
    private PersonRepository _repository;

    public PersonService(PersonRepository repository)
    {
        _repository = repository;
    }
    public async Task<IEnumerable<Person>> GetAllPersonsAsync()
    {
        return await _repository.GetAllPersonsAsync();
    }

    public async Task<Person> GetPersonByIdAsync(long personId)
    {
        return await _repository.GetPersonByIdAsync(personId);
    }

    public void InsertPerson(Person person)
    {
        _repository.InsertPerson(person);
        _repository.Save();
    }

    public void DeletePerson(long personId)
    {
        _repository.DeletePersonAsync(personId);
        _repository.Save();
    }

    public void UpdatePerson(Person person)
    {
        _repository.UpdatePerson(person);
        _repository.Save();
    }

    public async Task<long> Add(Person person)
    {
        _repository.InsertPerson(person);
        var result = await _repository.GetPersonByIdAsync(person.Id);
        return result.Id;
    }
}