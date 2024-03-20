using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchTilesOperation
{
    public List<Vector2> Execute(Board board, List<TileData> affectedTiles)
    {
        List<List<TileData>> matchList = new List<List<TileData>>();
        foreach(var tile in affectedTiles)
        {
            List<TileData> tileMatches = null;
            if(CheckGroup(board, tile, Vector2.up, out List<TileData> columnMatches))
            {
                tileMatches = columnMatches;
            }

            if (CheckGroup(board, tile, Vector2.right, out List<TileData> lineMatches) && lineMatches.Count > columnMatches.Count)
            {
                tileMatches = lineMatches;
            }

            if(tileMatches != null)
            {
                matchList.Add(tileMatches);
            }
        }

        if(matchList.Count <= 0)
        {
            return new List<Vector2>();
        }

        matchList = matchList.OrderBy(ml => ml.Count).ToList();
        affectedTiles.Clear();
        foreach(var matches in matchList)
        {
            if (affectedTiles.Intersect(matches).Any())
            {
                continue;
            }
            affectedTiles.AddRange(matches);
        }

        List<Vector2> emptySpaces = new List<Vector2>();
        foreach(var tile in affectedTiles)
        {
            emptySpaces.Add(board.GetIndex(tile));
            board.Remove(tile);
        }

        BoardEvents.OnTileOperation(TileOperation.Destroy, affectedTiles);
        return emptySpaces;
    }
  

    private bool CheckGroup(Board board, TileData tile, Vector2 direction, out List<TileData> matches)
    {
        matches = new List<TileData>() { tile };
        if (CheckSegment(board, tile, direction, out List<TileData> found))
        {
            matches.AddRange(found);
        }

        if (CheckSegment(board, tile, -direction, out found))
        {
            matches.AddRange(found);
        }

        return matches.Count >= 3;
    }

    private bool CheckSegment(Board board, TileData source, Vector2 direction, out List<TileData> tilesFound)
    {
        tilesFound = new List<TileData>();
        Vector2 currentIndex = board.GetIndex(source);
        while (source != null)
        {
            currentIndex += direction;
            if (!board.TryGetTile(currentIndex, out TileData tile))
            {
                break;
            }

            if (tile.color != source.color)
            {
                break;
            }

            tilesFound.Add(tile);
        }

        return tilesFound.Count > 0;
    }
}
