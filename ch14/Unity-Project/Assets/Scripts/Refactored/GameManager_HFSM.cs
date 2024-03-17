using System;
using UnityEngine;
using UnityHFSM;

public class GameManager_HFSM : MonoBehaviour
{
    private StateMachine _fsm;

    private bool _isConditionMetWin;
    private bool _isConditionMetLose;

    private void Start()
    {
        _fsm = new StateMachine();

        // Define all the states with their logic.
        _fsm.AddState("Menu", onEnter: state => ShowMenu());
        _fsm.AddState("Start", onEnter: state => StartGame());
        _fsm.AddState("Playing", onLogic: PlayingStateLogic());
        _fsm.AddState("Paused", onEnter: state => PauseGame(), onExit: state => ResumeGame());
        _fsm.AddState("GameOver", onEnter: state => EndGame());

        // Define all the transitions between states.
        _fsm.AddTransition("Menu", "Start", transition => Input.GetKeyDown(KeyCode.Space));      // Press space to start.
        _fsm.AddTransition("Start", "Playing", transition => true);                              // Automatically transition to playing after start.
        _fsm.AddTransition("Playing", "Paused", transition => Input.GetKeyDown(KeyCode.Escape)); // Press esc to pause.
        _fsm.AddTransition("Paused", "Playing", transition => Input.GetKeyDown(KeyCode.Escape)); // Press esc again to resume.
        _fsm.AddTransition("Playing", "GameOver", transition => IsGameOver());                   // Condition to end the game.
        _fsm.AddTransition("GameOver", "Menu", transition => Input.GetKeyDown(KeyCode.Space));   // Press space to return to menu.

        _fsm.SetStartState("Menu");
        _fsm.Init();
    }

    private void Update() => _fsm.OnLogic();

    private Action<State<string, string>> PlayingStateLogic()
    {
        // UNDONE: Evaluate conditions for win/lose and transition to game over state.
        //_isConditionMetWin;
        //_isConditionMetLose;

        //fsm.TransitionTo("GameOver");
        throw new NotImplementedException();
    }

    private void ShowMenu()
    {
        // UNDONE: Show menu UI.
        Debug.Log("Showing menu...");
    }

    private void StartGame()
    {
        // UNDONE: Initialize game start logic.
        Debug.Log("Game starting...");
    }

    private void PauseGame()
    {
        // UNDONE: Pause the game
        Debug.Log("Game Paused");
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        // UNDONE: Resume the game.
        Debug.Log("Game Resumed");
        Time.timeScale = 1f;
    }

    private void EndGame()
    {
        // UNDONE: Handle logic to end the gameplay.
        Debug.Log("Game Over");
        PauseGame();
    }

    private bool IsGameOver()
    {
        // UNDONE: Implement your game over predicate condition.
        return false;
    }
}