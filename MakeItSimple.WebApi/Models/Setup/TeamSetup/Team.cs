using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;


namespace MakeItSimple.WebApi.Models.Setup.TeamSetup
{
    public partial class Team : BaseEntity
    {
        public int Id {  get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public string TeamCode { get; set; }
        public string TeamName { get; set; }
        public ICollection<Channel> Channels { get; set; }

    }

}
