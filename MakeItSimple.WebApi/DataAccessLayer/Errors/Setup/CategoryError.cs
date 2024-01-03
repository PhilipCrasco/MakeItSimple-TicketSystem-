using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class CategoryError
    {
      public static Error CategoryAlreadyExist(string Category) =>
      new Error("Category.CategoryAlreadyExist", $"Category description {Category} already exist!");
      public static Error CategoryNotExist() =>
      new Error("Category.CategoryNotExist", $"Category not exist!");

      public static Error CategoryNochanges() =>
      new Error("Category.CategoryNochanges", "No changes has made!");

      public static Error CategoryIsUse(string category) =>
      new Error("Category.CategoryIsUse", $"Category {category} is use!");

    }
}
