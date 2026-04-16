using Twitter.Domain.Database.SqlServer.Entities;

namespace Twitter.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz del repositorio de posts.
/// </summary>
public interface IPostRepository
{
    Post Create(Post post);
    Post? GetById(Guid postId);
    List<Post> GetAll(int limit, int offset, Guid? userId = null, bool? isPublished = null);
    Post? Update(Guid postId, Post post);
    bool Delete(Guid postId);
    bool ChangeStatus(Guid postId, bool isPublished);
}