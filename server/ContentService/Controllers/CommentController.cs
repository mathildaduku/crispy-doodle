using AutoMapper;
using ContentService.Dto.Request;
using ContentService.Extensions;
using ContentService.Helpers;
using ContentService.Models;
using ContentService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly ILikeService _likeService;
        private readonly ILogger<CommentController> _logger;
        private readonly ApiResponse<object> _response = new ApiResponse<object>();

        public CommentController(IMapper mapper, IPostService postService, ICommentService commentService, IUserService userService, ILikeService likeService, ILogger<CommentController> logger)
        {
            this._mapper = mapper;
            this._postService = postService;
            this._commentService = commentService;
            this._userService = userService;
            this._likeService = likeService;
            this._logger = logger;
        }

        [HttpPost("{postId:guid}")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto, [FromRoute] Guid postId)
        {
            _logger.LogInformation("Creating a comment {@Comment}", createCommentDto);
            Guid userId = User.GetUserId();

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                _logger.LogError($"No User Profile found: {userId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            _logger.LogInformation($"Searching for post with Id: {postId} for user with id: {userId}");

            var post = await _postService.GetById(postId);

            if (post == null)
            {
                _logger.LogError($"Unable to find post with Id: {postId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Post does not exist";
                return NotFound(_response);
            }

            var comment = _mapper.Map<Comment>(createCommentDto);
            comment.UserId = userId;
            comment.PostId = postId;

            _commentService.Add(comment);

            post.CommentCount++;

            _logger.LogInformation($"Increasing comment count for post with Id: {postId} to {post.CommentCount}");

            _postService.Update(post);

            var result = await _commentService.SaveChangesAsync();

            if (!result)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Something went wrong";
                return new ObjectResult(_response) { StatusCode = 500 };
            }

            _logger.LogInformation($"Comment with Id: {comment.Id} created successfully");

            _response.Message = "Comment created successfully";
            _response.Result = new { comment };
            return Ok(_response);
        }

       

        [HttpPost("reply/{commentId:guid}")]
        [Authorize]
        public async Task<IActionResult> ReplyComment([FromBody] CreateCommentDto createCommentDto, [FromRoute] Guid commentId)
        {
            Guid userId = User.GetUserId();

            _logger.LogInformation("Replying a comment {@Comment}", createCommentDto);

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                _logger.LogError($"No User Profile found: {userId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            _logger.LogInformation($"Searching for comment with Id: {commentId} for user with id: {userId}");

            var parentComment = await _commentService.GetById(commentId);

            if (parentComment == null)
            {
                _logger.LogError($"Unable to find parent comment with Id: {commentId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Comment does not exist";
                return NotFound(_response);
            }

            _logger.LogInformation($"Comment successfully found");

            var post = await _postService.GetById(parentComment.PostId);

            _logger.LogInformation($"Searching for post with Id: {parentComment.PostId} for user with id: {userId}");

            if (post == null)
            {
                _logger.LogError($"Unable to find post with Id: {post.Id}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Post does not exist";
                return NotFound(_response);
            }

            _logger.LogInformation($"Post successfully found");


            var comment = _mapper.Map<Comment>(createCommentDto);
            comment.UserId = userId;
            comment.PostId = parentComment.PostId;
            comment.ParentCommentId = commentId;
            parentComment.ReplyCount++;

            _logger.LogInformation($"Increasing parent comment reply count for comment with Id: {comment.ParentCommentId} to {parentComment.ReplyCount}");


            _commentService.Add(comment);
            _commentService.Update(parentComment);
            post.CommentCount++;

            _logger.LogInformation($"Increasing post comment count for post with Id: {post.Id} to {post.CommentCount}");

            _postService.Update(post);

            var result = await _commentService.SaveChangesAsync();

            if (!result)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Something went wrong";
                return new ObjectResult(_response) { StatusCode = 500 };
            }

            _logger.LogInformation($"Comment reply with Id: {comment.Id} created successfully");

            _response.Message = "Comment reply created successfully";
            _response.Result = new { comment };
            return Ok(_response);
        }

        [HttpGet("{postId:guid}")]
        public async Task<ActionResult> GetCommentsForAPost(Guid postId, [FromQuery] SearchParams searchParams)
        {
            _logger.LogInformation("GetCommentsForAPost started with postId: {postId} and searchParams: {searchParams}", postId, searchParams);
            var post = await _postService.GetById(postId);

            if (post == null)
            {
                _logger.LogWarning("Post with id: {postId} does not exist", postId);
                _response.Status = ResponseStatus.Error;
                _response.Message = "Post does not exist";
                return NotFound(_response);
            }

            var comments = await _commentService.GetCommentsForAPost(searchParams, postId);
            _logger.LogInformation("Comments for post with id: {postId} fetched successfully", postId);

            _response.Message = "Comments fetched successfully";
            _response.Result = new { comments };
            return Ok(_response);
        }

        [HttpGet("replies/{commentId:guid}")]
        public async Task<ActionResult> GetRepliesToAComment(Guid commentId, [FromQuery] SearchParams searchParams)
        {
            _logger.LogInformation("GetRepliesToAComment started with commentId: {commentId} and searchParams: {searchParams}", commentId, searchParams);
            var comment = await _commentService.GetById(commentId);

            if (comment == null)
            {
                _logger.LogWarning("Comment with id: {commentId} does not exist", commentId);
                _response.Status = ResponseStatus.Error;
                _response.Message = "Comment does not exist";
                return NotFound(_response);
            }

            var replies = await _commentService.GetRepliesForComment(searchParams, commentId);

            _logger.LogInformation("Replies for comment with id: {commentId} fetched successfully", commentId);
            _response.Message = "Replies fetched successfully";
            _response.Result = new { replies };
            return Ok(_response);
        }

        [HttpPost("like/{commentId:guid}")]
        [Authorize]
        public async Task<IActionResult> LikeAPost(Guid commentId)
        {
            _logger.LogInformation("LikeAPost started with commentId: {commentId}", commentId);
            Guid userId = User.GetUserId();

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                _logger.LogWarning("Unable to find profile info for user with id: {userId}", userId);
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            var comment = await _commentService.GetById(commentId);

            if (comment == null)
            {
                _logger.LogWarning("Comment with id: {commentId} does not exist", commentId);
                _response.Status = ResponseStatus.Error;
                _response.Message = "Comment does not exist";
                return BadRequest(_response);
            }

            var like = new Like
            {
                CommentId = comment.Id,
                UserId = user.Id,
            };

            _likeService.Add(like);
            comment.LikeCount++;
            _commentService.Update(comment);

            var result = await _likeService.SaveChangesAsync();

            if (!result)
            {
                _logger.LogError("Unable to like comment with id: {commentId}", commentId);
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to like comment";
                return new ObjectResult(_response) { StatusCode = 500 };
            }

            _logger.LogInformation("Comment with id: {commentId} liked successfully", commentId);
            _response.Status = ResponseStatus.Success;
            _response.Message = "Comment liked successfully";
            return Ok(_response);
        }

        [HttpPost("unlike/{commentId:guid}")]
        [Authorize]
        public async Task<IActionResult> UnLikeAComment(Guid commentId)
        {
            _logger.LogInformation("UnLikeAComment started with commentId: {commentId}", commentId);
            Guid userId = User.GetUserId();

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                _logger.LogWarning("Unable to find profile info for user with id: {userId}", userId);
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            var comment = await _commentService.GetById(commentId);

            if (comment == null)
            {
                _logger.LogWarning("Comment with id: {commentId} does not exist", commentId);
                _response.Status = ResponseStatus.Error;
                _response.Message = "Comment does not exist";
                return BadRequest(_response);
            }

            var like = await _likeService.GetUserLastLikeForAComment(userId, commentId);

            if (like == null)
            {
                _logger.LogError($"User with id {userId} hasn't liked pocommentt with Id: {commentId}. Returning early with success response");
                _response.Status = ResponseStatus.Success;
                _response.Message = "Comment unliked successfully";
                return Ok(_response);
            }

            _likeService.Remove(like);
            comment.LikeCount--;

            _commentService.Update(comment);

            var result = await _likeService.SaveChangesAsync();

            if (!result)
            {
                _logger.LogError("Unable to unlike comment with id: {commentId}", commentId);
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to unlike comment";
                return new ObjectResult(_response) { StatusCode = 500 };
            }

            _logger.LogInformation("Comment with id: {commentId} unliked successfully", commentId);
            _response.Status = ResponseStatus.Success;
            _response.Message = "Comment unliked successfully";
            return Ok(_response);
        }
    }
}
