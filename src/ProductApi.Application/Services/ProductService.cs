using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;

namespace ProductApi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductReadDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var (items, totalCount) = await _productRepository.GetAllAsync(pageNumber, pageSize);

            return new PagedResult<ProductReadDto>
            {
                Items = items.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductReadDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with Id {id} was not found.");

            return MapToDto(product);
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto, string createdBy)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductReadDto> UpdateAsync(int id, ProductUpdateDto dto, string modifiedBy)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with Id {id} was not found.");

            product.ProductName = dto.ProductName;
            product.ModifiedBy = modifiedBy;
            product.ModifiedOn = DateTime.UtcNow;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Product with Id {id} was not found.");

            _productRepository.Delete(product);
            await _productRepository.SaveChangesAsync();
        }

        private static ProductReadDto MapToDto(Product product) => new()
        {
            Id = product.Id,
            ProductName = product.ProductName,
            CreatedBy = product.CreatedBy,
            CreatedOn = product.CreatedOn,
            ModifiedBy = product.ModifiedBy,
            ModifiedOn = product.ModifiedOn
        };
    }
}
