using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class SubCategoryError
    {
        public static Error SubCategoryAlreadyExist(string SubCategoryDescription) =>
        new Error("SubCategory.SubCategoryAlreadyExist", $"Sub category name {SubCategoryDescription} already exist!");

        public static Error CategoryNotExist() =>
        new Error("SubCategory.CategoryNotExist", "Category doesn't exist!");

        public static Error SubCategoryNotExist() =>
        new Error("SubCategory.SubCategoryNotExist", "Sub category doesn't exist!");

       public static Error SubcategoryNochanges() =>
      new Error("Subcategory.SubcategoryNochanges", "No changes has made!");

    }
}
