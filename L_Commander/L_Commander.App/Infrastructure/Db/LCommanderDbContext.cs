using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace L_Commander.App.Infrastructure.Db
{
    public class FileEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FileGuid { get; set; }

        public string Path { get; set; }

        public List<FileTagEntity> Tags { get; set; }
    }

    public class FileTagEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid FileGuid { get; set; }

        [Required]
        public Guid TagGuid { get; set; }

        public virtual FileEntity FileEntity { get; set; }

        public virtual TagEntity TagEntity { get; set; }
    }

    public class TagEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TagGuid { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int Color { get; set; }
    }

    public sealed class LCommanderDbContext : DbContext
    {
        private bool _isDisposing;

        public DbSet<FileEntity> Files { get; set; }

        public DbSet<FileTagEntity> FileTags { get; set; }

        public DbSet<TagEntity> Tags { get; set; }

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
            optionsBuilder.UseSqlite(@"Data Source=Tags.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileEntity>(entity =>
                {
                    entity.HasKey(x => x.FileGuid);
                    entity.Property(x => x.FileGuid).ValueGeneratedOnAdd();
                });
            modelBuilder.Entity<FileTagEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<TagEntity>(entity =>
            {
                entity.HasKey(x => x.TagGuid);
                entity.Property(x => x.TagGuid).ValueGeneratedOnAdd();
            });
        }
    }
}