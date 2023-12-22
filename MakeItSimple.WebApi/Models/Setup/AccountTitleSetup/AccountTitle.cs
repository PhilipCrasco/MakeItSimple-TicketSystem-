namespace MakeItSimple.WebApi.Models.Setup.AccountTitleSetup
{
    public class AccountTitle
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int AccountNo { get; set; }
        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public DateTime SyncDate { get; set; }

        public string SyncStatus { get; set; }

    }
}
