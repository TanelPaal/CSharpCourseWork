namespace GameBrain;

public record struct GameConfiguration()
{
    public string Name { get; set; } = default!;

    public int BoardSizeWidth { get; set; } = 3;
    public int BoardSizeHeight { get; set; } = 3;


    // 0 disabled
    public int MovePieceAfterNMoves { get; set; } = 0;
    public int PieceLimit { get; set; } = 4;
    

    public override string ToString() =>
        $"Board {BoardSizeWidth}x{BoardSizeHeight}, " +
        $"can move piece after {MovePieceAfterNMoves} moves";
}