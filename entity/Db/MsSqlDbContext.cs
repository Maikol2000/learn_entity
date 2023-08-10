using Microsoft.EntityFrameworkCore;

namespace entity.Db
{
	public class MsSqlDbContext: DbContext, IAppDbContext
	{
		public DbSet<Article> Articles { get; set; }

        public MsSqlDbContext(DbContextOptions<MsSqlDbContext> options) : base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<Article>(entity =>
			{
				entity.ToTable("Article");
				entity.Property(z => z.Id).HasColumnName("Id");
                entity.Property(z => z.Title).HasColumnName("Title");
                entity.Property(z => z.Content).HasColumnName("Content");
                entity.Property(z => z.Url).HasColumnName("Url");
            });
        }

    }
}

