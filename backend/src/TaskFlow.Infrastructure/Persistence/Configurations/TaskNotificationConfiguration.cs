using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations
{
    public sealed class TaskNotificationConfiguration : IEntityTypeConfiguration<TaskNotification>
    {
        public void Configure(EntityTypeBuilder<TaskNotification> builder)
        {
            builder.ToTable("notifications", "taskflow");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Message)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasIndex(x => x.EventId)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.ReciptientId,
                x.CreatedUtc
            });
        }
    }
}