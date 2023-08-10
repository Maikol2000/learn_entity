using Microsoft.EntityFrameworkCore;

namespace entity.Db
{
	public class PgDbContext: DbContext, IAppDbContext
	{
		public DbSet<Article> Articles { get; set; }

        public PgDbContext(DbContextOptions<PgDbContext> options) : base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<Article>(entity =>
			{
				entity.ToTable("article");
				entity.Property(z => z.Id).HasColumnName("id");
                entity.Property(z => z.Title).HasColumnName("title");
                entity.Property(z => z.Content).HasColumnName("content");
                entity.Property(z => z.Url).HasColumnName("url");
            });
        }
    }
}

