using System.ComponentModel.DataAnnotations;

namespace L_Commander.Database;

public class FileEntity
{
    [Key]
    public string Path { get; set; }

    public List<FileTagEntity> Tags { get; set; }
}