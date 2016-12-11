﻿using Launchpad.Models.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Launchpad.Services.IdentityManagers
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store) : base(store)
        {
        }
    }
}
