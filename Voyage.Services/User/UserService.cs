﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Voyage.Core;
using Voyage.Core.Exceptions;
using Voyage.Data.Repositories.UserPhone;
using Voyage.Models;
using Voyage.Models.Entities;
using Voyage.Services.IdentityManagers;
using Voyage.Services.Role;
using Microsoft.AspNet.Identity;

namespace Voyage.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserPhoneRepository _phoneRepository;
        private readonly IRoleService _roleService;
        private readonly ApplicationUserManager _userManager;
        private readonly IMapper _mapper;

        public UserService(ApplicationUserManager userManager, IMapper mapper, IRoleService roleService, IUserPhoneRepository phoneRepository)
        {
            _userManager = userManager.ThrowIfNull(nameof(userManager));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _roleService = roleService.ThrowIfNull(nameof(roleService));
            _phoneRepository = phoneRepository.ThrowIfNull(nameof(phoneRepository));
        }

        public async Task<UserModel> UpdateUserAsync(string userId, UserModel model)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
                throw new NotFoundException($"Could not locate entity with Id {userId}");

            _mapper.Map<UserModel, ApplicationUser>(model, appUser);

            CollectionHelpers.MergeCollection(
                _mapper,
                source: model.Phones,
                destination: appUser.Phones,
                predicate: (s, d) => s.Id == d.Id,
                deleteAction: entity => _phoneRepository.Delete(entity.Id));

            await _userManager.UpdateAsync(appUser);
            return _mapper.Map<UserModel>(appUser);
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleId)
        {
            var roleModel = _roleService.GetRoleById(roleId);
            if (roleModel == null)
            {
                return null;
            }

            var result = await _userManager.RemoveFromRoleAsync(userId, roleModel.Name);
            return result;
        }

        public async Task<RoleModel> AssignUserRoleAsync(string userId, RoleModel roleModel)
        {
            var identityResult = await _userManager.AddToRoleAsync(userId, roleModel.Name);
            if (!identityResult.Succeeded)
                throw new BadRequestException();

            return _roleService.GetRoleByName(roleModel.Name);
        }

        public IEnumerable<UserModel> GetUsers()
        {
            var users = _userManager.Users.Where(user => !user.Deleted).ToList();
            return _mapper.Map<IEnumerable<UserModel>>(users);
        }

        public async Task<bool> IsValidCredential(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);
            return user != null && user.IsActive && !user.Deleted;
        }

        public async Task<UserModel> CreateUserAsync(UserModel model)
        {
            var appUser = new ApplicationUser();
            _mapper.Map<UserModel, ApplicationUser>(model, appUser);
            IdentityResult result = await _userManager.CreateAsync(appUser, "Hello123!");
            if (!result.Succeeded)
                throw new BadRequestException();

            return _mapper.Map<UserModel>(appUser);
        }

        public async Task<IdentityResult> RegisterAsync(RegistrationModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsActive = true
            };

            IdentityResult identityResult = await _userManager.CreateAsync(user, model.Password);

            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRoleAsync(user.Id, "Basic");
            }

            return identityResult;
        }

        public async Task<IEnumerable<ClaimModel>> GetUserClaimsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"Could not locate entity with Id {userId}");

            var identity = await CreateClaimsIdentityAsync(user.UserName, "OAuth");
            return _mapper.Map<IEnumerable<ClaimModel>>(identity.Claims);
        }

        public async Task<ClaimsIdentity> CreateClaimsIdentityAsync(string userName, string authenticationType)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new NotFoundException($"Could not locate entity with Id {userName}");

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await _userManager.CreateIdentityAsync(user, authenticationType);

            // Add in role claims
            var userRoles = _userManager.GetRoles(user.Id);
            var roleClaims = userRoles.Select(_ => _roleService.GetRoleClaims(_))
                .SelectMany(_ => _)
                .Select(_ => new Claim(_.ClaimType, _.ClaimValue));
            userIdentity.AddClaims(roleClaims);

            return userIdentity;
        }

        public async Task<ClaimsIdentity> CreateJwtClaimsIdentityAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new NotFoundException($"Could not locate entity with Id {userName}");

            var identity = new ClaimsIdentity("JWT");

            // Add in role claims
            var userRoles = _userManager.GetRoles(user.Id);
            var roleClaims = userRoles.Select(_ => _roleService.GetRoleClaims(_))
                .SelectMany(_ => _)
                .Select(_ => new Claim(_.ClaimType, _.ClaimValue));
            identity.AddClaims(roleClaims);

            return identity;
        }

        /// <summary>
        /// This create client claims based on test data. Your application will need to create claim based on your business rule
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> CreateClientClaimsIdentityAsync(string clientId)
        {
            // TODO: This create client claims based on test data.Your application will need to create claim based on your business rule
            var identity = new ClaimsIdentity("JWT");
            if (Clients.Client2.Id == clientId || Clients.Client1.Id == clientId)
            {
                var user = await _userManager.FindByNameAsync("admin@admin.com");
                if (user == null)
                    throw new NotFoundException($"Could not locate entity with Id admin@admin.com");

                // Add in role claims
                var userRoles = _userManager.GetRoles(user.Id);
                var roleClaims = userRoles.Select(_ => _roleService.GetRoleClaims(_))
                    .SelectMany(_ => _)
                    .Select(_ => new Claim(_.ClaimType, _.ClaimValue));
                identity.AddClaims(roleClaims);
            }

            return identity;
        }

        public async Task<IEnumerable<RoleModel>> GetUserRolesAsync(string userId)
        {
            var roles = await _userManager.GetRolesAsync(userId);
            var roleModels = _roleService.GetRoles().Where(_ => roles.Contains(_.Name));
            return roleModels;
        }

        public RoleModel GetUserRoleById(string userId, string roleId)
        {
            var roleModel = _roleService.GetRoleById(roleId);
            if (roleModel == null)
                throw new NotFoundException($"Could not locate entity with Id {roleId}");

            if (!_userManager.IsInRole(userId, roleModel.Name))
                throw new UnauthorizedException($"User not authorized for role {roleModel.Name}");

            return roleModel;
        }

        public async Task<UserModel> GetUserAsync(string userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null || appUser.Deleted)
                throw new NotFoundException($"Could not locate entity with ID {userId}");

            return _mapper.Map<UserModel>(appUser);
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
                throw new NotFoundException($"Could not locate entity with ID {userId}");

            var identityResult = await _userManager.DeleteAsync(appUser);
            return identityResult;
        }
    }
}
