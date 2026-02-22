using Microsoft.AspNetCore.Identity;

namespace FileTable.Infrastructure.Identities
{
    public class ApplicationUser: IdentityUser
    {
        public long EntrepriseId { get; set; }

        public bool InitialPasswordResetCompleted { get; set; } = false;

        public string RaisonSocial { get; set; } = string.Empty;
    }
}
