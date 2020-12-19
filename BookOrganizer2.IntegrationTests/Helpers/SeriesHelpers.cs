using System.Collections.Generic;
using System.Threading.Tasks;
using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Commands = BookOrganizer2.Domain.BookProfile.SeriesProfile.Commands;

namespace BookOrganizer2.IntegrationTests.Helpers
{
    public static class SeriesHelpers
    {
        public static async Task<Series> CreateValidSeries()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);

            var seriesService = new SeriesService(repository);

            var command = new Commands.Create
            {
                Id = new SeriesId(SequentialGuid.NewSequentialGuid()),
                Name = "Series to be",
                PicturePath = @"\\filepath\file.jpg",
                Description = "Best books in the world!"
            };

            await seriesService.Handle(command);
            return await repository.GetAsync(command.Id);
        }

        public static async Task<Series> CreateValidSeriesWithBooks()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);

            var seriesService = new SeriesService(repository);
            
            var book1 = await BookHelpers.CreateValidBook("Book 1");
            var book2 = await BookHelpers.CreateValidBook("Book 2");
            var readOrder = new List<ReadOrder>
            {
                ReadOrder.NewReadOrder(book1, null, 1),
                ReadOrder.NewReadOrder(book2, null, 2)
            };

            var command = new Commands.Create
            {
                Id = new SeriesId(SequentialGuid.NewSequentialGuid()),
                Name = "Series to be",
                PicturePath = @"\\filepath\file.jpg",
                Description = "Best books in the world!",
                Books = readOrder
            };

            await seriesService.Handle(command);
            return await repository.GetAsync(command.Id);
        }
        
        internal static Task UpdateSeries(Series sut)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);

            var seriesService = new SeriesService(repository);
            var command = new Commands.Update
            {
                Id = sut.Id,
                Name = sut.Name,
                PicturePath = sut.PicturePath,
                Description = sut.Description,
                Books = sut.Books
            };

            return seriesService.Handle(command);
        }

        public static Task CreateInvalidSeries()
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);
            var seriesService = new SeriesService(repository);

            var seriesId = new SeriesId(SequentialGuid.NewSequentialGuid());
            var command = new Commands.Create { Id = seriesId };

            return seriesService.Handle(command);
        }

        public static Task UpdateSeriesName(SeriesId id, string firstName)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);

            var seriesService = new SeriesService(repository);
            var command = new Commands.SetSeriesName
            {
                Id = id,
                Name = firstName
            };

            return seriesService.Handle(command);
        }

        public static Task UpdateSeriesPicturePath(SeriesId id, string path)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);

            var seriesService = new SeriesService(repository);
            var command = new Commands.SetPicturePath
            {
                Id = id,
                PicturePath = path
            };

            return seriesService.Handle(command);
        }

        public static Task UpdateSeriesDescription(SeriesId id, string description)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);

            var seriesService = new SeriesService(repository);
            var command = new Commands.SetDescription()
            {
                Id = id,
                Description = description
            };

            return seriesService.Handle(command);
        }

        public static Task UpdateSeriesReadOrder(SeriesId seriesId, ICollection<ReadOrder> newReadOrder)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);

            var seriesService = new SeriesService(repository);
            var command = new Commands.SetReadOrder
            {
                Id = seriesId,
                Books = newReadOrder
            };

            return seriesService.Handle(command);
        }

        // DELETE

        public static Task RemoveSeries(SeriesId id)
        {
            var connectionString = ConnectivityService.GetConnectionString("TEMP");
            var context = new BookOrganizer2DbContext(connectionString);
            var repository = new SeriesRepository(context);

            var seriesService = new SeriesService(repository);
            var command = new Commands.DeleteSeries
            {
                Id = id,
            };

            return seriesService.Handle(command);
        }
    }
}
