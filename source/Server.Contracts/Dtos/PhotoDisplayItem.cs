using System.Diagnostics.CodeAnalysis;
using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public record PhotoDisplayItem
{
    public PhotoDisplayItem(JobCompletedPhotoId? id, string? path, bool containsPhoto = true)
    {
        Id = id;
        Path = path;
        ContainsPhoto = containsPhoto;
    }

    [MemberNotNullWhen(true, nameof(ContainsPhoto))]
    public JobCompletedPhotoId? Id { get; init; }

    [MemberNotNullWhen(true, nameof(ContainsPhoto))]
    public string Path { get; init; }

    public bool ContainsPhoto { get; init; }

    public static PhotoDisplayItem CreateEmpty() => new(null, null, false);
    public static PhotoDisplayItem CreateWith(JobCompletedPhotoId id, string path) => new(id, path, true);
};