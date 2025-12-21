namespace PcPartsStore.Common.Models;

public class Gpu : Product
{
    public int MemoryGb { get; set; }
    public string Chipset { get; set; } = string.Empty;
    public int TdpWatts { get; set; }

    public Gpu() { }

    public Gpu(string name, decimal price, int memoryGb, string chipset, int tdpWatts)
        : base(name, price)
    {
        MemoryGb = memoryGb;
        Chipset = chipset;
        TdpWatts = tdpWatts;
    }

    public override string GetInfo()
    => base.GetInfo() + $", VRAM: {MemoryGb} GB, Chipset: {Chipset}, TDP: {TdpWatts}W";

}
