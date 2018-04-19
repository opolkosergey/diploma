using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Core.Repositories.Abstracts.Base;

namespace Diploma.Core.Services
{
    public class SignatureRequestService
    {
        private readonly BaseRepository<IncomingSignatureRequest> _signatureRequestRepository;

        public SignatureRequestService(BaseRepository<IncomingSignatureRequest> signatureRequestRepository)
        {
            _signatureRequestRepository = signatureRequestRepository;
        }

        public async Task CloneSignatureRequests(ApplicationUser user, ApplicationUser warrantUser)
        {
            var requestsForUser = _signatureRequestRepository.FindBy(x => x.ApplicationUserId == user.Id);            

            foreach (var request in requestsForUser)
            {
                await _signatureRequestRepository.Add(new IncomingSignatureRequest
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
            await _signatureRequestRepository.Add(signatureRequest);
        }

        public IEnumerable<IncomingSignatureRequest> GetSignatureRequestsForUser(ApplicationUser user)
        {
            return _signatureRequestRepository.FindBy(x => x.ApplicationUserId == user.Id);
        }
    }
}
