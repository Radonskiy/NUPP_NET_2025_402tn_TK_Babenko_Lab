namespace PcPartsStore.Infrastructure.Models;

public class BrandEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<ProductEntity> Products { get; set; } = new();
}
