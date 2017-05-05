﻿using System.Net;
using Voyage.Core;
using Voyage.Models;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Voyage.Api.Filters;
using Voyage.Services.User;

namespace Voyage.Api.API.V1
{
    [RoutePrefix(Constants.RoutePrefixes.V1)]
    public class AccountController : ApiController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService.ThrowIfNull(nameof(userService));
        }

        /**
        * @api {post} /v1/account/register Register a new account
        * @apiVersion 0.1.0
        * @apiName CreateAccount
        * @apiGroup Account
        *
        * @apiPermission none
        * @apiSampleRequest http://qa-api-ms.voyageframework.com/api/v1/account/register
        * @apiParam {String} email User's email
        * @apiParam {String} password User's password
        * @apiParam {String} confirmPassword User's password (x2)
        * @apiParam {String} firstName First name
        * @apiParam {String} lastName Last name
        *
        * @apiSuccessExample Success-Response:
        *      HTTP/1.1 204 NO CONTENT
        *
        * @apiUse BadRequestError
        */
        [ClaimAuthorize(ClaimValue = Constants.AppClaims.CreateUser)]
        [Route("account/register")]
        public async Task<IHttpActionResult> Register(RegistrationModel model)
        {
            IdentityResult result = await _userService.RegisterAsync(model);
            if (!result.Succeeded)
                return BadRequest();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
