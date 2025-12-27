using Microsoft.EntityFrameworkCore;
using PcPartsStore.Infrastructure.Models;

namespace PcPartsStore.Infrastructure;

public class PcPartsStoreContext : DbContext
{
    public PcPartsStoreContext(DbContextOptions<PcPartsStoreContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<CpuEntity> Cpus => Set<CpuEntity>();
    public DbSet<GpuEntity> Gpus => Set<GpuEntity>();

    public DbSet<BrandEntity> Brands => Set<BrandEntity>();
    public DbSet<WarrantyEntity> Warranties => Set<WarrantyEntity>();

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------------------------
        // Product (base)  -> Products
        // -------------------------
        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");
        });

        // -------------------------
        // TPT (inheritance)
        // CpuEntity -> Cpus
        // GpuEntity -> Gpus
        // -------------------------
        modelBuilder.Entity<CpuEntity>(entity =>
        {
            entity.ToTable("Cpus");
            // якщо є якісь поля CPU — тут можна додати constraints
        });

        modelBuilder.Entity<GpuEntity>(entity =>
        {
            entity.ToTable("Gpus");

            entity.Property(x => x.Chipset)
                .IsRequired()
                .HasMaxLength(100);
        });

        // -------------------------
        // Brand 1-many Products
        // -------------------------
        modelBuilder.Entity<BrandEntity>(entity =>
        {
            entity.ToTable("Brands");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<ProductEntity>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // Warranty 1-1 Product
        // -------------------------
        modelBuilder.Entity<WarrantyEntity>(entity =>
        {
            entity.ToTable("Warranties");

            entity.HasKey(x => x.Id);

            entity.HasOne(w => w.Product)
                .WithOne(p => p.Warranty)
                .HasForeignKey<WarrantyEntity>(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // -------------------------
        // Orders + many-to-many Orders <-> Products
        // -------------------------
        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.ToTable("Orders");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<OrderEntity>()
            .HasMany(o => o.Products)
            .WithMany(p => p.Orders)
            .UsingEntity(j => j.ToTable("OrderProducts"));
    }
}
