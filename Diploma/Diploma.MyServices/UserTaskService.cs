using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Repositories;
using Diploma.Repositories.Abstracts;
using Diploma.Services.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Diploma.Services
{
    public class UserTaskService
    {
        private readonly ITaskRepository _taskRepository = new TaskRepository();
        private readonly IUserService _userService;

        public UserTaskService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userService = new UserService(userManager, roleManager);
        }

        public async Task CreateTask(UserTask userTask)
        {
            await _taskRepository.CreateTask(userTask);
        }

        public async Task UpdateTask(UserTask userTask)
        {
            await _taskRepository.UpdateTask(userTask);
        }

        public IEnumerable<UserTask> GetUserTasks(string userEmail)
        {
            return _taskRepository.GetUserTasks(userEmail);
        }

        public IEnumerable<UserTask> GetTasksForOrganization(int organizationId)
        {
            var organizationsUsers = _userService.GetAll()
                .Where(u => u.OrganizationId.HasValue && u.OrganizationId == organizationId)
                .Select(x => x.Email)
                .ToList();

            var tasks = _taskRepository.FindBy(x => organizationsUsers.Contains(x.AssignedTo));

            return tasks;
        }
    }
}
