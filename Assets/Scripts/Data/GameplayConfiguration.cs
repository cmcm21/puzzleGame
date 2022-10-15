using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gameplay_configuration", menuName = "Data/GameplayConfig", order = 1)]
public class GameplayConfiguration : ScriptableObject
{
    [SerializeField] private int pieceWidth;
    public int PieceWidth => pieceWidth;

    [SerializeField] private int pieceHeight;
    public int PieceHeight => pieceHeight;

    [SerializeField] private int rows;
    public int Rows => rows;

    [SerializeField] private int columns;
    public int Columns => columns;

    [SerializeField] private Sprite[] sprites;
    public Sprite[] Sprites => sprites;
}
