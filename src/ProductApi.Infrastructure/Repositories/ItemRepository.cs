using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Item>> GetByProductIdAsync(int productId)
        {
            return await _context.Items
                .AsNoTracking()
                .Where(i => i.ProductId == productId)
                .ToListAsync();
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.Products.AsNoTracking().AnyAsync(p => p.Id == productId);
        }

        public async Task AddAsync(Item item)
        {
            await _context.Items.AddAsync(item);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
