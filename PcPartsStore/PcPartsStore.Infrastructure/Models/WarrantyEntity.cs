namespace PcPartsStore.Infrastructure.Models;

public class WarrantyEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;

    public int Months { get; set; }
}
