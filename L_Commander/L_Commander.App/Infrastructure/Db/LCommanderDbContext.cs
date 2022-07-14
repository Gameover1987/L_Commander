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
        public string Path { get; set; }

        public List<TagEntity> Tags { get; set; }
    }

    public class TagEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int Color { get; set; }
    }

    public sealed class LCommanderDbContext : DbContext
    {
        private bool _isDisposing;

        public DbSet<FileEntity> Files { get; set; }

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
    }
}