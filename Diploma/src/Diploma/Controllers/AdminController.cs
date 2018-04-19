using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Pagging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Diploma.Core.Services;
using Diploma.ViewModels.AdminViewModels;
using System.Linq;

namespace Diploma.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly UserService _userService;

        public AdminController(OrganizationService organizationService, UserService userService)
        {
            _organizationService = organizationService;
            _userService = userService;
        }

        [HttpGet]        
        public async Task<IActionResult> OrganizationsList()
        {
            var organizations = await _organizationService.GetAll();
            return View(PaginatedList<Organization>.Create(organizations, 1, 10));
        }

        [HttpGet]
        public IActionResult CreateOrganization() => View(nameof(CreateOrganization));

        [HttpPost]
        public async Task<IActionResult> CreateOrganization(Organization organization)
        {
            await _organizationService.CreateOrganization(organization);
            return RedirectToAction(nameof(OrganizationsList));
        }

        public async Task<ActionResult> UpdateUser(
            [FromServices] DocumentService documentService, 
            string email, 
            string role, 
            string organization)
        {
            var organizationEntity = _organizationService.GetOrganization(organization);
            await _userService.UpdateUserByAdmin(email, role, organizationEntity);

            var user = await _userService.GetUserByEmail(email);
            await documentService.UpdateUserDocumentAccesses(user, true);

            return RedirectToAction(nameof(Users), new { forEdit = true });
        }

        public async Task<IActionResult> Users(int page = 1, bool forEdit = false)
        {
            ViewBag.ForEdit = forEdit;
            var users = await _userService.GetAll();

            var usersModel = users.Select(user => new AdminUserModel
            {
                Email = user.Email,
                Organization = user?.Organization?.Name,
                Role = _userService.GetRoles().First(x => x.Id == user.Roles.First().RoleId).Name
            });

            ViewData["organizations"] = (await _organizationService.GetAll()).Select(x => x.Name);

            return View(PaginatedList<AdminUserModel>.Create(usersModel.ToList(), 1, 10));
        }
    }
}