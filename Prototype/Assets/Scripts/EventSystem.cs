using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Attract, PlayerTurn, AITurn, GameOver};
public int players;

public class EventSystem : MonoBehaviour
{

    public GameState CurrentState;

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = GameState.Attract;
    }

    // Update is called once per frame
    void Update()
    {
        if CurrentState == GameState.Attract {
            //listen for the number of players and if player doesn't equal zero change state to PlayerMove
        }

        if CurrentState == GameState.PlayerTurn {

            //for pieces that have isPlayer set to true: highlight tiles (that aren't obstacles) 1 hex adjacent to the piece
            //if the player clicks on one, move() the player character to that hex
            //when done, switch CurrentState to AITurn
        }

        if CurrentState == GameState.AITurn {
            // for pieces that have isPlayer set to false:
            // speed of the piece is its range- if the pieces prey or the scent of the pray is inside the range, move to it
            // if no prey or scent of prey is in range: if a prey piece is on any tile on the board that is not isDark, move towards it
            // when done, switch CurrentState to PlayerTurn
        }
    }
}
