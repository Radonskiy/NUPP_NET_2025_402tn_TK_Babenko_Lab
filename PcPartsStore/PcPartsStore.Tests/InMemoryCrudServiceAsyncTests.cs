using PcPartsStore.Common.Models;
using PcPartsStore.Common.Services;
using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace PcPartsStore.Tests
{
    public class InMemoryCrudServiceAsyncTests
    {
        private static string GetTempFilePath()
        {
            return Path.Combine(Path.GetTempPath(), $"pcpartstest_{Guid.NewGuid()}.json");
        }

        private sealed class TestEntity : IEntity
        {
            public Guid Id { get; init; } = Guid.NewGuid();
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        [Fact]
        public async Task CreateAsync_ShouldAddElement()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            var entity = new TestEntity { Name = "Test", Value = 10 };

            var result = await service.CreateAsync(entity);
            var items = await service.ReadAllAsync();

            Assert.True(result);
            Assert.Single(items);
            Assert.Equal(entity.Id, items.First().Id);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnElementById()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            var entity = new TestEntity { Name = "CPU", Value = 100 };
            await service.CreateAsync(entity);

            var result = await service.ReadAsync(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result!.Id);
            Assert.Equal("CPU", result.Name);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnNull_WhenElementDoesNotExist()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            var result = await service.ReadAsync(Guid.NewGuid());

            Assert.Null(result);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task ReadAllAsync_ShouldReturnAllElements()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            await service.CreateAsync(new TestEntity { Name = "A", Value = 1 });
            await service.CreateAsync(new TestEntity { Name = "B", Value = 2 });
            await service.CreateAsync(new TestEntity { Name = "C", Value = 3 });

            var items = (await service.ReadAllAsync()).ToList();

            Assert.Equal(3, items.Count);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task ReadAllAsync_WithPagination_ShouldReturnCorrectPage()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            await service.CreateAsync(new TestEntity { Name = "A", Value = 1 });
            await service.CreateAsync(new TestEntity { Name = "B", Value = 2 });
            await service.CreateAsync(new TestEntity { Name = "C", Value = 3 });
            await service.CreateAsync(new TestEntity { Name = "D", Value = 4 });
            await service.CreateAsync(new TestEntity { Name = "E", Value = 5 });

            var pageItems = (await service.ReadAllAsync(2, 2)).ToList();

            Assert.Equal(2, pageItems.Count);
            Assert.Equal("C", pageItems[0].Name);
            Assert.Equal("D", pageItems[1].Name);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingElement()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            var entity = new TestEntity { Name = "Old", Value = 1 };
            await service.CreateAsync(entity);

            var updated = new TestEntity
            {
                Id = entity.Id,
                Name = "New",
                Value = 999
            };

            var result = await service.UpdateAsync(updated);
            var fromService = await service.ReadAsync(entity.Id);

            Assert.True(result);
            Assert.NotNull(fromService);
            Assert.Equal("New", fromService!.Name);
            Assert.Equal(999, fromService.Value);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenElementDoesNotExist()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            var entity = new TestEntity
            {
                Name = "Missing",
                Value = 5
            };

            var result = await service.UpdateAsync(entity);

            Assert.False(result);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveExistingElement()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            var entity = new TestEntity { Name = "ToRemove", Value = 50 };
            await service.CreateAsync(entity);

            var result = await service.RemoveAsync(entity);
            var items = (await service.ReadAllAsync()).ToList();

            Assert.True(result);
            Assert.Empty(items);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task RemoveAsync_ShouldReturnFalse_WhenElementDoesNotExist()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            var entity = new TestEntity { Name = "Missing", Value = 10 };

            var result = await service.RemoveAsync(entity);

            Assert.False(result);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task SaveAsync_ShouldSaveItemsToFile_AndLoadFromFileInNewService()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            await service.CreateAsync(new TestEntity { Name = "Saved1", Value = 11 });
            await service.CreateAsync(new TestEntity { Name = "Saved2", Value = 22 });

            var saveResult = await service.SaveAsync();

            var newService = new InMemoryCrudServiceAsync<TestEntity>(filePath);
            var items = (await newService.ReadAllAsync()).ToList();

            Assert.True(saveResult);
            Assert.True(File.Exists(filePath));
            Assert.Equal(2, items.Count);
            Assert.Contains(items, x => x.Name == "Saved1");
            Assert.Contains(items, x => x.Name == "Saved2");

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public async Task Service_ShouldBeThreadSafe_WhenCreatingManyItemsConcurrently()
        {
            var filePath = GetTempFilePath();
            var service = new InMemoryCrudServiceAsync<TestEntity>(filePath);

            var tasks = Enumerable.Range(1, 100)
                .Select(i => service.CreateAsync(new TestEntity
                {
                    Name = $"Item{i}",
                    Value = i
                }));

            await Task.WhenAll(tasks);

            var items = (await service.ReadAllAsync()).ToList();

            Assert.Equal(100, items.Count);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}