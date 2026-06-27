using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;

namespace ProductApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IValidator<ItemCreateDto> _createValidator;

        public ItemsController(IItemService itemService, IValidator<ItemCreateDto> createValidator)
        {
            _itemService = itemService;
            _createValidator = createValidator;
        }

        /// <summary>Create an item under a product. Requires authentication.</summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ItemReadDto>> Create([FromBody] ItemCreateDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var result = await _itemService.CreateAsync(dto);
            return StatusCode(201, result);
        }
    }
}
