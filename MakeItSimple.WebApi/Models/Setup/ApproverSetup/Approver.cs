using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;

namespace MakeItSimple.WebApi.Models.Setup.ApproverSetup
{
    public class Approver : BaseEntity
    {

        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public int ChannelId { get; set; }    
        public virtual Channel Channel { get; set; }

        public int ? ReceiverId { get; set; }  

        public Guid ? UserId { get; set; }
        public virtual User User { get; set; }
        public int ? ApproverLevel { get; set; }


    }
}
