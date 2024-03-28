using Ecommerce.Core.Models;
using Ecommerce.PL.CustomizeResponses;
using Ecommerce.PL.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }



        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO newUser)
        {

            User? userExist = await userManager.FindByEmailAsync(newUser.Email);
            if (userExist != null)
                return BadRequest(new Response(HttpStatusCode.BadRequest, "Email Address is Already Exist"));

            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    isAgree = newUser.isAgree,
                };


                IdentityResult result = await userManager.CreateAsync(user, newUser.Password);


                if (!result.Succeeded)
                {
                    ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);

                    foreach (IdentityError error in result.Errors)
                        errorResponse.Errors.Add(error.Description);

                    return BadRequest(errorResponse);
                }

                return Ok(new Response(HttpStatusCode.OK, "The User Created Succesfuly"));
            }

            else
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        errorResponse.Errors.Add(modelError.ErrorMessage);
                    }
                }

                return BadRequest(errorResponse);
            }

        }



        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
        {
            if (ModelState.IsValid)
            {
                User? user = await userManager.FindByEmailAsync(login.Email);
                if (user != null)
                {
                    bool result = await userManager.CheckPasswordAsync(user, login.Password);
                    if (result)
                    {
                        // create clams
                        List<Claim> authClaims = new List<Claim>
                            {
                                new Claim("Id", user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.Email, user.Email),
                            };


                        // create token
                        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                        // var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authnetication");
                        byte[]? key = Encoding.ASCII.GetBytes(configuration["JWT:SecretKey"]);
                        //configuration["JWT:SecretKey"]   =>Equal=>    configuration.GetSection("JWT")["SecretKey"];

                        SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(key);


                        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                        {
                            Subject = new ClaimsIdentity(authClaims),
                            Expires = DateTime.UtcNow.AddMonths(4),
                            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
                        };


                        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                        string tokenString = tokenHandler.WriteToken(token);


                        return Ok(new SuccessResponse("Login in Successfully", new { User = user, Token = tokenString }));

                    }
                    else
                    {
                        ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                        errorResponse.Errors.Add("Password invalid");
                        return BadRequest(errorResponse);
                    }

                }
                else
                {
                    ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                    errorResponse.Errors.Add("Invalid login Attempt. Email Address already exist");
                    return BadRequest(errorResponse);
                }
            }

            else
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);

                foreach (var modelState in ModelState.Values)
                    foreach (var modelError in modelState.Errors)
                        errorResponse.Errors.Add(modelError.ErrorMessage);

                return BadRequest(errorResponse);
            }


        }

    }
}
