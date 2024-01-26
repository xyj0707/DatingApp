
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;

        public LikesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost("{username}")]

        public async Task<ActionResult> AddLike(string username)
        {
            // Get the ID of the authenticated user making the request
            var sourceUserId = User.GetUserId();
            // Retrieve the user to be liked based on the provided username
            var likedUser = await _uow.UserRepository.GetUserByUsernameAsync(username);
            // Retrieve the authenticated user with their likes
            var sourceUser = await _uow.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();  // Check if the liked user exists
            // Check if the authenticated user is trying to like themselves
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");
            // Check if the authenticated user already likes the target user
            var userLike = await _uow.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("You already like this user");

            // If not already liked, create a new UserLike instance
            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            // Add the UserLike to the liked users collection of the authenticated user
            sourceUser.LikedUsers.Add(userLike);
            if (await _uow.Complete()) return Ok();
            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _uow.LikesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }

    }

}