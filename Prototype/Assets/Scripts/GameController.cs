using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum GameState { Attract, PlayerTurn, AITurn, GameOver };


public class GameController : MonoBehaviour
{
    public static GameController instance;

    public int players = 1;
    public Grid grid;
    public GameObject[] pieces;
    public static GameState CurrentState { get; set; }
    public Text debugStateText;

    public AudioSource audioSource;
    public AudioClip invalidSound;
    public float volume = 0.5f;
    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        CurrentState = GameState.Attract;

    }

    private void OnEnable()
    {
        //subscribe to the piece.oncompletemove event
        Piece.OnCompleteMove += Piece_OnCompleteMove;
        Piece.InvalidInput += InvalidInput;
    }

    private void Piece_OnCompleteMove()
    {
        //this is called when the piece completes a move.
        if (CurrentState == GameState.PlayerTurn)
            CurrentState = GameState.AITurn;
    }

    private void OnDisable()
    {
        //unsubscribe
        Piece.OnCompleteMove -= Piece_OnCompleteMove;
    }

    private void InvalidInput()
    {
        audioSource.PlayOneShot(invalidSound, volume);
        Debug.Log("Invalid Move");
        // bug, player currently can't move after a move is invalid
    }


    // Update is called once per frame
    void Update()
    {

        switch (CurrentState)
        {

            case GameState.Attract:
                debugStateText.text = "Gamestate: Attract";
                //listen for the number of players and if player doesn't equal zero change state to PlayerMove
                // for now we'll just click to start
                if (Input.GetMouseButtonDown(0))
                {
                    CurrentState = GameState.PlayerTurn;
                }
                break;


            case GameState.PlayerTurn:
                debugStateText.text = "Gamestate: PlayerTurn";
                //if no pieces have isPlayer switch CurrentState to GameOver
                //for pieces that have isPlayer set to true: highlight tiles (that aren't obstacles)
                //1 hex adjacent to the piece
                //if the player clicks on one, move() the player character to that hex
                //when done, switch CurrentState to AITurn

                break;

            case GameState.AITurn:
                debugStateText.text = "Gamestate: AITurn";
                // for pieces that have isPlayer set to false:
                // if the pieces prey or the scent of the pray is inside the range, move to it
                // if no prey or scent of prey is in range: if a prey piece is on any tile on the board that is not isDark, move towards it
                // when done, switch CurrentState to PlayerTurn
                break;

            case GameState.GameOver:
                debugStateText.text = "Gamestate: Gameover";
                // play the sad song
                // set state to attract
                break;

            default:
                Debug.LogError("Invalid Gamestate");
                debugStateText.text = "Gamestate: ERROR";
                break;
        }
    }
}