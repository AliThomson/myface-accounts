using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Models.Database;
using MyFace.Repositories;
using Microsoft.Extensions.Primitives;
using System;
using Microsoft.AspNetCore.Http;
using MyFace.Services;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _users;
        private readonly IAuthService _authService;

        public UsersController(IUsersRepo users, IAuthService auth)
        {
            _users = users;
            _authService = auth;
        }
        
        [HttpGet("")]
        public ActionResult<UserListResponse> Search([FromQuery] UserSearchRequest searchRequest)
        {
            var authHeader = Request.Headers["Authorization"];
            
            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];

            var authHeaderSplit = authHeaderString.Split(' ');
            var authType = authHeaderSplit[0];
            var encodedUsernamePassword = authHeaderSplit[1];

            var usernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword)
            );

            var usernamePasswordArray = usernamePassword.Split(':');

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];
            
            if (!_authService.IsValidUsernameAndPassword(username, password))
            {
                return Unauthorized("The username and password are not valid");
            }
            var users = _users.Search(searchRequest);
            var userCount = _users.Count(searchRequest);
            return UserListResponse.Create(searchRequest, users, userCount);
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var authHeader = Request.Headers["Authorization"];
            
            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];

            var authHeaderSplit = authHeaderString.Split(' ');
            var authType = authHeaderSplit[0];
            var encodedUsernamePassword = authHeaderSplit[1];

            var usernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword)
            );

            var usernamePasswordArray = usernamePassword.Split(':');

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];
            
            if (!_authService.IsValidUsernameAndPassword(username, password))
            {
                return Unauthorized("The username and password are not valid");
            }
            var user = _users.GetById(id);
            return new UserResponse(user);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = _users.Create(newUser);

            var url = Url.Action("GetById", new { id = user.Id });
            var responseViewModel = new UserResponse(user);
            return Created(url, responseViewModel);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authHeader = Request.Headers["Authorization"];
            //Console.WriteLine("on line 64 authheader = " + authHeader[0]);

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];

            var authHeaderSplit = authHeaderString.Split(' ');
            var authType = authHeaderSplit[0];
            var encodedUsernamePassword = authHeaderSplit[1];

            var usernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword)
            );

            var usernamePasswordArray = usernamePassword.Split(':');

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];
            
            if (!_authService.IsValidUsernameAndPassword(username, password))
            {
                return Unauthorized("The username and password are not valid");
            }

            User loggedInUser = _users.GetByUsername(username);
            var user = _users.Update(id, update);

            if (user.Id != loggedInUser.Id)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to update a different user"
                );
            }
            
            return new UserResponse(user);
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var authHeader = Request.Headers["Authorization"];
            //Console.WriteLine("on line 64 authheader = " + authHeader[0]);

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];

            var authHeaderSplit = authHeaderString.Split(' ');
            var authType = authHeaderSplit[0];
            var encodedUsernamePassword = authHeaderSplit[1];

            var usernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword)
            );

            var usernamePasswordArray = usernamePassword.Split(':');

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];
            
            if (!_authService.IsValidUsernameAndPassword(username, password))
            {
                return Unauthorized("The username and password are not valid");
            }
            User loggedInUser = _users.GetByUsername(username);
            //var user = _users.GetById(id);

            if (id != loggedInUser.Id)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to delete a different user"
                );
            }
            else if (loggedInUser.Role == 0) 
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You do not have the permissions to perform this action"
                );
            }
            _users.Delete(id);
            return Ok();
        }


        public ActionResult<UserResponse> UpdateRole([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {

             if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authHeader = Request.Headers["Authorization"];
            //Console.WriteLine("on line 64 authheader = " + authHeader[0]);

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];

            var authHeaderSplit = authHeaderString.Split(' ');
            var authType = authHeaderSplit[0];
            var encodedUsernamePassword = authHeaderSplit[1];

            var usernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword)
            );

            var usernamePasswordArray = usernamePassword.Split(':');

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];
            
            if (!_authService.IsValidUsernameAndPassword(username, password))
            {
                return Unauthorized("The username and password are not valid");
            }

            User loggedInUser = _users.GetByUsername(username);
            var user = _users.UpdateRole(id, update);


            if (loggedInUser.Role == Role.Member) {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You do not have the permissions to perform this action"
                );
            }
           
        return new UserResponse(user);
        }
    }
}