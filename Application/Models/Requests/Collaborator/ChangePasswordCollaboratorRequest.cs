using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace Application.Models.Requests.Collaborator
{
    public class ChangePasswordCollaboratorRequest
    {
        [Required(ErrorMessage = ValidationConstants.REQUIRED)]
        public string CurrentPassword { get; set; } = null!;
        [Required(ErrorMessage = ValidationConstants.REQUIRED)]
        public string NewPassword { get; set; } = null!;
    }
}
