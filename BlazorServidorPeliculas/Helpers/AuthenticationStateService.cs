﻿using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServidorPeliculas.Helpers
{
    public class AuthenticationStateService
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public AuthenticationStateService(AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> GetCurrentUserId()
        {
            var userState = await authenticationStateProvider.GetAuthenticationStateAsync();

            if (!userState.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var claims = userState.User.Claims;

            var claimWithUserId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (claimWithUserId == null)
            {
                throw new ApplicationException("Could not find User's ID");
            }

            return claimWithUserId.Value;
        }
    }
}
