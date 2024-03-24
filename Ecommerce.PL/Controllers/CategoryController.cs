using AutoMapper;
using Ecommerce.Core.IGenericRepository;
using Ecommerce.Core.ILog;
using Ecommerce.Core.Models;
using Ecommerce.PL.CustomizeResponses;
using Ecommerce.PL.DTO;
using Ecommerce.Repository.Data;
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

        [HttpGet]
        [ProducesResponseType(200)]
        [ResponseCache(CacheProfileName = "500SecondsDuration")]
        public async Task<ActionResult<IReadOnlyList<Category>>> GetAll()
        {
            logging.Log("Get All Category", "Information");// this statement will be logged in the console
            var Categories = await unitOfWork.categoryRepository.GetAllAsync();
            var CategoryDto = mapper.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryDTO>>(Categories); 
            return Ok(new ApiResponse(HttpStatusCode.OK, CategoryDto, "Success"));
        }


        [HttpGet("{id}", Name = "GetCategoryById")] // we put bracket to make the variable dynamic => if we but [HttpGet("id")] then the id will be static (string id)
        [ProducesResponseType(200)] //mean that the method will return 200 status code and the process is success
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(400)] // bad request
        [ResponseCache(CacheProfileName = "500SecondsDuration")]
        public async Task<ActionResult<Category>> GetById([FromRoute] int id)
        {
            if (id <= 0)
            {
                logging.Log("error becouse the id is less than zero", "Error");
                return BadRequest(new Response(400, "error becouse the id is less than zero"));
            }
            var Category = await unitOfWork.categoryRepository.GetByIdAsync(id);

            if (Category is not null)
            {
                logging.Log($"Category of {id} not exist", "Warning");
                return NotFound(new Response(404,$"The Category with id={id} is not exist"));
            }

            var CategoryDto = mapper.Map<Category, CategoryDTO>(Category);

            return Ok(new ApiResponse(HttpStatusCode.OK, CategoryDto, "Success"));
        }


        #endregion



        #region Post

        [HttpPost(Name = "Create Category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Category>> Create([FromBody] Category Category)
        {
            if (Category is null)
            {
                logging.Log("The Category is null", "Error");
                return BadRequest(new Response(400));
            }

            if (Category.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);

            await unitOfWork.categoryRepository.AddAsync(Category);
            await unitOfWork.SaveChangesAsync();

            var CategoryDto = mapper.Map<Category, CategoryDTO>(Category);

            return Ok(new ApiResponse(HttpStatusCode.OK, CategoryDto, "Success"));
        }
        #endregion



        #region Put
        [HttpPut("{id}", Name = "Update Category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Category>> Update([FromRoute] int id, [FromBody] Category model)
        {
            if (id <= 0 || model is null)
            {
                logging.Log("The id is less than zero or the Category is null", "Error");
                return BadRequest(new Response(400, "The id is less than zero or the Category is null"));
            }

            if (id != model.Id)
            {
                logging.Log("The id is not equal to the Category id", "Error");
                return BadRequest(new Response(400));
            }

            var Category = await unitOfWork.categoryRepository.GetByIdAsync(id);

            if (Category is null)
            {
                logging.Log("The Category is not exist", "Warning");
                return NotFound($"The Category with id={id} is not exist");
            }

            Category.Name = model.Name;
            Category.Description = model.Description;

            await unitOfWork.SaveChangesAsync();

            var CategoryDto = mapper.Map<Category, CategoryDTO>(Category);

            return Ok(new ApiResponse(HttpStatusCode.OK, CategoryDto, "Success"));
        }
        #endregion



        #region Delete
        [HttpDelete("{id}", Name = "Delete Category")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Category>> Delete([FromRoute] int id)
        {
            if (id <= 0)
            {
                logging.Log("The id is less than zero", "Error");
                return BadRequest(new Response(400));
            }

            var Category = await unitOfWork.categoryRepository.GetByIdAsync(id);
            if (Category is null)
            {
                logging.Log("The Category is not exist", "Warning");
                return NotFound($"The Category with id={id} is not exist");
            }

            await unitOfWork.categoryRepository.Delete(id);

            await unitOfWork.SaveChangesAsync();

            var CategoryDto = mapper.Map<Category, CategoryDTO>(Category);

            return Ok(new ApiResponse(HttpStatusCode.OK, CategoryDto, "Success"));
        }
        #endregion





    }
}
