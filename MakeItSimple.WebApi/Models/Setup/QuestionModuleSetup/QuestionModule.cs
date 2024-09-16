namespace MakeItSimple.WebApi.Models.Setup.QuestionModuleSetup
{
    public class QuestionModule
    {
        public int Id { get; set; }
        public string Question_Modules_Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public ICollection<QuestionModuleForm> QuestionModuleForms { get; set; }

        //public ICollection<int> FormId { get; set; }
        //public virtual Form Form { get; set; }



    }
}
