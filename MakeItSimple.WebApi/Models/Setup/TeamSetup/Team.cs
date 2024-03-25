using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;

namespace MakeItSimple.WebApi.Models.Setup.TeamSetup
{
    public class Team : BaseEntity
    {

        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int? SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public string TeamName { get; set; }



        //public ICollection<Channel> Channels { get; set; }

        //public ICollection<Approver> Approvers { get; set; }


    }
}
