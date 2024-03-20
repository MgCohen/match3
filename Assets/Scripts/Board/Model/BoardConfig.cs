using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/BoardConfig")]
public class BoardConfig : ScriptableObject
{
    public Vector2 BoardSize => boardSize;
    [SerializeField] private Vector2 boardSize;

    public List<Color> PossibleColors => possibleColors;
    [SerializeField] private List<Color> possibleColors = new List<Color>();

    public TileView TilePrefab => tilePrefab;
    [SerializeField] private TileView tilePrefab;

}
