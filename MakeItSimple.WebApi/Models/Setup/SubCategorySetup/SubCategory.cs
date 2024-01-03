using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup.UpsertCategory;

namespace MakeItSimple.WebApi.Models.Setup.SubCategorySetup
{
    public partial class SubCategory : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public string SubCategoryDescription {  get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
