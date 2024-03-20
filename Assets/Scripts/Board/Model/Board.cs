using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public Board(Vector2 size)
    {
        BoardSize = size;
    }

    public Vector2 BoardSize { get; private set; }

    private Dictionary<Vector2, TileData> vectorIndex = new Dictionary<Vector2, TileData>();
    private Dictionary<TileData, Vector2> tileIndex = new Dictionary<TileData, Vector2>();

    public Vector2 GetIndex(TileData tile)
    {
        return tileIndex.GetValueOrDefault(tile);
    }

    public bool TryGetIndex(TileData tile, out Vector2 index)
    {
        return tileIndex.TryGetValue(tile, out index);
    }

    public TileData GetTile(Vector2 index)
    {
        return vectorIndex.GetValueOrDefault(index);
    }

    public bool TryGetTile(Vector2 index, out TileData tile)
    {
        return vectorIndex.TryGetValue(index, out tile);
    }

    public void Set(TileData tile, Vector2 index)
    {
        vectorIndex[index] = tile;
        tileIndex[tile] = index;
    }

    public List<KeyValuePair<TileData, Vector2>> GetPairs()
    {
        return tileIndex.ToList();
    }

    public KeyValuePair<TileData, Vector2>[] GetColumn(int columnIndex)
    {
        KeyValuePair<TileData, Vector2>[] column = new KeyValuePair<TileData, Vector2>[(int)BoardSize.y];
        for (int i = 0; i < BoardSize.y; i++)
        {
            var index = new Vector2(columnIndex, i);
            column[i] = new KeyValuePair<TileData, Vector2>(GetTile(index), index);
        }
        return column;
    }

    public void Remove(TileData tile)
    {
        if (TryGetIndex(tile, out Vector2 index))
        {
            vectorIndex.Remove(index);
            tileIndex.Remove(tile);
        }
    }

    public void Remove(Vector2 index)
    {
        if(TryGetTile(index, out TileData tile))
        {
            vectorIndex.Remove(index);
            tileIndex.Remove(tile);
        }
    }

    public void Clear()
    {
        vectorIndex.Clear();
        tileIndex.Clear();
    }
}
