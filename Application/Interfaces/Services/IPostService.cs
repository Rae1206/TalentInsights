using Application.Models.DTOs;
using Application.Models.Requests.Post;
using Application.Models.Responses;

namespace Application.Interfaces.Services;

public interface IPostService
{
    Task<PostDto> Create(CreatePostRequest model);
    Task<PostDto> Update(Guid postId, UpdatePostRequest model);
    GenericResponse<List<PostDto>> Get(int limit, int offset, Guid? userId, bool? isPublished);
    PostDto Get(Guid postId);
    Task ChangeStatus(Guid postId, ChangePostStatusRequest model);
    Task Delete(Guid postId);
}
