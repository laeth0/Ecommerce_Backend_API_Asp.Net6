using Ecommerce.Core.IGenericRepository;
using Ecommerce.Core.Models;
using Ecommerce.PL.CustomizeResponses;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace Ecommerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly IUnitOfWork unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCartItems()
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string? authHeader = Request.Headers["Authorization"];
            //or i can access like this => string? authHeader = Request.Headers.Authorization;


            if (authHeader is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("You Should Send The Token With The Request");
                return BadRequest(errorResponse);
            }

            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken tokenS;

            try
            {
                tokenS = handler.ReadJwtToken(authHeader);
            }
            catch (Exception e)
            {

                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Invalid Token");
                errorResponse.Errors.Add(e.Message);
                return BadRequest(errorResponse);
            }

            string? userId = tokenS.Claims.First(claim => claim.Type == "Id").Value;

            var CartItems = await unitOfWork.cartRepository.GetCartItemsByUserIdAsync(userId);

            return Ok(new SuccessResponse( "Request Success", CartItems));

        }


        [HttpPost("[action]/{ProductId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddToCart([FromRoute] int ProductId)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string? authHeader = Request.Headers["Authorization"];
            //or i can access like this => string? authHeader = Request.Headers.Authorization;


            if (authHeader is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("You Should Send The Token With The Request");
                return BadRequest(errorResponse);
            }

            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken tokenS;

            try
            {
                tokenS = handler.ReadJwtToken(authHeader);
            }
            catch (Exception e)
            {

                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Invalid Token");
                errorResponse.Errors.Add(e.Message);
                return BadRequest(errorResponse);
            }


            string? userId = tokenS.Claims.First(claim => claim.Type == "Id").Value;

            var CartItemExist = await unitOfWork.cartRepository.GetCartItemByUserIdAndProductIdAsync(userId, ProductId);

            if(CartItemExist is not null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("The Item Already Exist In The Cart");
                return BadRequest(errorResponse);
            }

            Cart CartItem = new Cart() { ProductId = ProductId, Quantity = 1, UserId = userId };

            await unitOfWork.cartRepository.AddAsync(CartItem);
            await unitOfWork.SaveChangesAsync();

            return Ok(new SuccessResponse("The Item Added Successfully",CartItem));
        }


        [HttpPatch("[action]/{ProductId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> IncreseItemQuantity([FromRoute] int ProductId)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string? authHeader = Request.Headers["Authorization"];
            //or i can access like this => string? authHeader = Request.Headers.Authorization;


            if (authHeader is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("You Should Send The Token With The Request");
                return BadRequest(errorResponse);
            }

            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken tokenS;

            try
            {
                tokenS = handler.ReadJwtToken(authHeader);
            }
            catch (Exception e)
            {

                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Invalid Token");
                errorResponse.Errors.Add(e.Message);
                return BadRequest(errorResponse);
            }


            string? userId = tokenS.Claims.First(claim => claim.Type == "Id").Value;

            Cart? CartItem = await unitOfWork.cartRepository.GetCartItemByUserIdAndProductIdAsync(userId, ProductId);

            if (CartItem is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add("The Item not Exist in Cart");
                return NotFound(errorResponse);
            }

            ++CartItem.Quantity;

            await unitOfWork.SaveChangesAsync();

            return Ok(new SuccessResponse( "The Item Quantity is Incremented Successfully", CartItem));
        }


        [HttpPatch("[action]/{ProductId}")]
        public async Task<IActionResult> DecreaseQuantity([FromRoute] int ProductId)
        {

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string? authHeader = Request.Headers["Authorization"];
            //or i can access like this => string? authHeader = Request.Headers.Authorization;


            if (authHeader is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("You Should Send The Token With The Request");
                return BadRequest(errorResponse);
            }

            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken tokenS;

            try
            {
                tokenS = handler.ReadJwtToken(authHeader);
            }
            catch (Exception e)
            {

                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Invalid Token");
                errorResponse.Errors.Add(e.Message);
                return BadRequest(errorResponse);
            }


            string? userId = tokenS.Claims.First(claim => claim.Type == "Id").Value;

            Cart? CartItem = await unitOfWork.cartRepository.GetCartItemByUserIdAndProductIdAsync(userId, ProductId);

            if (CartItem is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add("The Item not Exist in Cart");
                return NotFound(errorResponse);
            }

            --CartItem.Quantity;

            await unitOfWork.SaveChangesAsync();

            return Ok(new SuccessResponse( "The Item Quantity is Decreased Successfully", CartItem));
        }


        [HttpDelete("[action]/{ProductId}")]
        public async Task<IActionResult> RemoveCartItem([FromRoute] int ProductId)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string? authHeader = Request.Headers["Authorization"];
            //or i can access like this => string? authHeader = Request.Headers.Authorization;


            if (authHeader is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("You Should Send The Token With The Request");
                return BadRequest(errorResponse);
            }

            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken tokenS;

            try
            {
                tokenS = handler.ReadJwtToken(authHeader);
            }
            catch (Exception e)
            {

                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Invalid Token");
                errorResponse.Errors.Add(e.Message);
                return BadRequest(errorResponse);
            }


            string? userId = tokenS.Claims.First(claim => claim.Type == "Id").Value;

            Cart? CartItem = await unitOfWork.cartRepository.GetCartItemByUserIdAndProductIdAsync(userId, ProductId);

            if (CartItem is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add("The Item not Exist in Cart");
                return NotFound(errorResponse);
            }

            await unitOfWork.cartRepository.Delete(CartItem.Id);
            await unitOfWork.SaveChangesAsync();

            return Ok(new SuccessResponse("The Item Deleted Successfully",CartItem));
        }


        [HttpDelete("[action]")]
        public async Task<IActionResult> ClearCart()
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string? authHeader = Request.Headers["Authorization"];
            //or i can access like this => string? authHeader = Request.Headers.Authorization;


            if (authHeader is null)
            {
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("You Should Send The Token With The Request");
                return BadRequest(errorResponse);
            }

            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken tokenS;

            try
            {
                tokenS = handler.ReadJwtToken(authHeader);
            }
            catch (Exception e)
            {

                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Invalid Token");
                errorResponse.Errors.Add(e.Message);
                return BadRequest(errorResponse);
            }


            string? userId = tokenS.Claims.First(claim => claim.Type == "Id").Value;

            await unitOfWork.cartRepository.ClearCart(userId);

            await unitOfWork.SaveChangesAsync();

            return Ok(new SuccessResponse( "The Cart is Empty"));
        }


    }
}
