﻿using Launchpad.Web.App_Start;
using Launchpad.Web.AuthProviders;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;

namespace Launchpad.Web
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }

        /**
        * @api {post} /Token Get an authentication token 
        * @apiVersion 0.1.0
        * @apiName Token
        * @apiGroup Token
        * 
        * @apiParam {String} grant_type=password Authentication method
        * @apiParam {String} password User's password
        * @apiParam {String} username User's login name
        * 
        * @apiHeader {String} Content-Type=application/x-www-form-urlencoded Expected content type of the params 
        * 
        * 
        * @apiHeaderExample Header-Example:
        *     {
        *       "Content-Type": "application/x-www-form-urlencoded"
        *     }
        * 
        * @apiPermission none
        * 
        * @apiSuccess {String} access_token Authentication token for secure web service requests
        * @apiSuccess {String} token_type Type of the authentication token
        * @apiSuccess {Number} expires_in Time to live for the token
        * @apiSuccess {String} userName Name of the authenticated user
        * @apiSuccess {Date} .issued Date the token was issued
        * @apiSuccess {Date} .expires Date the token expires 
        * 
        * @apiSuccessExample Success-Response:
        *      HTTP/1.1 200 OK
        *      {
        *           "access_token": "5_leQJfkDVIFxRgt-YZcnRGvzlrsFdNXClRaZEaTf3jzToaoFAUtOyneKNhtYo2cCSC9_fYXXBMS3UHwY3MZuzRbWmLqG_H1T5Cm6YHUzzQHPpKCCUqcYMx6m3NMLQoGVX9_f2svSSY4HieOTzwT-LTfxYbxHyuiRY0sJrmoGnQOtWGGC9GhdUXPCpM9_1L8p0vdAC1wnnTKtkDkxulAUkIxj3jJIUc9lUhqLoh5BRYJ8VsABsKEZYouLWvYfL5eqkL7EjwtxRNBhy9rJSU7nc_6lNCk5PEpsFg-laLYWeVO10gVFBkI3Ml7I8KgJA2Bs3vKKccIxuOwqoWjU_XIhV3f9Lhjm34OqSrwcThfx940k5TEWjHG9CHqxnItRvgHtqahc2rP8q0QuZ0qw6Ph5kxEE18YVGa8IVX7zAPW8V6WtXnVnR7SbZwnwiNTuh3i0t0Ib_gOKZAwUZTrVhsTqnFe20zMvJIsOzTtijl80bJMF7ZBAbKKL6zKfJcQ_DwYJaM0CISapXGjbJYMNWPaOzg6kTKtVuHb4akxfxlsAUk0QFqxZ793TT-KCdh-c0ppUNb3brPMh8wGpJDhwLbnhUfpBVnuxdC-sDrGUtzpSnMrPavKivovVT-sXBbqzeg6udOhjN3JXIv2ZzctisLYTw",
        *           "token_type": "bearer",
        *           "expires_in": 1209599,
        *           "userName": "admin@admin.com",
        *           ".issued": "Thu, 03 Nov 2016 14:38:29 GMT",
        *           ".expires": "Thu, 17 Nov 2016 14:38:29 GMT"
        *      }
        * 
        */
        public static void Configure(IAppBuilder app)
        {
            //Add autofac to the pipeline
            var httpConfig = new HttpConfiguration();

            ContainerConfig.Register(httpConfig);

            WebApiConfig.Register(httpConfig);


            app.UseAutofacMiddleware(ContainerConfig.Container);
  
            app.UseCors(CorsOptions.AllowAll);
            

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";


            //TODO: Add ApiDoc annotations
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true,
               
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            app.UseWebApi(httpConfig);
        }
    }
}