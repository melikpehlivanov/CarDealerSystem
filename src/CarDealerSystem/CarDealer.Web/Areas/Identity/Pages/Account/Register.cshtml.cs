﻿using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CarDealer.Web.Areas.Identity.Pages.Account
{
    using CarDealer.Models;
    using Controllers;
    using Infrastructure.Utilities.Interfaces;
    using Microsoft.AspNetCore.Identity;

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The passwords doesn't match!")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), new { area = "" });
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), new { area = "" });
            }

            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(WebConstants.AppMainEmailAddress, Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("/identity/account/registered");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
