using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L_Commander.Database;

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