using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FillTilesOperation
{
    public void Execute(BoardController controller, Board board, List<Vector2> emptySpaces)
    {
        List<TileData> affectedTiles = new List<TileData>();
        var columns = emptySpaces.GroupBy(v => v.x);
        foreach (var column in columns)
        {
            var tiles = board.GetColumn((int)column.Key);
            var minY = (int)column.Min(v => v.y);
            var moves = column.Count();
            for (int i = minY; i < board.BoardSize.y; i++)
            {
                if (tiles[i].Key != null)
                {
                    var newPos = tiles[i].Value + (Vector2.down * moves);
                    board.Remove(tiles[i].Key);
                    board.Set(tiles[i].Key, newPos);
                    affectedTiles.Add(tiles[i].Key);
                }
            }

            for (int i = (int)board.BoardSize.y - 1; i >= 0; i--)
            {
                var index = new Vector2((int)column.Key, i);
                if (!board.TryGetTile(index, out TileData tile))
                {
                    Debug.Log("Creting new tile at " + index);
                    tile = controller.CreateTile(index);
                    affectedTiles.Add(tile);
                }
            }
        }
        BoardEvents.OnTileOperation(TileOperation.Fill, affectedTiles);
    }
}
