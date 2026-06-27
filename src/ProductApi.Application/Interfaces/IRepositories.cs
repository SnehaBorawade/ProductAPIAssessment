using ProductApi.Domain.Entities;

namespace ProductApi.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        void Update(Product product);
        void Delete(Product product);
        Task<bool> SaveChangesAsync();
    }

    public interface IItemRepository
    {
        Task<List<Item>> GetByProductIdAsync(int productId);
        Task<bool> ProductExistsAsync(int productId);
        Task AddAsync(Item item);
        Task<bool> SaveChangesAsync();
    }
}
