using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration
{
    public int Id { get; init; }
    [MaxLength(256)] public string Name { get; set; } = default!;

    [Required] public int BoardSizeWidth { get; set; } = 3;
    [Required] public int BoardSizeHeight { get; set; } = 3;
    
    // 0 disabled
    public int MovePieceAfterNMoves { get; set; } = 0;
    public int PieceLimit { get; set; } = 4;

    public override string ToString()
    {
        return $"{Id}_{Name} ({BoardSizeWidth} x {BoardSizeHeight} {MovePieceAfterNMoves} {PieceLimit})";
    }
}