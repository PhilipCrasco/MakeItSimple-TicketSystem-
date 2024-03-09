using MakeItSimple.WebApi.Models.Setup.ChannelSetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class ApproverTicketing
    {
        
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public int ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
        public Guid ? UserId { get; set; }
        public virtual User User { get; set; }
        public bool ? IsApprove { get; set; }
        public int? ApproverLevel { get; set; }

        public int? ReceiverId { get; set; }
        public int? CurrentLevel { get; set; }
        public int ? RequestGeneratorId { get; set; }
        public string Status { get; set; }
        public virtual RequestGenerator RequestGenerator { get; set; }
    }
}
