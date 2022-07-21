using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDo_List.ViewModels;
using ToDo_List.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ToDo_List.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AccountController(UserManager<IdentityUser> uManager,
            SignInManager<IdentityUser> sManager,
            RoleManager<IdentityRole> roleManager)
        {
            userManager = uManager;
            signInManager = sManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel vm = new RegisterViewModel();
            return View(vm);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                ModelState.AddModelError(string.Empty, "Invalid Username or Password");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName,
                };
                IdentityResult result = await
                roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",
                                  error.Description);
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult ListRole()
        {
            var roles = roleManager.Roles.ToList();
            var vm = new List<ListRoleViewModel>();
            roles.ForEach(item => vm.Add(
                new ListRoleViewModel()
                {
                    Id = item.Id,
                    RoleName = item.Name
                }));
            return View(vm);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> AssignRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id: { roleId} not found";
                return View("NotFound");
            }
            var model = new List<RoleUserViewModel>();
            foreach (var user in userManager.Users)
            {
                var roleUserViewModel = new RoleUserViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName
                };
                if (await userManager.IsInRoleAsync(user,
                                                role.Name))
                {
                    roleUserViewModel.IsSelected = true;
                }
                else
                {
                    roleUserViewModel.IsSelected = false;
                }
                model.Add(roleUserViewModel);
            }
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AssignRole(List<RoleUserViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id: {roleId} not found";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count(); i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
            }
            return RedirectToAction("index", "home");
        }

        [Authorize (Roles ="admin")]
        [HttpGet]
        public async Task<IActionResult> EditPermission(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id: {roleId} not found";
                return View("NotFound");
            }
            var existingUserClaims = await roleManager.GetClaimsAsync(role);
            var model = new RolePermissionViewModel
            {
                RoleId=roleId
            };
            foreach (Claim claim in PermissionStore.AllPermissions)
            {
                RoleClaim roleClaim = new RoleClaim
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaims.Any(c => c.Type == claim.Type))
                {
                    roleClaim.IsSelected = true;
                }

                model.Claims.Add(roleClaim);
            }
            return View(model);
        }
        [Authorize(Roles = "admin")]

        [HttpPost]
        public async Task<IActionResult> EditPermission(RolePermissionViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id: {model.RoleId} not found";
                return View("NotFound");
            }
            var claims = await roleManager.GetClaimsAsync(role);
            bool flag = false;
            foreach (var c in claims)
            {
                var result = await roleManager.RemoveClaimAsync(role, c);
                if (!result.Succeeded)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                ModelState.AddModelError("", "Cannot remove role claims");
                return View(model);
            }
            flag = false;
            var cl = model.Claims.Where(c => c.IsSelected).Select
                (c => new Claim(c.ClaimType, c.ClaimType));
            foreach (var c in cl) 
            {
                var result = await roleManager.AddClaimAsync(role,c);
                if (!result.Succeeded)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("ListRole");
        }



        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

    }
}
