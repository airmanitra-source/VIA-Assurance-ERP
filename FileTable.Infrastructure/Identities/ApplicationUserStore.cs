using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FileTable.Infrastructure.Identities
{
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
    {
        private readonly string _connectionString;

        public ApplicationUserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("FileTableConnection") ?? throw new Exception("No connexion");
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "INSERT INTO [documentdb].[dbo].[AppUser] (UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash, SecurityStamp, EntrepriseId) VALUES (@UserName, @NormalizedUserName, @Email, @NormalizedEmail, @PasswordHash, @SecurityStamp, @EntrepriseId); SELECT CAST(SCOPE_IDENTITY() as bigint);";
            var id = await connection.ExecuteScalarAsync<long>(sql, user);
            user.Id = id.ToString();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("DELETE FROM [documentdb].[dbo].[AppUser] WHERE Id = @Id", new { user.Id });
            return IdentityResult.Success;
        }

        public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM [documentdb].[dbo].[AppUser] WHERE Id = @Id", new { Id = userId });
        }

        public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM [documentdb].[dbo].[AppUser] WHERE NormalizedUserName = @NormalizedUserName",
                new { NormalizedUserName = normalizedUserName });
        }

        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "UPDATE [documentdb].[dbo].[AppUser] SET UserName = @UserName, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail, PasswordHash = @PasswordHash, SecurityStamp = @SecurityStamp, EntrepriseId = @EntrepriseId WHERE Id = @Id",
                user);
            return IdentityResult.Success;
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT * FROM [documentdb].[dbo].[AppUser] WHERE NormalizedEmail = @NormalizedEmail",
                new { NormalizedEmail = normalizedEmail });
        }

        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
             using var connection = new SqlConnection(_connectionString);
             var roleId = await connection.ExecuteScalarAsync<string>("SELECT Id FROM [documentdb].[dbo].[UserRole] WHERE NormalizedName = @NormalizedName", new { NormalizedName = roleName.ToUpper() });
             
             if (roleId != null)
             {
                 await connection.ExecuteAsync("INSERT INTO [documentdb].[dbo].[EntrepriseUserRole] (UserId, RoleId) VALUES (@UserId, @RoleId)", new { UserId = user.Id, RoleId = roleId });
             }
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
            var roleId = await connection.ExecuteScalarAsync<string>("SELECT Id FROM [documentdb].[dbo].[UserRole] WHERE NormalizedName = @NormalizedName", new { NormalizedName = roleName.ToUpper() });

            if (roleId != null)
            {
                 await connection.ExecuteAsync("DELETE FROM [documentdb].[dbo].[EntrepriseUserRole] WHERE UserId = @UserId AND RoleId = @RoleId", new { UserId = user.Id, RoleId = roleId });
            }
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
             var roles = await connection.QueryAsync<string>(@"
                SELECT r.Name 
                FROM [documentdb].[dbo].[UserRole] r 
                INNER JOIN [documentdb].[dbo].[EntrepriseUserRole] ur ON r.Id = ur.RoleId 
                WHERE ur.UserId = @UserId", 
                new { UserId = user.Id });

             return roles.ToList();
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);
            var count = await connection.ExecuteScalarAsync<int>(@"
                SELECT COUNT(1) 
                FROM [documentdb].[dbo].[UserRole] r 
                INNER JOIN [documentdb].[dbo].[EntrepriseUserRole] ur ON r.Id = ur.RoleId 
                WHERE ur.UserId = @UserId AND r.NormalizedName = @NormalizedName", 
                new { UserId = user.Id, NormalizedName = roleName.ToUpper() });
            
            return count > 0;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
             using var connection = new SqlConnection(_connectionString);
             var users = await connection.QueryAsync<ApplicationUser>(@"
                SELECT u.* 
                FROM [documentdb].[dbo].[AppUser] u
                INNER JOIN [documentdb].[dbo].[EntrepriseUserRole] ur ON u.Id = ur.UserId
                INNER JOIN [documentdb].[dbo].[UserRole] r ON ur.RoleId = r.Id
                WHERE r.NormalizedName = @NormalizedName",
                new { NormalizedName = roleName.ToUpper() });
             return users.ToList();
        }

        public async Task<IList<ApplicationUser>> GetUsersAsync(CancellationToken cancellationToken)
        {
             using var connection = new SqlConnection(_connectionString);
             var users = await connection.QueryAsync<ApplicationUser>("SELECT u.*, e.RaisonSocial FROM [documentdb].[dbo].[AppUser] u LEFT JOIN [documentdb].[dbo].[Entreprise] e ON u.EntrepriseId = e.Id");
             return users.ToList();
        }

        public async Task<long> CreateEntrepriseAsync(string raisonSocial, string? email)
        {
             using var connection = new SqlConnection(_connectionString);
             var sql = "INSERT INTO [documentdb].[dbo].[Entreprise] (RaisonSocial, Email) VALUES (@RaisonSocial, @Email); SELECT CAST(SCOPE_IDENTITY() as bigint);";
             var id = await connection.ExecuteScalarAsync<long>(sql, new { RaisonSocial = raisonSocial, Email = email });
             return id;
        }

        void IDisposable.Dispose()
        {
            // Nothing to dispose
        }
    }
}
