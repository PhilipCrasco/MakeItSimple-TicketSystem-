namespace MakeItSimple.WebApi.Common
{
    public class ContentType
    {
        public string GetContentType(string fileName)
        {
            return fileName switch
            {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".csv" => "text/csv",
                // Add more types as needed
                _ => "application/octet-stream", // Default for unknown types
            };
        }
    }
}
