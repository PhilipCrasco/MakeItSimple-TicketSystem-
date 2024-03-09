using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;

namespace MakeItSimple.WebApi.Models.Setup
{
    public class Receiver  : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public int ? BusinessUnitId { get; set; }
        public virtual BusinessUnit BusinessUnit { get; set; }
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
