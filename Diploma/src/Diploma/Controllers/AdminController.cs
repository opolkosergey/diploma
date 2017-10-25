using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Pagging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Diploma.Core.Services;

namespace Diploma.Controllers
{
    public class AdminController : Controller
    {
        private readonly OrganizationService _organizationsService;

        public AdminController(OrganizationService organizationsService)
        {
            _organizationsService = organizationsService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> OrganizationsList()
        {
            var organizations = await _organizationsService.GetAll();

            return View(PaginatedList<Organization>.CreateAsync(organizations, 1, 10));
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult CreateOrganization()
        {
            return View("CreateOrganization");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateOrganization(Organization organization)
        {
            await _organizationsService.CreateOrganization(organization);

            return RedirectToAction("OrganizationsList");
        }
    }
}