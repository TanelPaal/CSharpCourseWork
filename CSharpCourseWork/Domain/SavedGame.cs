using System.ComponentModel.DataAnnotations;
namespace Domain;

public class SavedGame
{
    // Primary Key
    public int Id { get; set; }
    [MaxLength(128)] public string Name { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public string State { get; set; } = default!;

    // Expose the Foreign Key
    public int ConfigurationId { get; set; }
    public Configuration? Configuration { get; set; }



}