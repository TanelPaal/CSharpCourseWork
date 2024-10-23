using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            Console.ResetColor();
            if (y == 0)
            {
                for (var x = 0; x < gameInstance.DimX; x++)
                {
                    Console.Write("  ");
                    Console.Write(x);
                    Console.Write(" ");
                    if (x == gameInstance.DimX - 1) Console.Write(" ");
                }

                Console.WriteLine();
            }

            Console.Write(y);
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                SetBackgroundColor(x, y);
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x][y]) + " ");
                Console.ResetColor();
                if (x == gameInstance.DimX - 1) continue;
                Console.Write("|");
            }

            Console.WriteLine();
            if (y == gameInstance.DimY - 1) continue;
            Console.Write(" ");
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write("---");
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("+");
                }
            }

            Console.WriteLine();
        }


        void SetBackgroundColor(int x, int y)
        {
            if (gameInstance.IsInsidePlayableArea(x, y))
                // check if background coord is actually inside the grid if so colour it
                Console.BackgroundColor = ConsoleColor.DarkBlue;
        }
    }


    private static string DrawGamePiece(EGamePiece piece)
    {
        switch (piece)
        {
            case EGamePiece.X:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case EGamePiece.O:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
        }
        
        
        return piece switch
        {
            EGamePiece.Empty => " ",
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => "_"
        };
    }
}