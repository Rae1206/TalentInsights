using Application.Interfaces.Services;
using Application.Models.DTOs;
using Application.Models.Requests.Post;
using Application.Models.Responses;
using Twitter.Domain.Database.SqlServer;
using Twitter.Domain.Database.SqlServer.Entities;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using Shared.Exceptions;
using Shared.Helpers;

namespace Application.Services;

/// <summary>
/// Servicio para la gestión de posts.
/// </summary>
public class PostService(
    IUnitOfWork unitOfWork,
    ILogger<PostService> logger) : IPostService
{
    public async Task<PostDto> Create(CreatePostRequest model)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Intentando crear post para usuario: {UserId}", model.UserId);
        }

        var entity = new Post
        {
            PostId = Guid.NewGuid(),
            UserId = model.UserId,
            Content = model.Content,
            IsPublished = model.IsPublished ?? false,
            CreatedAt = DateTimeHelper.UtcNow()
        };

        var created = unitOfWork.postRepository.Create(entity);
        await unitOfWork.SaveChangesAsync();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Post creado exitosamente con ID: {PostId}", created.PostId);
        }
        return MapToDto(created);
    }

    public async Task<PostDto> Update(Guid postId, UpdatePostRequest model)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Intentando actualizar post con ID: {PostId}", postId);
        }

        var existing = unitOfWork.postRepository.GetById(postId);

        if (existing is null)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("Post no encontrado para actualizar: {PostId}", postId);
            }
            throw new ResourceNotFoundException("post", postId);
        }

        var updated = new Post
        {
            PostId = existing.PostId,
            UserId = model.UserId ?? existing.UserId,
            Content = model.Content ?? existing.Content,
            IsPublished = existing.IsPublished,
            CreatedAt = existing.CreatedAt
        };

        var result = unitOfWork.postRepository.Update(postId, updated);

        if (result is null)
        {
            logger.LogError("Error al actualizar post con ID: {PostId}", postId);
            throw new InvalidOperationException(ErrorConstants.INTERNAL_SERVER_ERROR);
        }

        await unitOfWork.SaveChangesAsync();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Post actualizado exitosamente con ID: {PostId}", result.PostId);
        }
        return MapToDto(result);
    }

    public GenericResponse<List<PostDto>> Get(int limit, int offset, Guid? userId = null, bool? isPublished = null)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Obteniendo lista de posts | Limit: {Limit}, Offset: {Offset}, UserId: {UserId}, Publicado: {IsPublished}",
                limit, offset, userId, isPublished);
        }

        var posts = unitOfWork.postRepository.GetAll(limit, offset, userId, isPublished);
        var dtos = posts.Select(MapToDto).ToList();
        return new GenericResponse<List<PostDto>> { Data = dtos };
    }

    public PostDto Get(Guid postId)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Buscando post con ID: {PostId}", postId);
        }

        var post = unitOfWork.postRepository.GetById(postId);

        if (post is null)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("Post no encontrado con ID: {PostId}", postId);
            }
            throw new ResourceNotFoundException("post", postId);
        }

        return MapToDto(post);
    }

    public async Task ChangeStatus(Guid postId, ChangePostStatusRequest model)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Intentando cambiar estado del post con ID: {PostId}", postId);
        }

        var result = unitOfWork.postRepository.ChangeStatus(postId, model.IsPublished);

        if (!result)
        {
            logger.LogError("Error al cambiar estado del post con ID: {PostId}", postId);
            throw new InvalidOperationException("No se pudo cambiar el estado del post");
        }

        await unitOfWork.SaveChangesAsync();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Estado del post cambiado exitosamente con ID: {PostId}", postId);
        }
    }

    public async Task Delete(Guid postId)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Intentando eliminar post con ID: {PostId}", postId);
        }

        var post = unitOfWork.postRepository.GetById(postId);

        if (post is null)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("Post no encontrado para eliminar: {PostId}", postId);
            }
            throw new ResourceNotFoundException("post", postId);
        }

        var result = unitOfWork.postRepository.Delete(postId);

        if (!result)
        {
            logger.LogError("Error al eliminar post con ID: {PostId}", postId);
            throw new InvalidOperationException("No se pudo eliminar el post");
        }

        await unitOfWork.SaveChangesAsync();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Post eliminado exitosamente con ID: {PostId}", postId);
        }
    }

    private static PostDto MapToDto(Post entity) => new()
    {
        PostId = entity.PostId,
        UserId = entity.UserId,
        Content = entity.Content,
        IsPublished = entity.IsPublished,
        CreatedAt = entity.CreatedAt
    };
}
