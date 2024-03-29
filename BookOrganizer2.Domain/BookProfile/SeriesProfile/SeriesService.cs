﻿using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using BookOrganizer2.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BookOrganizer2.Domain.BookProfile.SeriesProfile.Commands;

namespace BookOrganizer2.Domain.BookProfile.SeriesProfile
{
    public class SeriesService : ISeriesDomainService
    {
        public IRepository<Series, SeriesId> Repository { get; }

        public SeriesService(IRepository<Series, SeriesId> repository)
            => Repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public Series CreateItem() => Series.NewSeries;

        public Task Handle(object command)
        {
            return command switch
            {
                Create cmd => HandleCreate(cmd),
                Update cmd => HandleFullUpdate(cmd),
                SetSeriesName cmd => HandleUpdate(cmd.Id, (a) => a.SetName(cmd.Name),
                    (a) => Repository.Update(a)),
                SetPicturePath cmd => HandleUpdate(cmd.Id, (a) => a.SetPicturePath(cmd.PicturePath),
                    (a) => Repository.Update(a)),
                SetDescription cmd => HandleUpdate(cmd.Id, (a) => a.SetDescription(cmd.Description),
                    (a) => Repository.Update(a)),
                SetReadOrder cmd => HandleSeriesBooksUpdate(cmd),
                Delete cmd => HandleDeleteAsync(cmd),
                _ => Task.CompletedTask
            };
        }

        public async Task<Series> AddNew(Series model)
        {
            var command = new Create
            {
                Id = new SeriesId(SequentialGuid.NewSequentialGuid()),
                Name = model.Name,
                PicturePath = model.PicturePath,
                Description = model.Description,
                Books = model.Books
            };

            await Handle(command);

            return await Repository.GetAsync(command.Id);
        }

        public Guid GetId(SeriesId id) => id?.Value ?? Guid.Empty;
        public Task Update(Series model)
        {
            var command = new Update
            {
                Id = model.Id,
                Name = model.Name,
                PicturePath = model.PicturePath,
                Description = model.Description,
                Books = model.Books
            };

            return Handle(command);
        }

        public Task RemoveAsync(SeriesId id)
        {
            var command = new Delete
            {
                Id = id
            };

            return Handle(command);
        }

        private async Task HandleCreate(Create cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var series = Series.Create(cmd.Id, cmd.Name, cmd.PicturePath, cmd.Description, cmd.Books);

            await Repository.AddAsync(series);

            if (series.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }

            if (cmd.Books is not null && cmd.Books.Count > 0)
            {
                await UpdateSeriesReadOrder(series, cmd.Books).ConfigureAwait(false);
            }
        }

        private async Task HandleFullUpdate(Update cmd)
        {
            if (!await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} was not found! Update cannot finish.");

            var updatableSeries = await Repository.GetAsync(cmd.Id);

            updatableSeries.SetName(cmd.Name);
            if (!string.IsNullOrWhiteSpace(cmd.PicturePath))
            {
                updatableSeries.SetPicturePath(cmd.PicturePath);
            }
            updatableSeries.SetDescription(cmd.Description);
            updatableSeries.SetBooks(cmd.Books);

            Repository.Update(updatableSeries);

            if (updatableSeries.EnsureValidState())
            {
                await Repository.SaveAsync();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private async Task HandleUpdate(Guid id, Action<Series> operation, Action<Series> operation2 = null)
        {
            if (await Repository.ExistsAsync(id))
            {
                var series = await Repository.GetAsync(id);
                operation(series);
                operation2?.Invoke(series);

                if (series.EnsureValidState())
                {
                    await Repository.SaveAsync();
                }
            }
            else
                throw new ArgumentException();
        }

        private async Task HandleSeriesBooksUpdate(SetReadOrder cmd)
        {
            if (await Repository.ExistsAsync(cmd.Id))
            {
                var series = await ((ISeriesRepository)Repository).LoadAsync(cmd.Id);

                if (cmd.Books.Count > 0)
                {
                    await UpdateSeriesReadOrder(series, cmd.Books).ConfigureAwait(false);
                }
            }
            else
                throw new ArgumentException();
        }

        private Task UpdateSeriesReadOrder(Series series, ICollection<ReadOrder> books)
        {
            return ((ISeriesRepository)Repository).ChangeReadOrder(series, books);
        }

        private async Task HandleDeleteAsync(Delete cmd)
        {
            if (!await Repository.ExistsAsync(cmd.Id))
                throw new InvalidOperationException($"Entity with id {cmd.Id} was not found! Update cannot finish.");

            try
            {
                await Repository.RemoveAsync(cmd.Id);
                await Repository.SaveAsync();
            }
            catch (Exception)
            {
                throw new ArgumentNullException();
            }
        }
    }
}
