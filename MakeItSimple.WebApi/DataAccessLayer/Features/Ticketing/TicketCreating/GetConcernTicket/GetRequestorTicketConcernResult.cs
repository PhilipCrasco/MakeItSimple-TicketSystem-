namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket
{
    public partial class GetRequestorTicketConcern
    {
        public class GetRequestorTicketConcernResult
        {

            public int? RequestConcernId { get; set; }
            public string Concern { get; set; }

            public string Resolution { get; set; }

            public int? CompanyId { get; set; }
            public string Company_Code { get; set; }
            public string Company_Name { get; set; }

            public int? BusineesUnitId { get; set; }
            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }

            public int? DepartmentId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }

            public int? UnitId { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }

            public int? SubUnitId { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }

            public int? LocationId { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }

            public Guid? RequestorId { get; set; }
            public string FullName { get; set; }
            public int? CategoryId { get; set; }
            public string Category_Description { get; set; }
            public int? SubCategoryId { get; set; }
            public string SubCategory_Description { get; set; }
            public string Concern_Status { get; set; }
            public bool? Is_Done { get; set; }
            public string Remarks { get; set; }
            public string Notes { get; set; }
            public DateTime? Date_Needed { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? updated_At { get; set; }
            public bool? Is_Confirmed { get; set; }
            public DateTime? Confirmed_At { get; set; }

            public List<TicketRequestConcern> TicketRequestConcerns { get; set; }
            public class TicketRequestConcern
            {

                public int? TicketConcernId { get; set; }
                public int? ChannelId { get; set; }
                public string Channel_Name { get; set; }

                public Guid? UserId { get; set; }
                public string Issue_Handler { get; set; }
                public DateTime? Target_Date { get; set; }
                public bool? Is_Assigned { get; set; }
                public string Remarks { get; set; }
                public string Concern_Type { get; set; }
                public string Closing_Notes { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
                public bool Is_Active { get; set; }
                public DateTime? Closed_At { get; set; }
                public bool? Is_Transfer { get; set; }
                public DateTime? Transfer_At { get; set; }
                public string Transfer_By { get; set; }

            }

        }
    }
}