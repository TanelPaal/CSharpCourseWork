namespace GameBrain;

public record struct GameConfiguration()
{
    public string Name { get; set; } = default!;

    public int BoardSizeWidth { get; set; } = 3;
    public int BoardSizeHeight { get; set; } = 3;

    // Number of pieces needed in a straight line to win the game.
    public int WinCondition { get; set; } = 3;

    // 0 disabled
    public int MovePieceAfterNMoves { get; set; } = 0;

    public int[] PlayableAreaPos { get; set; } = { 1, 1 };
    

    public override string ToString() =>
        $"Board {BoardSizeWidth}x{BoardSizeHeight}, " +
        "to win: {WinCondition}, " +
        "can move piece after {MovePieceAfterNMoves} moves";
}