﻿using Launchpad.Models.EntityFramework;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Launchpad.Data.IntegrationTests.Extensions
{
    public static class LaunchpadContextExtensions
    {
        public static ActivityAudit AddActivityAudit(this LaunchpadDataContext context)
        {
            var audit = new ActivityAudit
            {
                RequestId = System.Guid.NewGuid().ToString(),
                IpAddress = "1.1.1.1",
                UserName = "bob@bob.com",
                Method = "PATCH",
                Path = "/widget/1",
                Date = System.DateTime.Now
            };

            context.ActivityAudits.Add(audit);
            context.SaveChanges();
            return audit;
        }

        public static Widget AddWidget(this LaunchpadDataContext context)
        {
            var widget = new Widget { Name = "My Test Widget", Color="Blue" };
            context.Widgets.Add(widget);
            context.SaveChanges();
            return widget;
        }

        public static ApplicationRole AddRole(this LaunchpadDataContext context)
        {
            var role = new ApplicationRole { Name = "MyRoleName" };
            var roleStore = new RoleStore<ApplicationRole>(context);
            roleStore.CreateAsync(role).Wait();
            return role;
        }
        
        public static RoleClaim AddRoleClaim(this LaunchpadDataContext context, ApplicationRole role, string type = "type1", string value = "value1")
        {
            var roleClaim = new RoleClaim
            {
                ClaimType = "type1",
                ClaimValue = "value1",
                RoleId = role.Id
            };
            context.RoleClaims.Add(roleClaim);
            context.SaveChanges();
            return roleClaim;
        }
    }
}
