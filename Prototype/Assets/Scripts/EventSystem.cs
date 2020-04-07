using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Attract, PlayerTurn, AITurn, GameOver };


public class EventSystem : MonoBehaviour
{

    public GameState CurrentState;
    public int players = 1;
    public string debugStateText;
    public Grid grid;
    public GameObject[] pieces;

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = GameState.Attract;
    }

    // Update is called once per frame
    void Update()
    {

        switch (CurrentState)
        { 

            case GameState.Attract:
                debugStateText = "Gamestate: Attract";
                //listen for the number of players and if player doesn't equal zero change state to PlayerMove
                break;
                

            case GameState.PlayerTurn:
                debugStateText = "Gamestate: PlayerTurn";
                //if no pieces have isPlayer switch CurrentState to GameOver
                //for pieces that have isPlayer set to true: highlight tiles (that aren't obstacles)
                //1 hex adjacent to the piece
                //if the player clicks on one, move() the player character to that hex
                //when done, switch CurrentState to AITurn

                //found this for getting mouse hex, not using it yet
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
                Debug.Log(coordinate);
                


                break;

            case GameState.AITurn:
                debugStateText = "Gamestate: AITurn";
                // for pieces that have isPlayer set to false:
                // speed of the piece is its range- if the pieces prey or the scent of the pray is inside the range, move to it
                // if no prey or scent of prey is in range: if a prey piece is on any tile on the board that is not isDark, move towards it
                // when done, switch CurrentState to PlayerTurn
                break;

            case GameState.GameOver:
                debugStateText = "Gamestate: Gameover";
                // play the sad song
                // set state to attract
                break;

            default:
                Debug.LogError("Invalid Gamestate");
                debugStateText = "Gamestate: ERROR";
                break;
        }
    }
}