using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Models;

public partial class InventoryManagementSystemContext : DbContext
{
    public InventoryManagementSystemContext()
    {
    }

    public InventoryManagementSystemContext(DbContextOptions<InventoryManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Expireditem> Expireditems { get; set; }

    public virtual DbSet<Lowstock> Lowstocks { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<Productinsight> Productinsights { get; set; }

    public virtual DbSet<ReturnItem> ReturnItems { get; set; }

    public virtual DbSet<Salestrend> Salestrends { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionDetail> TransactionDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK_customers_CustomerID");

            entity.ToTable("customers", "inventorymanagementsystem");

            entity.HasIndex(e => e.Email, "customers$Email_UNIQUE").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Age).HasMaxLength(10);
            entity.Property(e => e.ContactNumer).HasMaxLength(15);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.FullName).HasMaxLength(100);
        });

        modelBuilder.Entity<Expireditem>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("expireditems", "inventorymanagementsystem");

            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasPrecision(0);
            entity.Property(e => e.ItemId)
                .HasMaxLength(50)
                .HasColumnName("ItemID");
            entity.Property(e => e.ItemName).HasMaxLength(150);
            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ProductID");
            entity.Property(e => e.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<Lowstock>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("lowstock", "inventorymanagementsystem");

            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasPrecision(0);
            entity.Property(e => e.ItemId)
                .HasMaxLength(50)
                .HasColumnName("ItemID");
            entity.Property(e => e.ItemName).HasMaxLength(150);
            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ProductID");
            entity.Property(e => e.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK_payment method_PaymentMethodID");

            entity.ToTable("payment method", "inventorymanagementsystem");

            entity.HasIndex(e => e.MethodName, "payment method$MethodName_UNIQUE").IsUnique();

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.MethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_product_ProductID");

            entity.ToTable("product", "inventorymanagementsystem");

            entity.HasIndex(e => e.CategoryId, "CategoryID_idx");

            entity.HasIndex(e => e.ItemId, "product$ItemID_UNIQUE").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.CategoryId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ExpiryDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.ItemId)
                .HasMaxLength(50)
                .HasColumnName("ItemID");
            entity.Property(e => e.ItemName).HasMaxLength(150);
            entity.Property(e => e.ManufactureDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Price)
               .HasMaxLength(100)
               .HasDefaultValueSql("(NULL)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("product$CategoryID");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_product category_CategoryID");

            entity.ToTable("product category", "inventorymanagementsystem");

            entity.HasIndex(e => e.CategoryName, "product category$CategoryName_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CreatedAt, "product category$CreatedAt_UNIQUE").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<Productinsight>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("productinsights", "inventorymanagementsystem");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ItemName).HasMaxLength(150);
            entity.Property(e => e.Revenue).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<ReturnItem>(entity =>
        {
            entity.HasKey(e => e.ReturnItemsId).HasName("PK_return items_ReturnItemsID");

            entity.ToTable("return items", "inventorymanagementsystem");

            entity.HasIndex(e => e.ProductId, "ProductID_idx");

            entity.HasIndex(e => e.TransactionId, "TransactionID_idx");

            entity.Property(e => e.ReturnItemsId).HasColumnName("ReturnItemsID");
            entity.Property(e => e.ProductId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("ProductID");
            entity.Property(e => e.ReturnDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(8)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.TransactionId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("TransactionID");

            entity.HasOne(d => d.Product).WithMany(p => p.ReturnItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("return items$ReturnProductID");

            entity.HasOne(d => d.Transaction).WithMany(p => p.ReturnItems)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("return items$ReturnTransactionID");
        });

        modelBuilder.Entity<Salestrend>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("salestrend", "inventorymanagementsystem");

            entity.Property(e => e.TotalSales).HasColumnType("decimal(38, 0)");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK_transactions_TransactionID");

            entity.ToTable("transactions", "inventorymanagementsystem");

            entity.HasIndex(e => e.CustomerId, "CustomerID_idx");

            entity.HasIndex(e => e.PaymentMethodId, "PaymentMethodID_idx");

            entity.HasIndex(e => e.UserId, "UserID_idx");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(10)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.TotalAmount)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("decimal(10, 0)");
            entity.Property(e => e.TransactionDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactions$CustomerID");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactions$PaymentMethodID");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactions$UserID");
        });

        modelBuilder.Entity<TransactionDetail>(entity =>
        {
            entity.HasKey(e => e.TransactionDetailsId).HasName("PK_transaction details_TransactionDetailsID");

            entity.ToTable("transaction details", "inventorymanagementsystem");

            entity.HasIndex(e => e.ProductId, "ProductID_idx");

            entity.HasIndex(e => e.TransactionId, "TransactionID_idx");

            entity.Property(e => e.TransactionDetailsId).HasColumnName("TransactionDetailsID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("ProductID");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TransactionId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("TransactionID");

            entity.HasOne(d => d.Product).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("transaction details$ProductID");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("transaction details$TransactionID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_users_UserID");

            entity.ToTable("users", "inventorymanagementsystem");

            entity.HasIndex(e => e.Email, "users$Email_UNIQUE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.PhoneNumber)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("Phone Number");
            entity.Property(e => e.Role)
                .HasMaxLength(5)
                .HasDefaultValueSql("(NULL)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
