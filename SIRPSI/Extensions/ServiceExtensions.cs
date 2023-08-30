using DataAccess.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace SIRPSI.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddErrorDescriberExtension(this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddErrorDescriber<SpanishIdentityErrorDescriber>();
        }
    }
}