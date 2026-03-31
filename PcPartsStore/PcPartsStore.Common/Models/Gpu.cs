namespace PcPartsStore.Common.Models;

public class Gpu : Product
{
    public int MemoryGb { get; set; }
    public string Chipset { get; set; } = string.Empty;
    public int TdpWatts { get; set; }

    public Gpu() { }

    public Gpu(string name, decimal price, int stock, int memoryGb, string chipset, int tdpWatts)
        : base(name, price, stock)
    {
        MemoryGb = memoryGb;
        Chipset = chipset;
        TdpWatts = tdpWatts;
    }

    public static Gpu CreateNew()
    {
        var rnd = Random.Shared;

        var chipset = (rnd.Next(0, 2) == 0) ? "NVIDIA" : "AMD";

        return new Gpu(
            name: $"GPU-{rnd.Next(1000, 9999)}",
            price: rnd.Next(5000, 40000),
            stock: rnd.Next(0, 40),
            memoryGb: rnd.Next(6, 24),
            chipset: chipset,
            tdpWatts: rnd.Next(120, 450)
        );
    }

    public override string GetInfo()
        => base.GetInfo() + $", VRAM: {MemoryGb} GB, Chipset: {Chipset}, TDP: {TdpWatts}W";
}
