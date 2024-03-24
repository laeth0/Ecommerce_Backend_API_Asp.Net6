using Ecommerce.Repository.Data;
using Ecommerce.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.IGenericRepository;
using AutoMapper;
using Ecommerce.PL.DTO;
using Ecommerce.Core.ILog;
using Ecommerce.PL.CustomizeResponses;
using System.Net;

namespace Ecommerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogging logging;

        public ProductController(IMapper mapper, IUnitOfWork unitOfWork, ILogging logging)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.logging = logging;
        }


        #region  Get

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ResponseCache(CacheProfileName = "500SecondsDuration")]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetAll([FromQuery] string? ProductName, [FromQuery] int? MaxPriceOfProduct, [FromQuery] string? ProductColor, [FromQuery] string? CategoryNameForProduct)
        {
            logging.Log("Get All Estate", "Information");// this statement will be logged in the console
            var products = await unitOfWork.productRepository.GetAllAsync();

            if(products is null)
            {
                logging.Log("There is no products", "Warning");
                return NotFound(new Response(404, "There is no products"));
            }

            if(products.Count==0)
            {
                logging.Log("There is no products", "Warning");
                return NotFound(new Response(404, "There is no products"));
            }

            if (!string.IsNullOrEmpty(ProductName))
            {
                ProductName=ProductName.ToLower();
                products = products.Where(p => p.Name.ToLower() == ProductName).ToList();
            }

            if (MaxPriceOfProduct is not null) 
            {
                products = products.Where(p => p.Price <= MaxPriceOfProduct).ToList();
            }

            if (ProductColor is not null)
            {
                ProductColor = ProductColor.ToLower();
                products = products.Where(p => p.Color.ToLower() == ProductColor).ToList();
            }

            if (CategoryNameForProduct is not null)
            {
                CategoryNameForProduct = CategoryNameForProduct.ToLower();
                products = products.Where(p => p.Category?.Name.ToLower() == CategoryNameForProduct).ToList();
            }

            var productsDto = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDTO>>(products);

            return Ok(new ApiResponse(HttpStatusCode.OK, productsDto, "Success"));
        }


        [HttpGet("{id}", Name = "GetProductById")] // we put bracket to make the variable dynamic => if we but [HttpGet("id")] then the id will be static (string id)
        [ProducesResponseType(200)] //mean that the method will return 200 status code and the process is success
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(400)] // bad request
        [ResponseCache(CacheProfileName = "500SecondsDuration")]
        public async Task<ActionResult<Product>> GetById([FromRoute]int id)
        {
            if (id <= 0)
            {
                logging.Log("error becouse the id is less than zero", "Error");
                return BadRequest(new Response(400));
            }

            var product = await unitOfWork.productRepository.GetByIdAsync(id);

            if (product is null)
            {
                logging.Log($"Product of {id} not exist", "Warning");
                return NotFound(new Response(404, $"The product with id={id} is not exist"));
            }

            var productDto = mapper.Map<Product, ProductDTO>(product);

            return Ok(new ApiResponse(HttpStatusCode.OK, productDto, "Success"));

        }

        //private bool ProductExists(int id)
        //{
        //    return (dbContext.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        //}

        #endregion


        #region Post

        [HttpPost(Name ="Create Product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> Create([FromBody]Product product)
        {
            if (product is null)
            {
                logging.Log("The product is null", "Error");
                return BadRequest(new Response(400));
            }
            if (product.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);

            await unitOfWork.productRepository.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            var productDto = mapper.Map<Product, ProductDTO>(product);

            return Ok(new ApiResponse(HttpStatusCode.OK, productDto, "Success"));
        }
        #endregion


        #region Put
        [HttpPut("{id}", Name = "Update Product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> Update([FromRoute] int id, [FromBody] Product model)
        {
            if (id <= 0 || model is null)
            {
                logging.Log("The id is less than zero or the product is null", "Error");
                return BadRequest(new Response(400));
            }

            if (id != model.Id)
            {
                logging.Log("The id is not equal to the product id","Error");
                return BadRequest(new Response(400, "The id is not equal to the product id"));
            }

            var product=await unitOfWork.productRepository.GetByIdAsync(id);

            if (product is null)
            {
                logging.Log($"The product with id={id} is not exist", "Warning");
                return NotFound(new Response(404, $"The product with id={id} is not exist"));
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;
            product.ImageUrl = model.ImageUrl;

            await unitOfWork.SaveChangesAsync();

            var productDto = mapper.Map<Product, ProductDTO>(product);

            return Ok(new ApiResponse(HttpStatusCode.OK, productDto, "Success"));
        }
        #endregion


        #region Delete
        [HttpDelete("{id}", Name = "Delete Product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Product>> Delete([FromRoute] int id)
        {
            if (id <= 0)
            {
                logging.Log("The id is less than zero", "Error");
                return BadRequest(new Response(400, "The id is less than zero"));
            }

            var product = await unitOfWork.productRepository.GetByIdAsync(id);
            if (product is null)
            {
                logging.Log("The product is not exist", "Warning");
                return NotFound(new Response(404, $"The product with id={id} is not exist"));
            }

            await unitOfWork.productRepository.Delete(id);

            await unitOfWork.SaveChangesAsync();

            var productDto = mapper.Map<Product, ProductDTO>(product);

            return Ok(new ApiResponse(HttpStatusCode.OK, productDto, "Success"));
        }
        #endregion

    }
}
