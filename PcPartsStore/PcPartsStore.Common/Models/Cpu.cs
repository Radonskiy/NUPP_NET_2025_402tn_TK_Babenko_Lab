namespace PcPartsStore.Common.Models;

public class Cpu : Product
{
    public int Cores { get; set; }
    public int Threads { get; set; }
    public decimal BaseClockGhz { get; set; }

    public Cpu() { }

    public Cpu(string name, decimal price, int cores, int threads, decimal baseClockGhz)
        : base(name, price)
    {
        Cores = cores;
        Threads = threads;
        BaseClockGhz = baseClockGhz;
    }

    public override string GetInfo()
        => base.GetInfo() + $", Cores: {Cores}, Threads: {Threads}, Base: {BaseClockGhz} GHz";
}
