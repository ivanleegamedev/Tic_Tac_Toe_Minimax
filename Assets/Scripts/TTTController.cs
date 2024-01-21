using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTTController : MonoBehaviour
{
    [SerializeField]
    private List<Button> buttons = new List<Button>();

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Text outcomeText;

    private bool isGameOver = false;
    private int fieldsLeft = 9;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        foreach (Button button in buttons)
        {
            button.GetComponentInChildren<Text>().text = "";
            button.interactable = true;
        }
        isGameOver = false;
        fieldsLeft = 9;
        AiTurn();
    }

    private void AiTurn()
    {
        if (fieldsLeft == 9)
        {
            // Since the corners will always be the best first move, we can just pick one randomly to speed up the process on the first turn
            int[] corners = new int[] { 0, 2, 6, 8 };
            int cornerIndex = corners[UnityEngine.Random.Range(0, corners.Length)];

            buttons[cornerIndex].GetComponentInChildren<Text>().text = "O";
            buttons[cornerIndex].interactable = false;
            fieldsLeft--;
        }
        else
        {
            int bestScore = Int32.MinValue;
            int bestMove = 0;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (IsFieldEmpty(buttons[i]))
                {
                    buttons[i].GetComponentInChildren<Text>().text = "O";
                    fieldsLeft--;
                    int score = Minimax(0, false);
                    buttons[i].GetComponentInChildren<Text>().text = "";
                    fieldsLeft++;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = i;
                    }
                }
            }

            buttons[bestMove].GetComponentInChildren<Text>().text = "O";
            buttons[bestMove].interactable = false;
            fieldsLeft--;
        }

        if (CheckForWin("O") || fieldsLeft == 0)
        {
            EndGame();
        }
    }

    public void OnButtonClick(Button button)
    {
        if (isGameOver || !IsFieldEmpty(button))
        {
            return;
        }

        button.GetComponentInChildren<Text>().text = "X";
        fieldsLeft--;
        button.interactable = false;

        if (CheckForWin("X") || fieldsLeft == 0)
        {
            EndGame();
        }
        else
        {
            AiTurn();
        }
    }

    private int Minimax(int depth, bool isMaximizing)
    {
        string currentPlayer = isMaximizing ? "O" : "X";
        if (CheckForWin(currentPlayer))
        {
            return isMaximizing ? 10 - depth : depth - 10;
        }
        if (fieldsLeft == 0)
        {
            return 0;
        }

        int bestScore = isMaximizing ? Int32.MinValue : Int32.MaxValue;
        foreach (Button button in buttons)
        {
            if (IsFieldEmpty(button))
            {
                button.GetComponentInChildren<Text>().text = currentPlayer;
                fieldsLeft--;
                int score = Minimax(depth + 1, !isMaximizing);
                button.GetComponentInChildren<Text>().text = "";
                fieldsLeft++;
                bestScore = isMaximizing ? Math.Max(score, bestScore) : Math.Min(score, bestScore);
            }
        }

        return bestScore;
    }

    private Text GetText(Button button)
    {
        return button.GetComponentInChildren<Text>();
    }

    private bool CheckForWin(string mark)
    {
        // Horizontal
        if (CompareButtons(0, 1, 2, mark) || CompareButtons(3, 4, 5, mark) || CompareButtons(6, 7, 8, mark))
        {
            return true;
        }
        // Vertical
        if (CompareButtons(0, 3, 6, mark) || CompareButtons(1, 4, 7, mark) || CompareButtons(2, 5, 8, mark))
        {
            return true;
        }
        // Diagonal
        if (CompareButtons(0, 4, 8, mark) || CompareButtons(2, 4, 6, mark))
        {
            return true;
        }
        return false;
    }

    private bool CompareButtons(int index1, int index2, int index3, string mark)
    {
        Text text1 = GetText(buttons[index1]);
        Text text2 = GetText(buttons[index2]);
        Text text3 = GetText(buttons[index3]);
        bool equal =
            text1.text == mark &&
            text2.text == mark &&
            text3.text == mark;
        return equal;
    }

    private bool IsFieldEmpty(Button button)
    {
        return button.GetComponentInChildren<Text>().text == "";
    }

    private void EndGame()
    {
        isGameOver = true;
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
        if (CheckForWin("X"))
        {
            outcomeText.text = "You won!";
        }
        else if (CheckForWin("O"))
        {
            outcomeText.text = "You lost!";
        }
        else
        {
            outcomeText.text = "It's a draw!";
        }
    }

    public void RestartGame()
    {
        // Reset the game board and variables
        foreach (Button button in buttons)
        {
            button.GetComponentInChildren<Text>().text = "";
            button.interactable = true;
        }
        outcomeText.text = "";
        isGameOver = false;
        fieldsLeft = 9;

        AiTurn();
    }
}
