using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TicketOnHoldConfiguration: IEntityTypeConfiguration<TicketOnHold>
    {
        public void Configure(EntityTypeBuilder<TicketOnHold> builder)
        {
            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);
        }
    

    }

}
