﻿using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.Models.Ticketing.TicketRequest
{
    public class TicketAttachment 
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public string Attachment {  get; set; }
        public int ? TicketTransactionId { get; set; }
        public virtual TicketTransaction TicketTransaction { get; set; }

    }
}
