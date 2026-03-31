namespace PcPartsStore.Common.Models;

public class Cpu : Product
{
    public int Cores { get; set; }
    public int Threads { get; set; }
    public decimal BaseClockGhz { get; set; }

    public Cpu() { }

    public Cpu(string name, decimal price, int stock, int cores, int threads, decimal baseClockGhz)
        : base(name, price, stock)
    {
        Cores = cores;
        Threads = threads;
        BaseClockGhz = baseClockGhz;
    }

    public static Cpu CreateNew()
    {
        var rnd = Random.Shared;

        return new Cpu(
            name: $"CPU-{rnd.Next(1000, 9999)}",
            price: rnd.Next(3000, 20000),
            stock: rnd.Next(0, 50),
            cores: rnd.Next(4, 24),
            threads: rnd.Next(8, 48),
            baseClockGhz: Math.Round((decimal)(rnd.NextDouble() * 3.0 + 2.0), 2)
        );
    }

    public override string GetInfo()
        => base.GetInfo() + $", Cores: {Cores}, Threads: {Threads}, Base: {BaseClockGhz} GHz";
}
