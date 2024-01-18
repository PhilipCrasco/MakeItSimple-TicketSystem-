using FluentValidation;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest.AddNewTicket;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketConcern : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public int ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public string ConcernDetails { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }

        public bool? IsTransfer { get; set; }
        public DateTime? TransferAt { get; set; }
        public Guid? TransferBy { get; set; }
        public string TransferRemarks { get; set; }
        public virtual User TransferByUser { get; set; }

        public bool IsApprove { get; set; } = false;
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public virtual User ApprovedByUser { get; set; }

        public bool IsClosedApprove { get; set; } = false;
        public Guid? ClosedApproveBy { get; set; }
        public virtual User ClosedApproveByUser { get; set; }

        public bool IsClosedReject { get; set; }
        public string CloseRejectRemarks { get; set; }

        public bool IsReject { get; set; } = false;
        public string RejectRemarks { get; set; }

        public string Remarks { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime TargetDate { get; set; }

        public int? RequestGeneratorId { get; set; }
        public virtual RequestGenerator RequestGenerator { get; set; }

        public ICollection<ClosingTAttachment> ClosingTAttachments { get; set; }



    }
}
