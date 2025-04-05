using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class IdentityContext : IdentityDbContext<AppUser, AppRole, string >
    {
        // Constructor that accepts DbContextOptions and passes it to the base class
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }
        
    }
}
