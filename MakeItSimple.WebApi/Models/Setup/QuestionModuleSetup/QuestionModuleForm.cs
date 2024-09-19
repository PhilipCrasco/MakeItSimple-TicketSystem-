using MakeItSimple.WebApi.Models.Setup.FormSetup;

namespace MakeItSimple.WebApi.Models.Setup.QuestionModuleSetup
{
    public class QuestionModuleForm
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }


        public int FormId { get; set; }
        public virtual Form Form { get; set; }

        public int QuestionModuleId { get; set; }
        public virtual QuestionModule QuestionModule { get; set; }





    }
}
