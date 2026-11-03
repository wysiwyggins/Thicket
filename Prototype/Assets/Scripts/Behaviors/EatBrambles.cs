using System.Collections.Generic;
using UnityEngine;

// Eats one patch of brambles next to the piece each turn, opening up the hex.
// Used by the turtle: put it after MoveTowardsTarget in the piece's behaviours.
public class EatBrambles : PieceBehaviour
{
	public string FoodName = "Brambles"; // matches Scenery.SceneryName

	public override void Begin()
	{
		Grid grid = PieceManager.Instance.grid;
		Vector3Int cell = grid.WorldToCell(transform.position);

		var food = new List<Scenery>();
		foreach (Vector3Int neighbor in HexCoordinates.GetNeighborsAtPos(cell))
		{
			foreach (Scenery scenery in SceneryManager.GetSceneriesAtPos(neighbor))
			{
				if (scenery != null && scenery.SceneryName == FoodName)
					food.Add(scenery);
			}
		}

		if (food.Count > 0)
		{
			Scenery meal = food[Random.Range(0, food.Count)];
			MessageManager.AddMessage("The " + piece.PieceName + " eats through the " + FoodName.ToLower() + ".");
			meal.Remove();
		}

		SendCompleteMessage();
	}
}
