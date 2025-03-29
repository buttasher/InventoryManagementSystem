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

    public virtual DbSet<Paymentmethod> Paymentmethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productcategory> Productcategories { get; set; }

    public virtual DbSet<Returnitem> Returnitems { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Transactiondetail> Transactiondetails { get; set; }

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
            entity.HasKey(e => e.ExpiredItemId).HasName("PK_expireditems_ExpiredItemId");

            entity.ToTable("expireditems", "inventorymanagementsystem");

            entity.HasIndex(e => e.ItemId, "fk_expireditems_products");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ItemName).HasMaxLength(255);

            entity.HasOne(d => d.Item).WithMany(p => p.Expireditems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("expireditems$fk_expireditems_products");
        });

        modelBuilder.Entity<Lowstock>(entity =>
        {
            entity.HasKey(e => e.LowStockId).HasName("PK_lowstock_LowStockId");

            entity.ToTable("lowstock", "inventorymanagementsystem");

            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ItemName).HasMaxLength(255);
        });

        modelBuilder.Entity<Paymentmethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK_paymentmethod_PaymentMethodID");

            entity.ToTable("paymentmethod", "inventorymanagementsystem");

            entity.HasIndex(e => e.MethodName, "paymentmethod$MethodName_UNIQUE").IsUnique();

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.MethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_products_ProductID");

            entity.ToTable("products", "inventorymanagementsystem");

            entity.HasIndex(e => e.CategoryId, "CategoryID_idx");

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
            entity.Property(e => e.ItemName).HasMaxLength(150);
            entity.Property(e => e.ManufactureDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Price)
                .HasMaxLength(100)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValueSql("(NULL)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("products$CategoryID");
        });

        modelBuilder.Entity<Productcategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_productcategory_CategoryID");

            entity.ToTable("productcategory", "inventorymanagementsystem");

            entity.HasIndex(e => e.CategoryName, "productcategory$CategoryName_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CreatedAt, "productcategory$CreatedAt_UNIQUE").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<Returnitem>(entity =>
        {
            entity.HasKey(e => e.ReturnItemsId).HasName("PK_returnitems_ReturnItemsID");

            entity.ToTable("returnitems", "inventorymanagementsystem");

            entity.HasIndex(e => e.ProductId, "ProductID_idx");

            entity.HasIndex(e => e.TransactionId, "TransactionID_idx");

            entity.Property(e => e.ReturnItemsId).HasColumnName("ReturnItemsID");
            entity.Property(e => e.ProductId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("ProductID");
            entity.Property(e => e.ReturnDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
           
            entity.Property(e => e.TransactionId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("TransactionID");

            entity.HasOne(d => d.Product).WithMany(p => p.Returnitems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("returnitems$ReturnProductID");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Returnitems)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("returnitems$ReturnTransactionID");
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

            entity.Property(e => e.Tax)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.Subtotal)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("decimal(10,2)");


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

        modelBuilder.Entity<Transactiondetail>(entity =>
        {
            entity.HasKey(e => e.TransactionDetailsId).HasName("PK_transactiondetails_TransactionDetailsID");

            entity.ToTable("transactiondetails", "inventorymanagementsystem");

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

            entity.HasOne(d => d.Product).WithMany(p => p.Transactiondetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("transactiondetails$ProductID");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Transactiondetails)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("transactiondetails$TransactionID");
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
