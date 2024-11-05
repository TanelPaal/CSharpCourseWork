using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration
{
    public int Id { get; set; } // Primary key
    
    [MaxLength(128)] public string Name { get; set; } = default!;

    [Required] public int BoardSizeWidth { get; set; }
    [Required] public int BoardSizeHeight { get; set; }
    
    // 0 disabled
    public int MovePieceAfterNMoves { get; set; } = 0;
    public int PieceLimit { get; set; } = 4;

    public ICollection<SavedGame>? SaveGames { get; set; }

    
    public override string ToString()
    {
        return $"{Id}_{Name} ({BoardSizeWidth} x {BoardSizeHeight} {MovePieceAfterNMoves} {PieceLimit})";
    }

    public Configuration(string name, int boardSizeWidth, int boardSizeHeight, int movePieceAfterNMoves, int pieceLimit)
    {
        // Overwrite default values with provided values
        Name = name;
        BoardSizeWidth = boardSizeWidth;
        BoardSizeHeight = boardSizeHeight;
        MovePieceAfterNMoves = movePieceAfterNMoves;
        PieceLimit = pieceLimit;
    }
}