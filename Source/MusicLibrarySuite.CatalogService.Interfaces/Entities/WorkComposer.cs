using System;
using System.ComponentModel.DataAnnotations;

namespace MusicLibrarySuite.CatalogService.Interfaces.Entities;

/// <summary>
/// Represents a work-to-artist relationship where the artist has the "composer" role.
/// </summary>
public class WorkComposer
{
    /// <summary>
    /// Gets or sets the work's unique identifier.
    /// </summary>
    [Required]
    public Guid WorkId { get; set; }

    /// <summary>
    /// Gets or sets the artist's unique identifier.
    /// </summary>
    [Required]
    public Guid ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the relationship's display order.
    /// </summary>
    [Required]
    public int Order { get; set; }
}
