using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PieceManager
{
    
    public static List<Piece> AllPieces = new List<Piece>();

    public static Piece GetPieceAtPos(Vector3Int aCoord)
    {
        Grid grid = GameController.instance.grid;
        foreach (Piece aPiece in AllPieces)
        {
            Vector3Int coordinate = grid.WorldToCell(aPiece.transform.position);
            if (aCoord == coordinate)
            {
                return aPiece;
            }
        }
        return null;
    }

    //public static Piece[] GetAllPiecesOfKind()
    //{
    //    //do some stuff
    //}
}
