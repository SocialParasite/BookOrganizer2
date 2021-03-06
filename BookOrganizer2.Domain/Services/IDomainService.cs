﻿using BookOrganizer2.Domain.DA;
using System;
using System.Threading.Tasks;

namespace BookOrganizer2.Domain.Services
{
    public interface IDomainService<T, in TId> where T : class
    {
        IRepository<T, TId> Repository { get; }
        T CreateItem();
        Task<T> AddNew(T model);
        Guid GetId(TId id);

        // BaseDetailViewModel
        Task SaveChanges() => Repository.SaveAsync();
        Task RemoveAsync(TId id);
        bool HasChanges() => Repository.HasChanges();
        Task Update(T entity);
    }
}
