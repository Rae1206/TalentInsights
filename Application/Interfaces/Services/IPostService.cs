using Application.Models.DTOs;
using Application.Models.Requests.Post;
using Application.Models.Responses;

namespace Application.Interfaces.Services;

public interface IPostService
{
    GenericResponse<PostDto> Create(CreatePostRequest model);
    GenericResponse<PostDto> Update(Guid postId, UpdatePostRequest model);
    GenericResponse<List<PostDto>> Get(int limit, int offset, Guid? userId, bool? isPublished);
    GenericResponse<PostDto?> Get(Guid postId);
    GenericResponse<bool> ChangeStatus(Guid postId, ChangePostStatusRequest model);
    GenericResponse<bool> Delete(Guid postId);
}
