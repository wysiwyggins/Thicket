using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PieceManager
{
    public static List<Piece> AllPieces = new List<Piece>();

    public static Piece GetPieceAtPos(Vector3 aPos)
    {
        foreach (Piece aPiece in AllPieces)
        {
            //if the piece is at aPos, return that piece
            //      return aPiece;
        }
        return null;
    }

    //public static Piece[] GetAllPiecesOfKind()
    //{
    //    //do some stuff
    //}
}