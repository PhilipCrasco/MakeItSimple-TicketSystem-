﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;

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

        public int? ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public string ConcernDetails { get; set; }

        public int ? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int ? SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }

        //public DateTime? StartDate { get; set; }

        //public DateTime? TargetDate { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public bool? IsTransfer { get; set; }
        public DateTime? TransferAt { get; set; }
        public Guid? TransferBy { get; set; }
        public string TransferRemarks { get; set; }
        public virtual User TransferByUser { get; set; }

        public bool IsRejectTransfer { get; set; }
        public DateTime? RejectTransferAt { get; set; }
        public Guid? RejectTransferBy { get; set; }
        public virtual User RejectTransferByUser { get; set; }
        public string RejectRemarks { get; set; }
        public Guid? TicketApprover { get; set; }
        public int? ReceiverId { get; set; }

        public int? RequestTransactionId { get; set; }
        public virtual RequestTransaction RequestTransaction { get; set; }

        public int? TicketTransactionId { get; set; }
        public virtual TicketTransaction TicketTransaction { get; set; }

 

        public string Remarks { get; set; }
        public string TicketNo { get; set; }

    }



}
