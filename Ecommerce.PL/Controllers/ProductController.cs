using Ecommerce.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Core.IGenericRepository;
using AutoMapper;
using Ecommerce.PL.DTO;
using Ecommerce.Core.ILog;
using Ecommerce.PL.CustomizeResponses;
using System.Net;
using Ecommerce.PL.Helper;

namespace Ecommerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogging logging;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IMapper mapper, IUnitOfWork unitOfWork, ILogging logging, IWebHostEnvironment webHostEnvironment)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.logging = logging;
            this.webHostEnvironment = webHostEnvironment;
        }


        #region  Get
        //[Authorize(Roles ="User")]
        [HttpGet("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ResponseCache(CacheProfileName = "500SecondsDuration")]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetAllProducts([FromQuery] string? ProductName, [FromQuery] int? MaxPriceOfProduct, [FromQuery] string? ProductColor, [FromQuery] string? CategoryNameForProduct)
        {

            logging.Log("Get All Products", "Information");// this statement will be logged in the console
            var products = await unitOfWork.productRepository.GetAllAsync();

            if (products is null)
            {
                logging.Log("There is no products", "Warning");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add("There is no Products");
                return NotFound(errorResponse);
            }

            if (!string.IsNullOrEmpty(ProductName))
            {
                ProductName = ProductName.ToLower();
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

            var productsDto = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductResponseDTO>>(products);

            return Ok(new SuccessResponse("Success", productsDto));
        }


        [HttpGet("[action]/{id:int}")] // we put bracket to make the variable dynamic => if we but [HttpGet("id")] then the id will be static (string id)
        [ProducesResponseType(200)] //mean that the method will return 200 status code and the process is success
        [ProducesResponseType(404)] // not found
        [ProducesResponseType(400)] // bad request
        [ResponseCache(CacheProfileName = "500SecondsDuration")]

        public async Task<ActionResult<Product>> GetProductById([FromRoute] int id)
        {
            if (id <= 0)
            {
                logging.Log("Error becouse the Id is Incorrect", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Error becouse the Id is Incorrect");
                return BadRequest(errorResponse);
            }

            Product? product = await unitOfWork.productRepository.GetByIdAsync(id);

            if (product is null)
            {
                logging.Log($"Product of {id} not exist", "Warning");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add($"Product of {id} not exist");
                return NotFound(errorResponse);
            }

            ProductResponseDTO productDto = mapper.Map<Product, ProductResponseDTO>(product);

            return Ok(new SuccessResponse("Success", productDto));

        }



        [HttpGet("[action]/{CategoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ResponseCache(CacheProfileName = "500SecondsDuration")]
        public async Task<ActionResult<Product>> GetProductsByCategoryId([FromRoute] int CategoryId)
        {
            if (CategoryId <= 0)
            {
                logging.Log("Error Becouse the id is Invalid", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Error Becouse the id is Invalid");
                return BadRequest(errorResponse);
            }

            var products = await unitOfWork.productRepository.GetProductsByCategoryAsync(CategoryId);

            if (products is null)
            {
                logging.Log($"Categgory of {CategoryId} not exist", "Warning");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add($"The Category with id={CategoryId} is not exist");
                return NotFound(errorResponse);
            }

            var productsDto = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductResponseDTO>>(products);

            return Ok(new SuccessResponse("Success", productsDto));

        }




        [HttpGet]
        [Route("GetImage/{imageName}")]
        public IActionResult GetImage([FromRoute] string imageName)
        {
            string FolderName = "images";
            string imagePath = Path.Combine(webHostEnvironment.WebRootPath, "files", FolderName, imageName);
            if (System.IO.File.Exists(imagePath))
            {
                FileStream? image = System.IO.File.OpenRead(imagePath);
                return File(image, "image/png");
            }
            ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
            errorResponse.Errors.Add("Image Not Found");
            return NotFound(errorResponse);

            //to make images $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}/api/Product/GetImage/{imageName}" 
        }

        #endregion


        #region Post

        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        //[Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Admin,User")] // this line to make the method only for the admin and the user Roles

        public async Task<ActionResult<Product>> CreateProduct([FromForm] ProductRequestDTO model)
        {
            if (model is null)
            {
                logging.Log("The product is null", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("The product is null");
                return BadRequest(errorResponse);
            }

            //if (model.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);


            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Amount = model.Amount,
                    Price = model.Price,
                    Color = model.Color,
                    ImageUrl = FileManagement.UploadFile(model.image).Result,
                    CategoryId = model.CategoryId
                };


                await unitOfWork.productRepository.AddAsync(product);

                await unitOfWork.SaveChangesAsync();

                var productDto = mapper.Map<Product, ProductResponseDTO>(product);

                return Ok(new SuccessResponse("Product Created SuccessFully", productDto));
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
        public async Task<ActionResult<Product>> UpdateProduct([FromRoute] int id, [FromForm] ProductRequestDTO model)
        {
            if (id <= 0)
            {
                logging.Log("Error becouse the Id is Incorrect", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Error becouse the Id is Incorrect");
                return BadRequest(errorResponse);
            }

            if (model is null)
            {
                logging.Log("The product is null", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("The product is null");
                return BadRequest(errorResponse);
            }

            Product? product = await unitOfWork.productRepository.GetByIdAsync(id);

            if (product is null)
            {
                logging.Log($"Product of {id} not exist", "Warning");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add($"Product of {id} not exist");
                return NotFound(errorResponse);
            }


            if (ModelState.IsValid)
            {

                FileManagement.DeleteFile(product.ImageUrl);


                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Amount = model.Amount;
                product.Color = model.Color;
                product.ImageUrl = FileManagement.UploadFile(model.image).Result;
                product.CategoryId = model.CategoryId;

                await unitOfWork.SaveChangesAsync();

                ProductResponseDTO productDto = mapper.Map<Product, ProductResponseDTO>(product);

                return Ok(new SuccessResponse("Success", productDto));

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


        #region Delete
        [HttpDelete("[action]/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Product>> DeleteProduct([FromRoute] int id)
        {
            if (id <= 0)
            {
                logging.Log("Error becouse the Id is Incorrect", "Error");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.BadRequest);
                errorResponse.Errors.Add("Error becouse the Id is Incorrect");
                return BadRequest(errorResponse);
            }


            Product? product = await unitOfWork.productRepository.GetByIdAsync(id);

            if (product is null)
            {
                logging.Log($"Product of {id} not exist", "Warning");
                ErrorResponse errorResponse = new ErrorResponse(HttpStatusCode.NotFound);
                errorResponse.Errors.Add($"Product of {id} not exist");
                return NotFound(errorResponse);
            }


            FileManagement.DeleteFile(product.ImageUrl);

            await unitOfWork.productRepository.Delete(id);

            await unitOfWork.SaveChangesAsync();

            var productDto = mapper.Map<Product, ProductResponseDTO>(product);

            return Ok(new SuccessResponse("Success", productDto));
        }
        #endregion

    }
}
