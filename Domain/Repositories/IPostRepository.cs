using Domain.Entities;

namespace Domain.Repositories;

public interface IPostRepository
{
    Post Create(Post post);
    Post? GetById(Guid postId);
    List<Post> GetAll(int limit, int offset, Guid? userId = null, bool? isPublished = null);
    Post? Update(Guid postId, Post post);
    bool Delete(Guid postId);
    bool ChangeStatus(Guid postId, bool isPublished);
}
