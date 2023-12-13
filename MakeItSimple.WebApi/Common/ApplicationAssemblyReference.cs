using System.Reflection;

namespace MakeItSimple.WebApi.Common
{
    public static class ApplicationAssemblyReference
    {
        public static Assembly Assembly => typeof(Program).Assembly;
    }
}
