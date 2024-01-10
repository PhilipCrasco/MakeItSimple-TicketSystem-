using FluentValidation;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup.UpsertCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.AddNewTicket;

namespace MakeItSimple.WebApi.Models.Ticketing.TicketRequest
{
    public class TicketTransaction
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


        public bool? IsApprove { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public virtual User ApprovedByUser { get; set; }

        public string Remarks { get; set; }

        public ICollection<TicketConcern> TicketConcerns { get; set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }




    }
}
