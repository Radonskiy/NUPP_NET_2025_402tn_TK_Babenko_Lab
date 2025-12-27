using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PcPartsStore.Infrastructure;

public class PcPartsStoreContextFactory : IDesignTimeDbContextFactory<PcPartsStoreContext>
{
    public PcPartsStoreContext CreateDbContext(string[] args)
    {
        // EF Tools запускає StartupProject (Console),
        // тому AppContext.BaseDirectory == ...\PcPartsStore.Console\bin\Debug\net10.0\
        var dbPath = Path.Combine(AppContext.BaseDirectory, "pcpartsstore.db");

        var options = new DbContextOptionsBuilder<PcPartsStoreContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        return new PcPartsStoreContext(options);
    }
}
