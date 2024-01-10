using FluentValidation;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.AddNewTicket;

namespace MakeItSimple.WebApi.Models.Ticketing.TicketRequest
{
    public class TicketConcern : BaseEntity
    {
        public int Id {  get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int  SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public int  ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public int  ChannelUserId { get; set; }
        public virtual ChannelUser ChannelUser { get; set; }

        public string ConcernDetails { get; set; }

        public int  CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int  SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }    

        public bool ? IsClosedApprove { get; set; }
        public Guid ? ClosedApproveBy { get; set; }
        public virtual User ClosedApproveByUser {  get; set; }

        public string Remarks { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime TargetDate { get; set; }

        public int ? TicketTransactionId { get; set; }
        public virtual TicketTransaction TicketTransaction { get; set; }
        public ICollection<ClosingTAttachment> ClosingTAttachments{ get; set;}



    }
}
