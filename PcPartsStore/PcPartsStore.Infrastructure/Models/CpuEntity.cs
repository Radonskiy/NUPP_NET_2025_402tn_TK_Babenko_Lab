namespace PcPartsStore.Infrastructure.Models;

public class CpuEntity : ProductEntity
{
    public int Cores { get; set; }
    public int Threads { get; set; }
    public decimal BaseClockGhz { get; set; }
}
