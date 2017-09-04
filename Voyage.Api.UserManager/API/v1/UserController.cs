﻿using System.Net;
using System.Net.Http;
using Voyage.Core;
using Voyage.Models;
using System.Threading.Tasks;
using System.Web.Http;
using Voyage.Api.UserManager.Filters;
using Voyage.Services.User;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Voyage.Api.UserManager.API.V1
{
    [RoutePrefix(RoutePrefixConstants.RoutePrefixes.V1)]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService.ThrowIfNull(nameof(userService));
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse(200, "IEnumerable<UserModel>", typeof(IEnumerable<UserModel>))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [ClaimAuthorize(ClaimValue = AppClaims.ListUsers)]
        [HttpGet]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            var users = _userService.GetUsers();
            return Ok(users);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [SwaggerResponse(200, "UserModel", typeof(UserModel))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [ClaimAuthorize(ClaimValue = AppClaims.UpdateUser)]
        [HttpPut]
        [Route("users/{userId}")]
        public async Task<IHttpActionResult> UpdateUser([FromUri] string userId, [FromBody] UserModel userModel)
        {
            var result = await _userService.UpdateUserAsync(userId, userModel);
            return Ok(result);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [SwaggerResponse(204)]
        [SwaggerResponse(401, "UnauthorizedException")]
        [SwaggerResponse(400, "BadRequestException")]
        [ClaimAuthorize(ClaimValue = AppClaims.DeleteUser)]
        [HttpDelete]
        [Route("users/{userId}")]
        public async Task<IHttpActionResult> DeleteUser([FromUri] string userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (!result.Succeeded)
                return BadRequest();

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [SwaggerResponse(201, "UserModel", typeof(UserModel))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [ClaimAuthorize(ClaimValue = AppClaims.CreateUser)]
        [HttpPost]
        [Route("users")]
        public async Task<IHttpActionResult> CreateUser(UserModel user)
        {
            var result = await _userService.CreateUserAsync(user);
            return CreatedAtRoute("GetUserAsync", new { UserId = result.Id }, result);
        }

        /// <summary>
        /// Get a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [SwaggerResponse(200, "UserModel", typeof(UserModel))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [SwaggerResponse(404, "NotFoundException")]
        [ClaimAuthorize(ClaimValue = AppClaims.ViewUser)]
        [HttpGet]
        [Route("users/{userId}", Name = "GetUserAsync")]
        public async Task<IHttpActionResult> GetUser(string userId)
        {
            var result = await _userService.GetUserAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [SwaggerResponse(200, "IEnumerable<UserModel>", typeof(IEnumerable<UserModel>))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [ClaimAuthorize(ClaimValue = AppClaims.ListUserRole)]
        [Route("users/{userId}/roles")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserRoles(string userId)
        {
            var result = await _userService.GetUserRolesAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Get user permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [SwaggerResponse(200, "IEnumerable<ClaimModel>", typeof(IEnumerable<ClaimModel>))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [ClaimAuthorize(ClaimValue = AppClaims.ListUserPermissions)]
        [HttpGet]
        [Route("users/{userId}/permissions")]
        public async Task<IHttpActionResult> GetClaims(string userId)
        {
            var result = await _userService.GetUserClaimsAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleModel"></param>
        /// <returns></returns>
        [SwaggerResponse(201, "RoleModel", typeof(RoleModel))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [SwaggerResponse(400, "BadRequestException")]
        [ClaimAuthorize(ClaimValue = AppClaims.AssignUserRole)]
        [HttpPost]
        [Route("users/{userId}/roles")]
        public async Task<IHttpActionResult> AssignRole([FromUri] string userId, RoleModel roleModel)
        {
            var result = await _userService.AssignUserRoleAsync(userId, roleModel);
            return CreatedAtRoute("GetUserRoleById", new { UserId = userId, RoleId = result.Id }, result);
        }

        /// <summary>
        /// Get role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [SwaggerResponse(200, "IEnumerable<RoleModel>", typeof(IEnumerable<RoleModel>))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [ClaimAuthorize(ClaimValue = AppClaims.ViewUserRole)]
        [HttpGet]
        [Route("users/{userId}/roles/{roleId}", Name = "GetUserRoleById")]
        public IHttpActionResult GetUserRoleById(string userId, string roleId)
        {
            var result = _userService.GetUserRoleById(userId, roleId);
            return Ok(result);
        }

        /// <summary>
        /// Remove a role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [SwaggerResponse(200, "IdentityResult", typeof(IdentityResult))]
        [SwaggerResponse(401, "UnauthorizedException")]
        [SwaggerResponse(400, "BadRequestException")]
        [ClaimAuthorize(ClaimValue = AppClaims.DeleteUserRole)]
        [HttpDelete]
        [Route("users/{userId}/roles/{roleId}")]
        public async Task<IHttpActionResult> RemoveRole([FromUri] string userId, [FromUri] string roleId)
        {
            var result = await _userService.RemoveUserFromRoleAsync(userId, roleId);
            return Ok(result);
        }
    }
}
