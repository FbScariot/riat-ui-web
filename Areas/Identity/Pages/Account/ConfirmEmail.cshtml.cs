using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using RIAT.DAL.Entity.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Identity.Core;
using Microsoft.Extensions.Logging;
using RIAT.DAL.Entity.Data;
using RIAT.UI.Web.Data;
using Microsoft.AspNetCore.Mvc.Localization;

namespace RIAT.UI.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHtmlLocalizer<ConfirmEmailModel> _localizer;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager,
            [FromServices] ApplicationDbContext context, IHtmlLocalizer<ConfirmEmailModel> localizer)
        {
            _userManager = userManager;
            _context = context;
            _localizer = localizer;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string sender)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            if (sender == "api")
            {
                user.EmailConfirmed = true;
                _context.Update(user);

                int resultUpdate = await _context.SaveChangesAsync();
                //StatusMessage = resultUpdate == 1 ? "Thank you for confirming your email." : "Error confirming your email.";
                StatusMessage = resultUpdate == 1 ? _localizer["StatusMessageSucess"].Value : _localizer["StatusMessageError"].Value;
            }
            else
            {
                //RIAT.UI.Web.Resources.Areas.Identity.Pages.Account.ConfirmEmailModel 
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user, code);
                //StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
                StatusMessage = result.Succeeded ? _localizer["StatusMessageSucess"].Value : _localizer["StatusMessageError"].Value;
            }       

            return Page();
        }
    }
}
