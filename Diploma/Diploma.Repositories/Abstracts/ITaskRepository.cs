using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Models;

namespace Diploma.Repositories.Abstracts
{
    public interface ITaskRepository
    {
        Task CreateTask(UserTask userTask);

        Task UpdateTask(UserTask userTask);

        IEnumerable<UserTask> GetUserTasks(string userEmail);

        IEnumerable<UserTask> FindBy(Func<UserTask, bool> func);
    }
}
