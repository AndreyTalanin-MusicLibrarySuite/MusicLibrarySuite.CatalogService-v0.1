using MusicLibrarySuite.CatalogService.Interfaces.Entities.Base;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a non-generic wrapper for the <see cref="PageResponse{T}" /> class where <see cref="Genre" /> is the entity type.
/// </summary>
public class GenrePageResponse : PageResponse<Genre>
{
}
