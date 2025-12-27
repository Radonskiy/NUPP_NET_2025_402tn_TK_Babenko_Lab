using System;
using System.Collections.Generic;
using System.Linq;

namespace PcPartsStore.Common.Services
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        private readonly List<T> _items = new();

        public T Create(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _items.Add(item);
            return item;
        }

        public T? Read(Guid id)
        {
            // Шукаємо властивість Id типу Guid
            return _items.FirstOrDefault(x =>
            {
                var prop = x.GetType().GetProperty("Id");
                if (prop is null) return false;

                var value = prop.GetValue(x);
                return value is Guid g && g == id;
            });
        }

        public IEnumerable<T> ReadAll() => _items.ToList();

        public T Update(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            // Беремо Id з item
            var prop = item.GetType().GetProperty("Id");
            if (prop is null) throw new InvalidOperationException("Об'єкт не має властивості Id.");

            var value = prop.GetValue(item);
            if (value is not Guid id) throw new InvalidOperationException("Властивість Id повинна бути типу Guid.");

            var existing = Read(id);
            if (existing is null) throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");

            _items.Remove(existing);
            _items.Add(item);

            return item;
        }

        public bool Remove(Guid id)
        {
            var existing = Read(id);
            if (existing is null) return false;

            _items.Remove(existing);
            return true;
        }
    }
}
