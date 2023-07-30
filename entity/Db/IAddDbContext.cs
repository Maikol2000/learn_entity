using Microsoft.EntityFrameworkCore;

namespace entity.Db
{
    public interface IAppDbContext
    {
        DbSet<Article> Articles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
