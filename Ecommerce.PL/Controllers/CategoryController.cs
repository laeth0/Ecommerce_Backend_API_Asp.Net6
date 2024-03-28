using AutoMapper;
using Ecommerce.Core.IGenericRepository;
using Ecommerce.Core.ILog;
using Ecommerce.Core.Models;
using Ecommerce.PL.CustomizeResponses;
using Ecommerce.PL.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ecommerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogging logging;

        public CategoryController(IMapper mapper, IUnitOfWork unitOfWork, ILogging logging)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.logging = logging;
        }



        #region  Get

        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResponseCache(CacheProfileName = "500SecondsDuration")]
        public async Task<ActionResult<IReadOnlyList<Category>>> GetAllCategory()
        {
            logging.Log("Get All Category", "Information");// this statement will be logged in the console
            var Categories = await unitOfWork.categoryRepository.GetAllAsync();
            var CategoryDto = mapper.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryDTO>>(Categories);
            return Ok(new SuccessResponse( "Success", CategoryDto));
        }


        [HttpGet("[action]/{id:int}")] // we put bracket to make the variable dynamic => if we but [HttpGet("id")] then the id will be static (string id)
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResponseCache(CacheProfileName = "500SecondsDuration")]
        public async Task<ActionResult<Category>> GetCategoryById([FromRoute] int id)
        {
            if (id <= 0)
            {
                logging.Log("error becouse the id is less than zero", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Error Becouse the Id is less than Zero");
                return BadRequest(errorResponse);
            }

            Category? Category = await unitOfWork.categoryRepository.GetByIdAsync(id);

            if (Category is null)
            {
                logging.Log($"Category of {id} not exist", "Warning");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add($"The Category with id={id} is not exist");
                return NotFound(errorResponse);
            }

            CategoryDTO CategoryDto = mapper.Map<Category, CategoryDTO>(Category);

            return Ok(new SuccessResponse( "Success", CategoryDto));
        }


        #endregion



        #region Post
        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        //[Authorize(Roles = "Admin")] // i write this line to make the method only for the admin
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category Category)
        {
            if (Category is null)
            {
                logging.Log("The Category is null, Please send the Category to add it to database", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("The Category is null, Please send the Category to add it to database");
                return BadRequest(errorResponse);
            }

            if (Category.Id > 0) //return StatusCode(StatusCodes.Status500InternalServerError);
            {
                logging.Log("The Id is incorrect", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("The Id is incorrect");
                return BadRequest(errorResponse);
            }

            if (ModelState.IsValid)
            {
                await unitOfWork.categoryRepository.AddAsync(Category);
                await unitOfWork.SaveChangesAsync();

                var CategoryDto = mapper.Map<Category, CategoryDTO>(Category);

                return Ok(new SuccessResponse( "Category Created Successfully", CategoryDto));
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
        #endregion



        #region Put
        [HttpPut("[action]/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Category>> UpdateCategory([FromRoute] int id, [FromBody] CategoryDTO model)
        {

            if (id <= 0)
            {
                logging.Log("error becouse the id is less than zero", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Error Becouse the Id is less than Zero");
                return BadRequest(errorResponse);
            }
            if (model is null)
            {
                logging.Log("Please Send The Category Information", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Please Send The Category Information");
                return BadRequest(errorResponse);
            }


            Category? Category = await unitOfWork.categoryRepository.GetByIdAsync(id);

            if (Category is null)
            {
                logging.Log($"The Category with id={id} is not exist", "Warning");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add($"The Category with id={id} is not exist");
                return NotFound(errorResponse);
            }

            Category.Name = model.Name;
            Category.Description = model.Description;

            await unitOfWork.SaveChangesAsync();

            CategoryDTO CategoryDto = mapper.Map<Category, CategoryDTO>(Category);

            return Ok(new SuccessResponse( "Category Updated SuccessFully", CategoryDto));
        }
        #endregion



        #region Delete
        [HttpDelete("[action]/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Category>> DeleteCategory([FromRoute] int id)
        {
            if (id <= 0)
            {
                logging.Log("error becouse the id is less than zero", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Error Becouse the Id is less than Zero");
                return BadRequest(errorResponse);
            }

            Category? Category = await unitOfWork.categoryRepository.GetByIdAsync(id);

            if (Category is null)
            {
                logging.Log($"There is no category with id ={id}", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("The Category is null, Please send the Category to add it to database");
                return BadRequest(errorResponse);
            }

            await unitOfWork.categoryRepository.Delete(id);

            await unitOfWork.SaveChangesAsync();

            CategoryDTO CategoryDto = mapper.Map<Category, CategoryDTO>(Category);

            return Ok(new SuccessResponse("Success", CategoryDto));
        }
        #endregion





    }
}
