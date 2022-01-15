using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class PersonRepository : IPersonRepository, IDisposable
{
    private AppContext _context;

    public PersonRepository(AppContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Person>> GetAllPersonsAsync()
    {
        return await _context.Persons.Include(x => x.Address).ToListAsync();
    }

    public async Task<Person> GetPersonByIdAsync(long personId)
    {
        return await _context.Persons.Include(x => x.Address).FirstOrDefaultAsync(x => x.Id == personId) ?? throw new InvalidOperationException();
    }

    public void InsertPerson(Person person)
    {
        _context.Persons.Add(person);
    }

    public async Task DeletePersonAsync(long personId)
    {
        var person = await GetPersonByIdAsync(personId);
        _context.Persons.Remove(person);
    }

    public void UpdatePerson(Person person)
    {
        _context.Entry(person).State = EntityState.Modified;
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected void Dispose(bool disposing)
    {
        if (!disposing) return;
        _context.Dispose();
        _context = null;
    }
}