using System;
using System.Collections.Generic;

namespace PcPartsStore.Infrastructure.Models;

public class OrderEntity
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // many-to-many (Orders <-> Products)
    public List<ProductEntity> Products { get; set; } = new();
}
