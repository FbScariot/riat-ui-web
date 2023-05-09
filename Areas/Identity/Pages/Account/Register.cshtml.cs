using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using RIAT.DAL.Entity.Data;
using RIAT.DAL.Entity.Models;
using RIAT.UI.Web.Services;

namespace RIAT.UI.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RIATContext _context;
        private readonly IMyEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            [FromServices] RIATContext context,
            IMyEmailSender emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }

            [Required]
            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long Your password needs to contain at least one symbol (!, @, #, etc).", MinimumLength = 6)]
            [StringLength(100, ErrorMessage = "A Senha deve ter pelo menos {2} e no máximo {1} caracteres e ela devem conter um caractere especial (!, @, #), um número, um caractere minúsculo e um caractere maiúsculo. Exemplo: 01Teste@.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, FirstName = Input.FirstName, LastName = Input.LastName };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var resultRole = _userManager.AddToRoleAsync(user, Input.Role).Result;

                    Pessoa pessoa = new Pessoa
                    {
                        AspuIdAspnetuser = user.Id
                    };

                    _context.Pessoas.Add(pessoa);

                    if (Input.Role == "Pacientes")
                    {
                        pessoa.Paciente = new Paciente
                        {
                            IdPessoa = pessoa.Id,
                            PaciStAtivo = "A",
                            PaciDtAtivacao = DateTime.Now
                        };
                    }
                    else if (Input.Role == "Profissionais")
                    {
                        pessoa.Profissional = new Profissional
                        {
                            IdPessoa = pessoa.Id,
                            ProfStAtivo = "A",
                            ProfDtAtivacao = DateTime.Now,
                            TiprIdTipoProfissional = 1
                        };
                    }
                    else if (Input.Role == "Estagiarios")
                    {
                        pessoa.Profissional = new Profissional
                        {
                            IdPessoa = pessoa.Id,
                            ProfStAtivo = "A",
                            ProfDtAtivacao = DateTime.Now,
                            TiprIdTipoProfissional = 4
                        };
                    }
                    else if (Input.Role == "Profissionais Estagiarios")
                    {
                        pessoa.Profissional = new Profissional
                        {
                            IdPessoa = pessoa.Id,
                            ProfStAtivo = "A",
                            ProfDtAtivacao = DateTime.Now,
                            TiprIdTipoProfissional = 2
                        };
                    }

                    _context.SaveChanges();

                    _logger.LogInformation("User created a new account with password.");

                    string code = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;

                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        //return Page();
                        return LocalRedirect(returnUrl);
                    }

                    
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
