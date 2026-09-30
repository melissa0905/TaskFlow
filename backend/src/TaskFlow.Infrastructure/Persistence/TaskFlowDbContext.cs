using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence
{
    public class TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : DbContext(options)
    {
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<TeamMember> TeamMembers { get; set; } = null!;
        public DbSet<WorkTask> WorkTasks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("taskflow");

            modelBuilder.Entity<Project>(entity =>
             {
                 entity.ToTable("projects");
                 entity.HasKey(p => p.Id);
                 entity.Property(p => p.Name).HasMaxLength(150).IsRequired();
                 entity.Property(p => p.Description).HasMaxLength(1000);

                 entity.HasMany(p => p.WorkTasks)
                       .WithOne(t => t.Project)
                       .HasForeignKey(t => t.ProjectId)
                       .OnDelete(DeleteBehavior.Cascade);
             });

            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.ToTable("team_members");
                entity.HasKey(tm => tm.Id);
                entity.Property(tm => tm.FullName).HasMaxLength(150).IsRequired();
                entity.Property(tm => tm.Email).HasMaxLength(255).IsRequired();
                entity.HasIndex(x => x.Email)
                               .IsUnique();
                entity.HasMany(tm => tm.AssignedTasks)
                      .WithOne(t => t.AssignedTo)
                      .HasForeignKey(t => t.AssignedToId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<WorkTask>(entity =>
            {
                entity.ToTable("work_tasks");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
                entity.Property(t => t.Description).HasMaxLength(2000);
                entity.Property(x => x.Status)
                             .HasConversion<int>();
                entity.HasIndex(x => new { x.ProjectId, x.Status });
                entity.HasIndex(x => x.DueDate);
            });
        }



    }
}