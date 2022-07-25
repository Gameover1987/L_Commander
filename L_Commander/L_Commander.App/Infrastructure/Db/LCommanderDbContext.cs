using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L_Commander.App.Infrastructure.Db
{
    public class FileEntity
    {
        [Key]
        public string Path { get; set; }

        public List<FileTagEntity> Tags { get; set; }
    }

    public class FileTagEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public FileEntity FileEntity { get; set; }

        [Required]
        public TagEntity TagEntity { get; set; }
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
            optionsBuilder.UseSqlite(@"Data Source=FileSystemTags.db");
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