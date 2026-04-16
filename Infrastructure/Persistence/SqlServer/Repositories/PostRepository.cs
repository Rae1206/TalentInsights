using Twitter.Domain.Database.SqlServer.Context;
using Twitter.Domain.Database.SqlServer.Entities;
using Twitter.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de posts.
/// </summary>
public class PostRepository(TwitterDbContext context) : IPostRepository
{
    public Post Create(Post post)
    {
        context.Posts.Add(post);
        return post;
    }

    public Post? GetById(Guid postId) =>
        context.Posts.Include(p => p.User).FirstOrDefault(p => p.PostId == postId);

    public List<Post> GetAll(int limit, int offset, Guid? userId = null, bool? isPublished = null)
    {
        var query = context.Posts.Include(p => p.User).AsQueryable();

        if (userId.HasValue)
            query = query.Where(p => p.UserId == userId.Value);

        if (isPublished.HasValue)
            query = query.Where(p => p.IsPublished == isPublished.Value);

        var normalizedOffset = Math.Max(offset, 0);
        var normalizedLimit = limit <= 0 ? int.MaxValue : limit;

        return query
            .Skip(normalizedOffset)
            .Take(normalizedLimit)
            .ToList();
    }

    public Post? Update(Guid postId, Post post)
    {
        var entity = context.Posts.Find(postId);
        if (entity is null) return null;

        entity.Content = post.Content;
        entity.IsPublished = post.IsPublished;

        if (post.UserId != entity.UserId)
            entity.UserId = post.UserId;

        return entity;
    }

    public bool Delete(Guid postId)
    {
        var entity = context.Posts.Find(postId);
        if (entity is null) return false;

        context.Posts.Remove(entity);
        return true;
    }

    public bool ChangeStatus(Guid postId, bool isPublished)
    {
        var entity = context.Posts.Find(postId);
        if (entity is null) return false;

        entity.IsPublished = isPublished;
        return true;
    }
}
