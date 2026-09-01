namespace InventoryManagementSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Product
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .Property(p => p.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Purchase
        modelBuilder.Entity<Purchase>()
            .Property(p => p.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Purchase>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Purchases)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // PurchaseItem
        modelBuilder.Entity<PurchaseItem>()
            .Property(pi => pi.UnitCost)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Purchase)
            .WithMany(p => p.PurchaseItems)
            .HasForeignKey(pi => pi.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.PurchaseItems)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sale
        modelBuilder.Entity<Sale>()
            .Property(s => s.TotalAmount)
            .HasColumnType("decimal(18,2)");

        // SaleItem
        modelBuilder.Entity<SaleItem>()
            .Property(si => si.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<SaleItem>()
            .HasOne(si => si.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SaleItem>()
            .HasOne(si => si.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}