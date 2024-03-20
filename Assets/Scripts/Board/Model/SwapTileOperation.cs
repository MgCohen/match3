using System.Collections.Generic;

public class SwapTileOperation
{
    public void Execute(Board board, List<TileData> affectedTiles)
    {
        if (affectedTiles.Count != 2)
        {
            return;
        }

        var pos0 = board.GetIndex(affectedTiles[0]);
        var pos1 = board.GetIndex(affectedTiles[1]);
        board.Set(affectedTiles[0], pos1);
        board.Set(affectedTiles[1], pos0);
        BoardEvents.OnTileOperation(TileOperation.Swap, affectedTiles);
    }
}
