using Microsoft.AspNetCore.Identity;

namespace FileTable.Infrastructure.Identities
{
    public class ApplicationUser: IdentityUser
    {
        public string RaisonSocial { get; set; } = string.Empty;
        public long EntrepriseId { get; set; }
    }
}
