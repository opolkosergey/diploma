using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Repositories;

namespace Diploma.Services
{
    public class SignatureRequestService
    {
        private readonly SignatureRequestRepository _signatureRequestRepository = new SignatureRequestRepository();

        public async Task CloneSignatureRequests(ApplicationUser user, ApplicationUser warrantUser)
        {
            var requestsForUser = _signatureRequestRepository.GetAll().Where(x => x.ApplicationUserId == user.Id);            

            foreach (var request in requestsForUser)
            {
                await _signatureRequestRepository.Create(new IncomingSignatureRequest
                {
                    ApplicationUserId = warrantUser.Id,
                    DocumentId = request.DocumentId,
                    UserRequester = request.UserRequester,
                    ClonnedUsingWarrant = true
                });
            }
        }

        public async Task CreateSignatureRequest(IncomingSignatureRequest signatureRequest)
        {
            await _signatureRequestRepository.Create(signatureRequest);
        }

        public async Task<IEnumerable<IncomingSignatureRequest>> GetSignatureRequestsForUser(ApplicationUser user)
        {
            return null;
        }
    }
}
