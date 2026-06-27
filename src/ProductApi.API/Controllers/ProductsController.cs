using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;

namespace ProductApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] // simple API versioning via URL segment
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IItemService _itemService;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _updateValidator;

        public ProductsController(
            IProductService productService,
            IItemService itemService,
            IValidator<ProductCreateDto> createValidator,
            IValidator<ProductUpdateDto> updateValidator)
        {
            _productService = productService;
            _itemService = itemService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>Get all products (paginated). Public — no auth required.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ProductReadDto>>> GetAll(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>Get a single product by Id. Public — no auth required.</summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductReadDto>> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>Get items belonging to a product (related resource).</summary>
        [HttpGet("{id:int}/items")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ItemReadDto>>> GetItems(int id)
        {
            var result = await _itemService.GetByProductIdAsync(id);
            return Ok(result);
        }

        /// <summary>Create a new product. Requires authentication.</summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductReadDto>> Create([FromBody] ProductCreateDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var createdBy = User.Identity?.Name ?? "system";
            var result = await _productService.CreateAsync(dto, createdBy);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update an existing product. Requires authentication.</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ProductReadDto>> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var modifiedBy = User.Identity?.Name ?? "system";
            var result = await _productService.UpdateAsync(id, dto, modifiedBy);

            return Ok(result);
        }

        /// <summary>Delete a product. Requires authentication.</summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
