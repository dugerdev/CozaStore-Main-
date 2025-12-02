
using Microsoft.AspNetCore.Identity;

namespace CozaStore.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
}

public class ApplicationRole : IdentityRole<Guid>
{
    public string Description { get; set; }
}

public class ApplicationUserRole : IdentityUserRole<Guid> { }

public class ApplicationUserClaim : IdentityUserClaim<Guid> { }
public class ApplicationRoleClaim : IdentityRoleClaim<Guid> { }
public class ApplicationUserLogin : IdentityUserLogin<Guid> { }

public class ApplicationUserToken : IdentityUserToken<Guid> { }