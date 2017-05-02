using System.Threading.Tasks;
using Diploma.Core.Models;

namespace Diploma.Services.Abstracts
{
    public interface IAuditLogger
    {
        Task Log(AuditEntry auditEntry);
    }
}
