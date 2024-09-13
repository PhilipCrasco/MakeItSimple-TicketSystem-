using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.Models.Setup.FormSetup
{
    public class Form : BaseEntity
    {
        public int Id { get; set; }
        public string Form_Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

    }
}
