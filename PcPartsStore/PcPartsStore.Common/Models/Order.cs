using System;
using System.Collections.Generic;
using System.Linq;

namespace PcPartsStore.Common.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Тут має бути Product, НЕ ProductEntity
    public List<Product> Items { get; set; } = new();

    public void AddItem(Product item) => Items.Add(item);

    public bool RemoveItem(Guid productId)
    {
        var item = Items.FirstOrDefault(x => x.Id == productId);
        if (item is null) return false;

        Items.Remove(item);
        return true;
    }

    public decimal GetTotal() => Items.Sum(x => x.Price);
}
