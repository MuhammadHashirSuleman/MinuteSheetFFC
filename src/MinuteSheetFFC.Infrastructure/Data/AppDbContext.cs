using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Domain.Entities;

namespace MinuteSheetFFC.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<RequestType> RequestTypes => Set<RequestType>();
    public DbSet<MinuteSheetRequest> MinuteSheetRequests => Set<MinuteSheetRequest>();
    public DbSet<WorkflowStage> WorkflowStages => Set<WorkflowStage>();
    public DbSet<WorkflowHistory> WorkflowHistory => Set<WorkflowHistory>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<WorkflowRule> WorkflowRules => Set<WorkflowRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee
        modelBuilder.Entity<Employee>(e =>
        {
            e.HasKey(x => x.PNo);
            e.Property(x => x.PNo).HasMaxLength(20);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Designation).HasMaxLength(200);
            e.HasOne(x => x.Manager).WithMany(x => x.Subordinates).HasForeignKey(x => x.ManagerPNo).OnDelete(DeleteBehavior.Restrict);
        });

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Username).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeePNo).OnDelete(DeleteBehavior.SetNull);
        });

        // UserRole
        modelBuilder.Entity<UserRole>(e =>
        {
            e.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
        });

        // RequestType
        modelBuilder.Entity<RequestType>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        // MinuteSheetRequest
        modelBuilder.Entity<MinuteSheetRequest>(e =>
        {
            e.HasIndex(x => x.ReferenceNumber).IsUnique();
            e.Property(x => x.ReferenceNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Subject).HasMaxLength(500).IsRequired();
            e.Property(x => x.EstimatedBudget).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Requester).WithMany().HasForeignKey(x => x.RequesterPNo).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CurrentActioner).WithMany().HasForeignKey(x => x.CurrentActionerPNo).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.RequestType).WithMany().HasForeignKey(x => x.RequestTypeId).OnDelete(DeleteBehavior.Restrict);
        });

        // WorkflowStage
        modelBuilder.Entity<WorkflowStage>(e =>
        {
            e.HasOne(x => x.MinuteSheet).WithMany(x => x.Stages).HasForeignKey(x => x.MinuteSheetId);
            e.Property(x => x.ActionerPNo).HasMaxLength(20).IsRequired();
        });

        // WorkflowHistory
        modelBuilder.Entity<WorkflowHistory>(e =>
        {
            e.HasOne(x => x.MinuteSheet).WithMany(x => x.History).HasForeignKey(x => x.MinuteSheetId);
            e.Property(x => x.ActionerPNo).HasMaxLength(20).IsRequired();
        });

        // Attachment
        modelBuilder.Entity<Attachment>(e =>
        {
            e.HasOne(x => x.MinuteSheet).WithMany(x => x.Attachments).HasForeignKey(x => x.MinuteSheetId);
            e.Property(x => x.FileName).HasMaxLength(500).IsRequired();
        });

        // WorkflowRule
        modelBuilder.Entity<WorkflowRule>(e =>
        {
            e.Property(x => x.BudgetFrom).HasColumnType("decimal(18,2)");
            e.Property(x => x.BudgetTo).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.RequestType).WithMany().HasForeignKey(x => x.RequestTypeId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
