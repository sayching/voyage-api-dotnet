﻿using Launchpad.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Launchpad.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IdentityResult> CreateRoleAsync(RoleModel model);

        IEnumerable<RoleModel> GetRoles();

        IEnumerable<ClaimModel> GetRoleClaims(string name);

        Task AddClaimAsync(RoleModel role, ClaimModel claim);

        Task<IdentityResult> RemoveRoleAsync(RoleModel role);

        void RemoveClaim(RoleModel roleModel, ClaimModel claimModel);
    }
}
