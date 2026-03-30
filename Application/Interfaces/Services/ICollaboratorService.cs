using Application.Models.DTOs;
using Application.Models.Requests.Collaborator;
using Application.Models.Responses;

namespace Application.Interfaces.Services
{
    public interface ICollaboratorService
    {
        public GenericResponse<CollaboratorDto> Create(CreateCollaboratorRequest model);
        public GenericResponse<CollaboratorDto> Update(Guid collaboratorId, UpdateCollaboratorRequest model);
        public GenericResponse<List<CollaboratorDto>> Get(int limit, int offset);
        public GenericResponse<CollaboratorDto?> Get(Guid collaboratorId);
        public GenericResponse<bool> Delete(Guid collaboratorId);
    }
}
