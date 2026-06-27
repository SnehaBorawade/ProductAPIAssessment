using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;

namespace ProductApi.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<List<ItemReadDto>> GetByProductIdAsync(int productId)
        {
            var exists = await _itemRepository.ProductExistsAsync(productId);
            if (!exists)
                throw new NotFoundException($"Product with Id {productId} was not found.");

            var items = await _itemRepository.GetByProductIdAsync(productId);

            return items.Select(i => new ItemReadDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();
        }

        public async Task<ItemReadDto> CreateAsync(ItemCreateDto dto)
        {
            var exists = await _itemRepository.ProductExistsAsync(dto.ProductId);
            if (!exists)
                throw new NotFoundException($"Product with Id {dto.ProductId} was not found.");

            var item = new Item
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();

            return new ItemReadDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };
        }
    }
}
