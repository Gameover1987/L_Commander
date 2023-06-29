using System;
using System.Collections.Generic;
using L_Commander.Database;

namespace L_Commander.App.Infrastructure;

public class TagEqualityComparer : IEqualityComparer<Tag>
{
    private static IEqualityComparer<Tag> _instance;

    private TagEqualityComparer()
    {

    }

    public static IEqualityComparer<Tag> Instance
    {
        get
        {
            if (_instance == null)
                _instance = new TagEqualityComparer();
            return _instance;
        }
    }

    public bool Equals(Tag x, Tag y)
    {
        if (x == null || y == null)
            return false;

        return x.Guid == y.Guid;
    }

    public int GetHashCode(Tag obj)
    {
        return obj.Guid.GetHashCode();
    }
}

public static class TagExtensions
{
    public static TagEntity ToEntity(this Tag tag)
    {
        return new TagEntity
        {
            TagGuid = tag.Guid,
            Color = tag.Color,
            Text = tag.Text
        };
    }

    public static Tag FromEntity(this TagEntity tagEntity)
    {
        return new Tag
        {
            Guid = tagEntity.TagGuid,
            Text = tagEntity.Text,
            Color = tagEntity.Color,
        };
    }

    public static Tag FromEntity(this FileTagEntity fileTagEntity)
    {
        return new Tag
        {
            Guid = fileTagEntity.TagEntity.TagGuid,
            Text = fileTagEntity.TagEntity.Text,
            Color = fileTagEntity.TagEntity.Color,
        };
    }
}

/// <summary>
/// Tag for file system entry
/// </summary>
public class Tag
{
    public Guid Guid { get; set; }

    public string Text { get; set; }

    public int Color { get; set; }
}

public class FileWithTags
{
    public string FilePath { get; set; }

    public Tag[] Tags { get; set; }
}