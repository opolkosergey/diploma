using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Services.Abstracts;

namespace Diploma.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public async Task Log(AuditEntry auditEntry)
        {
            _context.AuditEntries.Add(auditEntry);

            await _context.SaveChangesAsync();
        }
    }
}
