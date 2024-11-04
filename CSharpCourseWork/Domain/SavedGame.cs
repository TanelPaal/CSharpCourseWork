namespace Domain;

public class SavedGame
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string State { get; set; } = default!;

}