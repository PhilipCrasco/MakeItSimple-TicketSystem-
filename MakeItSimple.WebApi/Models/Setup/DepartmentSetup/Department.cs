using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.Models.Setup.DepartmentSetup
{
    public class Department : BaseEntity
    {
        public int Id { get; set ; }
        public bool IsActive { get ; set ; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get ; set ; }
        public Guid? AddedBy { get; set ; }
        public virtual User AddedByUser {  get; set ; }
        public Guid? ModifiedBy { get; set ; }
        public virtual User ModifiedByUser {  get; set ; }
        public int DepartmentNo { get; set ; }
        public string DepartmentCode { get; set ; }
        public string DepartmentName { get; set ; }
        public DateTime SyncDate { get; set ; }
        public string SyncStatus { get; set ; }
        public ICollection<User> Users { get; set ; }
        
       
        

    }
}
