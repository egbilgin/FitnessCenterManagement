// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FitnessCenterManagement.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Beni Hatırla")]
            public bool RememberMe { get; set; }
        }

        // 🔹 LOGIN SAYFASI GET
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // External cookie temizle
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = new List<AuthenticationScheme>();

            // 🔥 returnUrl saklanır
            ReturnUrl = returnUrl;
        }

        // 🔹 LOGIN POST
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
                return Page();
            }

            // 🔴 1️⃣ returnUrl VARSA → HER ŞEYDEN ÖNCE ORAYA
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            // 🔹 Kullanıcıyı al
            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // 🔴 2️⃣ ADMIN → Admin Dashboard
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction(
                    actionName: "Index",
                    controllerName: "Dashboard",
                    routeValues: new { area = "Admin" }
                );
            }

            // 🔴 3️⃣ MEMBER → Ana Sayfa (veya Member Dashboard)
            if (await _userManager.IsInRoleAsync(user, "Member"))
            {
                return RedirectToAction(
                    actionName: "Index",
                    controllerName: "Home",
                    routeValues: new { area = "" }
                );
            }

            // 🔹 4️⃣ Fallback
            return RedirectToAction("Index", "Home");
        }
    }
}
