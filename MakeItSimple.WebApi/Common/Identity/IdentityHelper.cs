using System.Security.Claims;

namespace MakeItSimple.WebApi.Common.Identity
{
    public class IdentityHelper
    {
        public static bool TryGetUser(ClaimsIdentity identity , out Guid userId)
        {
            return Guid.TryParse(identity.FindFirst("id")?.Value, out userId);
        }

        public static string GetRole(ClaimsIdentity identity)
        {
            return identity.FindFirst(ClaimTypes.Role).Value;
        }
    }
}
