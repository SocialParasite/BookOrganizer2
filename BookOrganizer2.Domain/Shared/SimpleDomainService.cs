using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;

namespace BookOrganizer2.Domain.Shared
{
    //public class SimpleDomainService<T, TId> : ISimpleDomainService<T, TId> where T : class
    //{
    //    // TODO: Add Name prop to interface
    //    // command.Id ?

    //    public IRepository<T, TId> Repository { get; }

    //    public SimpleDomainService(IRepository<T, TId> repository)
    //        => Repository = repository ?? throw new ArgumentNullException(nameof(repository));

    //    public async Task<T> AddNew(T model)
    //    {
    //        var command = new Commands.Create
    //        {
    //            Id = new NationalityId(SequentialGuid.NewSequentialGuid()),
    //            Name = model.Name
    //        };

    //        await Handle(command);

    //        return await Repository.GetAsync(command.Id);
    //    }

    //    public Task Handle(object command)
    //    {
    //        return command switch
    //        {
    //            Commands.Create cmd => HandleCreate(cmd),
    //            Commands.Update cmd => HandleUpdate(cmd),
    //            Commands.Delete cmd => HandleDeleteAsync(cmd),
    //            _ => Task.CompletedTask
    //        };
    //    }

    //    public T CreateItem()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Guid GetId(TId id) => id?.Value ?? Guid.Empty;

    //    public Task RemoveAsync(TId id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task Update(T model)
    //    {
    //        var command = new Events.Updated
    //        {
    //            Id = model.Id,
    //            Name = model.Name
    //        };

    //        return Handle(command);
    //    }

    //    private async Task HandleCreate(Commands.Create cmd)
    //    {
    //        if (await Repository.ExistsAsync(cmd.Id))
    //            throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

    //        T item = T.Create(cmd.Id, cmd.Name);

    //        await Repository.AddAsync(item);

    //        if (item.EnsureValidState())
    //        {
    //            await Repository.SaveAsync();
    //        }
    //        else
    //        {
    //            throw new ArgumentNullException();
    //        }
    //    }

    //    private async Task HandleUpdate(Commands.Update cmd)
    //    {
    //        if (!await Repository.ExistsAsync(cmd.Id))
    //            throw new InvalidOperationException($"Entity with id {cmd.Id} was not found! Update cannot finish.");

    //        var updatableItem = await Repository.GetAsync(cmd.Id);

    //        updatableItem.SetName(cmd.Name);

    //        Repository.Update(updatableItem);

    //        if (updatableItem.EnsureValidState())
    //        {
    //            await Repository.SaveAsync();
    //        }
    //        else
    //        {
    //            throw new ArgumentNullException();
    //        }
    //    }
    //    private async Task HandleDeleteAsync(Commands.Delete cmd)
    //    {
    //        if (!await Repository.ExistsAsync(cmd.Id))
    //            throw new InvalidOperationException($"Entity with id {cmd.Id} was not found! Update cannot finish.");

    //        try
    //        {
    //            await Repository.RemoveAsync(cmd.Id);
    //            await Repository.SaveAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new ArgumentNullException();
    //        }
    //    }
    //}
}
