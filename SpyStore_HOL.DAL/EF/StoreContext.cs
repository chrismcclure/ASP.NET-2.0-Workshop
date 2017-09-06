﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SpyStore_HOL.Models.Entities;

namespace SpyStore_HOL.DAL.EF
{
    public class StoreContext : DbContext
    {
        internal StoreContext() { }
        public StoreContext(DbContextOptions options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = @"Server=(localdb)\mssqllocaldb;Database=SpyStore_HOL2;Trusted_Connection=True;MultipleActiveResultSets= true;";
            if (!optionsBuilder.IsConfigured)
            {       //optionsBuilder.UseSqlServer(connectionString);       
                optionsBuilder
                    .UseSqlServer(connectionString, options => options.EnableRetryOnFailure())
                    .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
            }
        }
        [DbFunction("GetOrderTotal", Schema = "Store")]
        public static int GetOrderTotal(int orderId)
        {   
            //code in here doesn’t matter   
            throw new Exception();
        } 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.EmailAddress).HasName("IX_Customers").IsUnique();
            });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ShipDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.LineItemTotal)
                .HasColumnType("money")
                .HasComputedColumnSql("[Quantity]*[UnitCost]");
                entity.Property(e => e.UnitCost).HasColumnType("money");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.UnitCost).HasColumnType("money");
                entity.Property(e => e.CurrentPrice).HasColumnType("money");
            });
            modelBuilder.Entity<ShoppingCartRecord>(entity =>
            {
                entity.HasIndex(e => new
                {
                    ShoppingCartRecordId = e.Id,
                    e.ProductId,
                    e.CustomerId
                }).HasName("IX_ShoppingCart").IsUnique();
                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
                entity.Property(e => e.Quantity)
                .ValueGeneratedNever()
                .HasDefaultValue(1);
            });

            modelBuilder.Entity<Order>(entity => 
            {
                entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ShipDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

                entity.Property(e => e.OrderTotal)
                .HasColumnType("money")
                .HasComputedColumnSql("Store.GetOrderTotal([Id])");
            });
        }

       

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartRecord> ShoppingCartRecords { get; set; }
    }
}
