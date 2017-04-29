using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Pagging;
using Diploma.Services;
using Diploma.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrganizationsService _organizationsService = new OrganizationService();

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