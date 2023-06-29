using Microsoft.EntityFrameworkCore;

namespace L_Commander.Database
{
    public sealed class LCommanderDbContext : DbContext
    {
        private bool _isDisposing;

        public DbSet<HistoryItemEntity> History { get; set; }

        public DbSet<FileEntity> Files { get; set; }

        public DbSet<FileTagEntity> FileTags { get; set; }

        public DbSet<TagEntity> Tags { get; set; }

        public LCommanderDbContext()
        {
            
        }

        public LCommanderDbContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
            Database.Migrate();
        }

        public bool IsDisposing => _isDisposing;

        public override void Dispose()
        {
            _isDisposing = true;
            base.Dispose();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=L_CommanderStorage.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileTagEntity>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<FileTagEntity>()
                .HasOne(x => x.FileEntity);
            modelBuilder.Entity<FileTagEntity>()
                .HasOne(x => x.TagEntity);

        }
    }
}