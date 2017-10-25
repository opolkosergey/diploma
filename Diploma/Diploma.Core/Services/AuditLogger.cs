using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;

namespace Diploma.Core.Services
{
    public class AuditLogger
    {
        private readonly ApplicationDbContext _context;

        public AuditLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Log(AuditEntry auditEntry)
        {
            _context.AuditEntries.Add(auditEntry);

            await _context.SaveChangesAsync();
        }
    }
}
