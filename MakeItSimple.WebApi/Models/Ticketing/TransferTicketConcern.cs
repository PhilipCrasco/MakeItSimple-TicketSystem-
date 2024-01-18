using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TransferTicketConcern : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public int SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public int ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public string ConcernDetails { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? TargetDate { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public bool? IsTransfer { get; set; }
        public DateTime? TransferAt { get; set; }
        public Guid? TransferBy { get; set; }
        public string TransferRemarks { get; set; }
        public virtual User TransferByUser { get; set; }


    }



}
