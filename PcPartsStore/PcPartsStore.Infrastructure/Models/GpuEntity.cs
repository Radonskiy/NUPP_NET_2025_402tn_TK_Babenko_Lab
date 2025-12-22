namespace PcPartsStore.Infrastructure.Models;

public class GpuEntity : ProductEntity
{
    public int MemoryGb { get; set; }
    public string Chipset { get; set; } = string.Empty;
    public int TdpWatts { get; set; }
}
