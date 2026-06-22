using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class EH_SEDO_AssignmentContext(DbContextOptions<EH_SEDO_AssignmentContext> options) : IdentityDbContext<EH_SEDO_Assignment.Data.ApplicationUser>(options)
{
}
