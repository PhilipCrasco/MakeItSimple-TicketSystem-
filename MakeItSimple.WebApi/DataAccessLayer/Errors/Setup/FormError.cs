using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class FormError
    {
        public static Error FormNameExist() =>
        new Error("Form.FormNameExist", "Form name already exist!");
        public static Error FormNotExist() =>
        new Error("Form.FormNotExist", "Form not exist!");

    }
}
