using System.Runtime.CompilerServices;
using System.Security.Claims;
using AutoMapper;
using Duende.IdentityModel;
using IS.Entities;
using IS.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IS.Pages.Account.Register
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        IMapper _mapper;
        [BindProperty]
        public UserRegistrationModel Input { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager,IMapper d)
        {
            _userManager = userManager;
            _mapper = d;
        }

        public IActionResult OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _mapper.Map<ApplicationUser>(Input);
           // var user = new ApplicationUser();
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return Page();
            }

            await _userManager.AddToRoleAsync(user, "Visitor");

            await _userManager.AddClaimsAsync(
                user,
                new List<Claim>
                {
                    new Claim(JwtClaimTypes.GivenName, user.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, user.LastName),
                    new Claim(JwtClaimTypes.Role, "Visitor"),
                    new Claim(JwtClaimTypes.Address, user.Address),
                    new Claim("country", user.Country),
                }
            );

            return Redirect(ReturnUrl);
        }
    }
}
