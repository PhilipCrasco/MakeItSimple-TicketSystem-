﻿using MakeItSimple.WebApi.Models.Ticketing.TicketRequest;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TicketAttachmentConfiguration : IEntityTypeConfiguration<TicketAttachment>
    {
        public void Configure(EntityTypeBuilder<TicketAttachment> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }


}
