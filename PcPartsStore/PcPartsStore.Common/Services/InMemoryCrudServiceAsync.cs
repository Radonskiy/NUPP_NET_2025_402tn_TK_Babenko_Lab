using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PcPartsStore.Common.Models;

namespace PcPartsStore.Common.Services;

public sealed class InMemoryCrudServiceAsync<T> : ICrudServiceAsync<T> where T : IEntity
{
    private readonly List<T> _items = new();
    private readonly SemaphoreSlim _mutex = new(1, 1);

    public string FilePath { get; }

    public InMemoryCrudServiceAsync(string filePath)
    {
        FilePath = filePath;

        // Завантаження при старті (якщо файл існує)
        if (File.Exists(FilePath))
        {
            try
            {
                var json = File.ReadAllText(FilePath);
                var loaded = JsonSerializer.Deserialize<List<T>>(json);

                if (loaded is not null)
                    _items.AddRange(loaded);
            }
            catch
            {
                // якщо файл битий/порожній — просто стартуємо з порожньої колекції
            }
        }
    }

    public async Task<bool> CreateAsync(T element)
    {
        await _mutex.WaitAsync();
        try
        {
            _items.Add(element);
            return true;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<T?> ReadAsync(Guid id)
    {
        await _mutex.WaitAsync();
        try
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        await _mutex.WaitAsync();
        try
        {
            return _items.ToList(); // snapshot
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1) page = 1;
        if (amount < 1) amount = 1;

        await _mutex.WaitAsync();
        try
        {
            return _items
                .Skip((page - 1) * amount)
                .Take(amount)
                .ToList(); // snapshot
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<bool> UpdateAsync(T element)
    {
        await _mutex.WaitAsync();
        try
        {
            var index = _items.FindIndex(x => x.Id == element.Id);
            if (index < 0) return false;

            _items[index] = element;
            return true;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<bool> RemoveAsync(T element)
    {
        await _mutex.WaitAsync();
        try
        {
            var removed = _items.RemoveAll(x => x.Id == element.Id);
            return removed > 0;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<bool> SaveAsync()
    {
        await _mutex.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _mutex.Release();
        }
    }

    // IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
        _mutex.Wait();
        try
        {
            return _items.ToList().GetEnumerator();
        }
        finally
        {
            _mutex.Release();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
