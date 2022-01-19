using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;


namespace BLL.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;

    public PersonService(IPersonRepository repository)
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
        _repository.Save();
        var result = await _repository.GetPersonByIdAsync(person.Id);
        return result.Id;
    }
}