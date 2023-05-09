// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Identity.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RIAT.UI.Web.Services
{
    /// <summary>
    /// Provides the APIs for managing user in a persistence store.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public class UserManager<TUser> : Microsoft.AspNetCore.Identity.UserManager<TUser>, IDisposable where TUser : class
    {
        public UserManager(IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        //public UserManager(ILogger logger)
        //{
        //    Logger = logger;
        //}


        private readonly Dictionary<string, IUserTwoFactorTokenProvider<TUser>> _tokenProviders = new Dictionary<string, IUserTwoFactorTokenProvider<TUser>>();

        protected override CancellationToken CancellationToken => base.CancellationToken;

        public override ILogger Logger { get => base.Logger; set => base.Logger = value; }

        public override bool SupportsUserAuthenticationTokens => base.SupportsUserAuthenticationTokens;

        public override bool SupportsUserAuthenticatorKey => base.SupportsUserAuthenticatorKey;

        public override bool SupportsUserTwoFactorRecoveryCodes => base.SupportsUserTwoFactorRecoveryCodes;

        public override bool SupportsUserTwoFactor => base.SupportsUserTwoFactor;

        public override bool SupportsUserPassword => base.SupportsUserPassword;

        public override bool SupportsUserSecurityStamp => base.SupportsUserSecurityStamp;

        public override bool SupportsUserRole => base.SupportsUserRole;

        public override bool SupportsUserLogin => base.SupportsUserLogin;

        public override bool SupportsUserEmail => base.SupportsUserEmail;

        public override bool SupportsUserPhoneNumber => base.SupportsUserPhoneNumber;

        public override bool SupportsUserClaim => base.SupportsUserClaim;

        public override bool SupportsUserLockout => base.SupportsUserLockout;

        public override bool SupportsQueryableUsers => base.SupportsQueryableUsers;

        public override IQueryable<TUser> Users => base.Users;
             

        /// <summary>
        /// Validates that an email confirmation token matches the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to validate the token against.</param>
        /// <param name="token">The email confirmation token to validate.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public override async Task<IdentityResult> ConfirmEmailAsync(TUser user, string token)
        {
            ThrowIfDisposed();
            var store = GetEmailStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!await VerifyUserTokenAsync(user, Options.Tokens.EmailConfirmationTokenProvider, ConfirmEmailTokenPurpose, token))
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidToken());
            }

            await store.SetEmailConfirmedAsync(user, true, CancellationToken);

            return await UpdateUserAsync(user);
        }

        /// <summary>
        /// Called to update the user after validating and updating the normalized email/user name.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Whether the operation was successful.</returns>
        protected override async Task<IdentityResult> UpdateUserAsync(TUser user)
        {
            var result = await ValidateUserAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            await UpdateNormalizedUserNameAsync(user);
            await UpdateNormalizedEmailAsync(user);
            return await Store.UpdateAsync(user, CancellationToken);
        }

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="token"/> is valid for
        /// the given <paramref name="user"/> and <paramref name="purpose"/>.
        /// </summary>
        /// <param name="user">The user to validate the token against.</param>
        /// <param name="tokenProvider">The token provider used to generate the token.</param>
        /// <param name="purpose">The purpose the token should be generated for.</param>
        /// <param name="token">The token to validate</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the <paramref name="token"/>
        /// is valid, otherwise false.
        /// </returns>
        public override async Task<bool> VerifyUserTokenAsync(TUser user, string tokenProvider, string purpose, string token)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (tokenProvider == null)
            {
                throw new ArgumentNullException(nameof(tokenProvider));
            }

            if (!_tokenProviders.ContainsKey(tokenProvider))
            {
                //throw new NotSupportedException(Resources.FormatNoTokenProvider(nameof(TUser), tokenProvider));
            }
            // Make sure the token is valid
            var result = await _tokenProviders[tokenProvider].ValidateAsync(purpose, token, this, user);

            if (!result)
            {
                Logger.LogWarning(9, "VerifyUserTokenAsync() failed with purpose: {purpose} for user {userId}.", purpose, await GetUserIdAsync(user));
            }
            return result;
        }

        private IUserEmailStore<TUser> GetEmailStore(bool throwOnFail = true)
        {
            var cast = Store as IUserEmailStore<TUser>;

            if (throwOnFail && cast == null)
            {
                //throw new NotSupportedException(Resources.StoreNotIUserEmailStore);
            }
            return cast;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override string GetUserName(ClaimsPrincipal principal)
        {
            return base.GetUserName(principal);
        }

        public override string GetUserId(ClaimsPrincipal principal)
        {
            return base.GetUserId(principal);
        }

        public override Task<TUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return base.GetUserAsync(principal);
        }

        public override Task<string> GenerateConcurrencyStampAsync(TUser user)
        {
            return base.GenerateConcurrencyStampAsync(user);
        }

        public override Task<IdentityResult> CreateAsync(TUser user)
        {
            return base.CreateAsync(user);
        }

        public override Task<IdentityResult> UpdateAsync(TUser user)
        {
            return base.UpdateAsync(user);
        }

        public override Task<IdentityResult> DeleteAsync(TUser user)
        {
            return base.DeleteAsync(user);
        }

        public override Task<TUser> FindByIdAsync(string userId)
        {
            return base.FindByIdAsync(userId);
        }

        public override Task<TUser> FindByNameAsync(string userName)
        {
            return base.FindByNameAsync(userName);
        }

        public override Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            return base.CreateAsync(user, password);
        }

        public override string NormalizeName(string name)
        {
            return base.NormalizeName(name);
        }

        public override string NormalizeEmail(string email)
        {
            return base.NormalizeEmail(email);
        }

        public override Task UpdateNormalizedUserNameAsync(TUser user)
        {
            return base.UpdateNormalizedUserNameAsync(user);
        }

        public override Task<string> GetUserNameAsync(TUser user)
        {
            return base.GetUserNameAsync(user);
        }

        public override Task<IdentityResult> SetUserNameAsync(TUser user, string userName)
        {
            return base.SetUserNameAsync(user, userName);
        }

        public override Task<string> GetUserIdAsync(TUser user)
        {
            return base.GetUserIdAsync(user);
        }

        public override Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            return base.CheckPasswordAsync(user, password);
        }

        public override Task<bool> HasPasswordAsync(TUser user)
        {
            return base.HasPasswordAsync(user);
        }

        public override Task<IdentityResult> AddPasswordAsync(TUser user, string password)
        {
            return base.AddPasswordAsync(user, password);
        }

        public override Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
        {
            return base.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public override Task<IdentityResult> RemovePasswordAsync(TUser user)
        {
            return base.RemovePasswordAsync(user);
        }

        protected override Task<PasswordVerificationResult> VerifyPasswordAsync(IUserPasswordStore<TUser> store, TUser user, string password)
        {
            return base.VerifyPasswordAsync(store, user, password);
        }

        public override Task<string> GetSecurityStampAsync(TUser user)
        {
            return base.GetSecurityStampAsync(user);
        }

        public override Task<IdentityResult> UpdateSecurityStampAsync(TUser user)
        {
            return base.UpdateSecurityStampAsync(user);
        }

        public override Task<string> GeneratePasswordResetTokenAsync(TUser user)
        {
            return base.GeneratePasswordResetTokenAsync(user);
        }

        public override Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword)
        {
            return base.ResetPasswordAsync(user, token, newPassword);
        }

        public override Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            return base.FindByLoginAsync(loginProvider, providerKey);
        }

        public override Task<IdentityResult> RemoveLoginAsync(TUser user, string loginProvider, string providerKey)
        {
            return base.RemoveLoginAsync(user, loginProvider, providerKey);
        }

        public override Task<IdentityResult> AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return base.AddLoginAsync(user, login);
        }

        public override Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return base.GetLoginsAsync(user);
        }

        public override Task<IdentityResult> AddClaimAsync(TUser user, Claim claim)
        {
            return base.AddClaimAsync(user, claim);
        }

        public override Task<IdentityResult> AddClaimsAsync(TUser user, IEnumerable<Claim> claims)
        {
            return base.AddClaimsAsync(user, claims);
        }

        public override Task<IdentityResult> ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim)
        {
            return base.ReplaceClaimAsync(user, claim, newClaim);
        }

        public override Task<IdentityResult> RemoveClaimAsync(TUser user, Claim claim)
        {
            return base.RemoveClaimAsync(user, claim);
        }

        public override Task<IdentityResult> RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims)
        {
            return base.RemoveClaimsAsync(user, claims);
        }

        public override Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return base.GetClaimsAsync(user);
        }

        public override Task<IdentityResult> AddToRoleAsync(TUser user, string role)
        {
            return base.AddToRoleAsync(user, role);
        }

        public override Task<IdentityResult> AddToRolesAsync(TUser user, IEnumerable<string> roles)
        {
            return base.AddToRolesAsync(user, roles);
        }

        public override Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role)
        {
            return base.RemoveFromRoleAsync(user, role);
        }

        public override Task<IdentityResult> RemoveFromRolesAsync(TUser user, IEnumerable<string> roles)
        {
            return base.RemoveFromRolesAsync(user, roles);
        }

        public override Task<IList<string>> GetRolesAsync(TUser user)
        {
            return base.GetRolesAsync(user);
        }

        public override Task<bool> IsInRoleAsync(TUser user, string role)
        {
            return base.IsInRoleAsync(user, role);
        }

        public override Task<string> GetEmailAsync(TUser user)
        {
            return base.GetEmailAsync(user);
        }

        public override Task<IdentityResult> SetEmailAsync(TUser user, string email)
        {
            return base.SetEmailAsync(user, email);
        }

        public override Task<TUser> FindByEmailAsync(string email)
        {
            return base.FindByEmailAsync(email);
        }

        public override Task UpdateNormalizedEmailAsync(TUser user)
        {
            return base.UpdateNormalizedEmailAsync(user);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(TUser user)
        {
            return base.GenerateEmailConfirmationTokenAsync(user);
        }

        public override Task<bool> IsEmailConfirmedAsync(TUser user)
        {
            return base.IsEmailConfirmedAsync(user);
        }

        public override Task<string> GenerateChangeEmailTokenAsync(TUser user, string newEmail)
        {
            return base.GenerateChangeEmailTokenAsync(user, newEmail);
        }

        public override Task<IdentityResult> ChangeEmailAsync(TUser user, string newEmail, string token)
        {
            return base.ChangeEmailAsync(user, newEmail, token);
        }

        public override Task<string> GetPhoneNumberAsync(TUser user)
        {
            return base.GetPhoneNumberAsync(user);
        }

        public override Task<IdentityResult> SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            return base.SetPhoneNumberAsync(user, phoneNumber);
        }

        public override Task<IdentityResult> ChangePhoneNumberAsync(TUser user, string phoneNumber, string token)
        {
            return base.ChangePhoneNumberAsync(user, phoneNumber, token);
        }

        public override Task<bool> IsPhoneNumberConfirmedAsync(TUser user)
        {
            return base.IsPhoneNumberConfirmedAsync(user);
        }

        public override Task<string> GenerateChangePhoneNumberTokenAsync(TUser user, string phoneNumber)
        {
            return base.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        public override Task<bool> VerifyChangePhoneNumberTokenAsync(TUser user, string token, string phoneNumber)
        {
            return base.VerifyChangePhoneNumberTokenAsync(user, token, phoneNumber);
        }

        public override Task<string> GenerateUserTokenAsync(TUser user, string tokenProvider, string purpose)
        {
            return base.GenerateUserTokenAsync(user, tokenProvider, purpose);
        }

        public override void RegisterTokenProvider(string providerName, IUserTwoFactorTokenProvider<TUser> provider)
        {
            base.RegisterTokenProvider(providerName, provider);
        }

        public override Task<IList<string>> GetValidTwoFactorProvidersAsync(TUser user)
        {
            return base.GetValidTwoFactorProvidersAsync(user);
        }

        public override Task<bool> VerifyTwoFactorTokenAsync(TUser user, string tokenProvider, string token)
        {
            return base.VerifyTwoFactorTokenAsync(user, tokenProvider, token);
        }

        public override Task<string> GenerateTwoFactorTokenAsync(TUser user, string tokenProvider)
        {
            return base.GenerateTwoFactorTokenAsync(user, tokenProvider);
        }

        public override Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return base.GetTwoFactorEnabledAsync(user);
        }

        public override Task<IdentityResult> SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            return base.SetTwoFactorEnabledAsync(user, enabled);
        }

        public override Task<bool> IsLockedOutAsync(TUser user)
        {
            return base.IsLockedOutAsync(user);
        }

        public override Task<IdentityResult> SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            return base.SetLockoutEnabledAsync(user, enabled);
        }

        public override Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return base.GetLockoutEnabledAsync(user);
        }

        public override Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user)
        {
            return base.GetLockoutEndDateAsync(user);
        }

        public override Task<IdentityResult> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd)
        {
            return base.SetLockoutEndDateAsync(user, lockoutEnd);
        }

        public override Task<IdentityResult> AccessFailedAsync(TUser user)
        {
            return base.AccessFailedAsync(user);
        }

        public override Task<IdentityResult> ResetAccessFailedCountAsync(TUser user)
        {
            return base.ResetAccessFailedCountAsync(user);
        }

        public override Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return base.GetAccessFailedCountAsync(user);
        }

        public override Task<IList<TUser>> GetUsersForClaimAsync(Claim claim)
        {
            return base.GetUsersForClaimAsync(claim);
        }

        public override Task<IList<TUser>> GetUsersInRoleAsync(string roleName)
        {
            return base.GetUsersInRoleAsync(roleName);
        }

        public override Task<string> GetAuthenticationTokenAsync(TUser user, string loginProvider, string tokenName)
        {
            return base.GetAuthenticationTokenAsync(user, loginProvider, tokenName);
        }

        public override Task<IdentityResult> SetAuthenticationTokenAsync(TUser user, string loginProvider, string tokenName, string tokenValue)
        {
            return base.SetAuthenticationTokenAsync(user, loginProvider, tokenName, tokenValue);
        }

        public override Task<IdentityResult> RemoveAuthenticationTokenAsync(TUser user, string loginProvider, string tokenName)
        {
            return base.RemoveAuthenticationTokenAsync(user, loginProvider, tokenName);
        }

        public override Task<string> GetAuthenticatorKeyAsync(TUser user)
        {
            return base.GetAuthenticatorKeyAsync(user);
        }

        public override Task<IdentityResult> ResetAuthenticatorKeyAsync(TUser user)
        {
            return base.ResetAuthenticatorKeyAsync(user);
        }

        public override string GenerateNewAuthenticatorKey()
        {
            return base.GenerateNewAuthenticatorKey();
        }

        public override Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(TUser user, int number)
        {
            return base.GenerateNewTwoFactorRecoveryCodesAsync(user, number);
        }

        protected override string CreateTwoFactorRecoveryCode()
        {
            return base.CreateTwoFactorRecoveryCode();
        }

        public override Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(TUser user, string code)
        {
            return base.RedeemTwoFactorRecoveryCodeAsync(user, code);
        }

        public override Task<int> CountRecoveryCodesAsync(TUser user)
        {
            return base.CountRecoveryCodesAsync(user);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override Task<byte[]> CreateSecurityTokenAsync(TUser user)
        {
            return base.CreateSecurityTokenAsync(user);
        }

        protected override Task<IdentityResult> UpdatePasswordHash(TUser user, string newPassword, bool validatePassword)
        {
            return base.UpdatePasswordHash(user, newPassword, validatePassword);
        }
    }
}