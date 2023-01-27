using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L_Commander.Database;

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