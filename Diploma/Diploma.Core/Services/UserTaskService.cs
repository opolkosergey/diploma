using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Core.Repositories;
using Diploma.Core.Repositories.Abstracts.Base;

namespace Diploma.Core.Services
{
    public class UserTaskService
    {
        private readonly BaseRepository<UserTask> _taskRepository;
        private readonly UserService _userService;

        public UserTaskService( UserService userService, BaseRepository<UserTask> taskRepository)
        {
            _userService = userService;
            _taskRepository = taskRepository;
        }

        public async Task CreateTask(UserTask userTask)
        {
            await _taskRepository.Add(userTask);
        }

        public async Task UpdateTask(UserTask userTask)
        {
            await _taskRepository.Update(userTask);
        }

        public IEnumerable<UserTask> GetUserTasks(string userEmail)
        {
            return _taskRepository.FindBy(userTask => userTask.AssignedTo == userEmail);
        }

        public IEnumerable<UserTask> GetTasksForOrganization(int organizationId)
        {
            var organizationsUsers = _userService.GetAll()
                .Where(u => u.OrganizationId.HasValue && u.OrganizationId == organizationId)
                .Select(x => x.Email)
                .ToList();

            var tasks = _taskRepository.FindBy(x => organizationsUsers.Contains(x.Creator));

            return tasks;
        }
    }
}
