using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RetryMachine.SQL.Models;

public partial class RetrymachineContext : DbContext
{
    public RetrymachineContext()
    {
    }

    public RetrymachineContext(DbContextOptions<RetrymachineContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RetryTask> RetryTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RetryTask>(entity =>
        {
            entity.ToTable("RetryTask");

            entity.Property(e => e.ActionOn).HasColumnType("datetime");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
