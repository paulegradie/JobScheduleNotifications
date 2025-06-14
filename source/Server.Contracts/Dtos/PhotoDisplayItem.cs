using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public record PhotoDisplayItem(JobCompletedPhotoId Id, string Path)
{
    public bool ContainsPhoto { get; init; }
};