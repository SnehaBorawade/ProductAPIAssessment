using ProductApi.Application.DTOs;

namespace ProductApi.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductReadDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductReadDto> GetByIdAsync(int id);
        Task<ProductReadDto> CreateAsync(ProductCreateDto dto, string createdBy);
        Task<ProductReadDto> UpdateAsync(int id, ProductUpdateDto dto, string modifiedBy);
        Task DeleteAsync(int id);
    }

    public interface IItemService
    {
        Task<List<ItemReadDto>> GetByProductIdAsync(int productId);
        Task<ItemReadDto> CreateAsync(ItemCreateDto dto);
    }

    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(string username);
    }
}
