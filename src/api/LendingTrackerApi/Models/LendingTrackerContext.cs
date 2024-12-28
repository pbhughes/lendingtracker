using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LendingTrackerApi.Models;

public partial class LendingTrackerContext : DbContext
{
    public LendingTrackerContext()
    {
    }

    public LendingTrackerContext(DbContextOptions<LendingTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Borrower> Borrowers { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<StandardMessage> StandardMessages { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Borrower>(entity =>
        {
            entity.HasKey(e => e.BorrowerId).HasName("PK__Borrower__568EDB576270152D");

            entity.HasIndex(e => e.UserId, "IX_Borrowers_UserId");

            entity.HasIndex(e => new { e.BorrowerEmail, e.UserId }, "UQ__Borrower__User__14F16AF6B87CB05D").IsUnique();

            entity.Property(e => e.BorrowerId).ValueGeneratedNever();
            entity.Property(e => e.BorrowerEmail).HasMaxLength(100);
            entity.Property(e => e.BorrowerSms).HasMaxLength(15);
            entity.Property(e => e.CountryCode).HasMaxLength(5);
            entity.Property(e => e.IsEligible).HasDefaultValue(true);
            entity.Property(e => e.Name).HasDefaultValue("");

            entity.HasOne(d => d.User).WithMany(p => p.Borrowers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Borrowers__UserI__3E52440B");

            entity.HasIndex(i => new { i.BorrowerId, i.UserId })
            .IsUnique();
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Items__727E838BC5CFABC7");

            entity.HasIndex(e => e.OwnerId, "IX_Items_OwnerId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.ItemName).HasMaxLength(100);

            entity.HasOne(d => d.Owner).WithMany(p => p.Items)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK__Items__OwnerId__4316F928");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.HasIndex(e => e.ItemId, "IX_Message_ItemId");

            entity.HasIndex(e => e.TransactionId, "IX_Message_ItemId1");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.MessageDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Text).HasMaxLength(300);
            entity.Property(e => e.TransactionId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Item).WithMany(p => p.Messages).HasForeignKey(d => d.ItemId);

            entity.HasOne(d => d.Transaction).WithMany(p => p.Messages).HasForeignKey(d => d.TransactionId);
        });

        modelBuilder.Entity<StandardMessage>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Text).HasMaxLength(166);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6B92A192EE");

            entity.Property(e => e.TransactionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BorrowedAt).HasColumnType("datetime2");
            entity.Property(e => e.DueDate).HasColumnType("datetime2");
            entity.Property(e => e.ReturnedAt).HasColumnType("datetime2");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Borrower).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BorrowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Borro__47DBAE45");

            entity.HasOne(d => d.Item).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__ItemI__48CFD27E");

            entity.HasOne(d => d.Lender).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.LenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Lende__46E78A0C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C829177A8");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105344616AE11").IsUnique();

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CountryCode)
                .HasMaxLength(5)
                .HasDefaultValue("");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
