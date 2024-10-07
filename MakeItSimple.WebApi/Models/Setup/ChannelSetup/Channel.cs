using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;


namespace MakeItSimple.WebApi.Models.Setup.ChannelSetup
{
    public partial class Channel : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public  Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public string ChannelName { get; set; }

        public int? SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public int ? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public Guid ? UserId { get; set; }
        public virtual User User { get; set; }


        
        public ICollection<ChannelUser> ChannelUsers { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
