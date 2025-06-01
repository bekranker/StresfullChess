using UnityEngine;

[CreateAssetMenu(fileName = "New Piece Data", menuName = "Chess/ChessPieceData")]
public class ChessPieceData : ScriptableObject
{
    public Sprite PieceSprite;
    public int StressAmount;
    public int BaseAmount;
    public Vector2Int[] movementPattern;
}