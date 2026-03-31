using System;
using System.Collections.Generic;
using System.Text;

namespace PcPartsStore.Common.Services
{
    public interface ICrudService<T>
    {
        T Create(T item);
        T? Read(Guid id);
        IEnumerable<T> ReadAll();
        T Update(T item);
        bool Remove(Guid id);

        void Save(string filePath);
        void Load(string filePath);
    }
}