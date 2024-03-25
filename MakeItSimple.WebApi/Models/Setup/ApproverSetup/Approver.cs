using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.TeamSetup;

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

        public int ? SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public int ? ChannelId { get; set; }    
        public virtual Channel Channel { get; set; }

        //public int? TeamId { get; set; }
        //public virtual Team Team { get; set; }

        //public int ? ReceiverId { get; set; }  

        public Guid ? UserId { get; set; }
        public virtual User User { get; set; }
        public int ? ApproverLevel { get; set; }


    }
}
