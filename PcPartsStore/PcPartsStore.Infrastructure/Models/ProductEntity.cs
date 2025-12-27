using System;
using PcPartsStore.Infrastructure.Models;


namespace PcPartsStore.Infrastructure.Models;

public class ProductEntity
{
    public int Id { get; set; }                 // (якщо в тебе було інакше — залиш своє)
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // 1-many (Brand -> Products)
    public int BrandId { get; set; }
    public BrandEntity Brand { get; set; } = null!;

    // 1-1 (Product -> Warranty)
    public WarrantyEntity? Warranty { get; set; }

    // many-to-many (Orders <-> Products)
    public List<OrderEntity> Orders { get; set; } = new();


}
