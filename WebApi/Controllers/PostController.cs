using Application.Interfaces.Services;
using Application.Models.Requests.Post;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController(IPostService postService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var post = await postService.Create(model);
        return CreatedAtAction(nameof(GetPostById), new { id = post.PostId }, post);
    }

    [HttpGet("list")]
    public IActionResult GetAllPosts([FromQuery] GetAllPostRequest model)
    {
        var rsp = postService.Get(model.Limit ?? 0, model.Offset ?? 0, model.UserId, model.IsPublished);
        return Ok(rsp);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetPostById(Guid id)
    {
        var post = postService.Get(id);
        return Ok(post);
    }

    [HttpPut("{id:guid}/update")]
    public async Task<IActionResult> UpdatePost([FromBody] UpdatePostRequest model, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var post = await postService.Update(id, model);
        return Ok(post);
    }

    [HttpPatch("{id:guid}/change-status")]
    public async Task<IActionResult> ChangePostStatus(Guid id, [FromBody] ChangePostStatusRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await postService.ChangeStatus(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}/delete")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        await postService.Delete(id);
        return NoContent();
    }
}