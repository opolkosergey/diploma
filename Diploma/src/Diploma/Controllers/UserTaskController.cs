using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Controllers
{
    public class UserTaskController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;


        private readonly UserTaskService _userTaskService;
        private readonly UserService _userService;

        public UserTaskController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userTaskService = new UserTaskService(userManager, roleManager);
            _userService = new UserService(userManager, roleManager);
        }

        [HttpGet]
        public IActionResult CreateUserTask()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserTask(UserTask userTask)
        {
            userTask.Creator = User.Identity.Name;

            await _userTaskService.CreateTask(userTask);

            return RedirectToAction("TasksList");
        }

        [HttpGet]
        public IActionResult TasksList()
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var tasks = _userTaskService.GetTasksForOrganization(user.OrganizationId.Value);

            return View(tasks);
        }
    }
}