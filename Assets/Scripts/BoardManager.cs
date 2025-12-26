using System.Collections;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public enum Player
    {
        None,
        Player1,
        Player2
    }

    public enum CellState
    {
        Empty,
        Player1,
        Player2
    }

    [Header("References")]
    [SerializeField] private BoardButtonsManager boardButtonsManager;

    [Header("Settings")]
    [SerializeField] private int[] ammoThresholds = new int[2] { 3, 6 };

    private Player currentPlayer;
    private Player lastShooter;

    private int rows = 6;
    private int columns = 7;

    private CellState[,] board = new CellState[6, 7];

    private bool p1CanShoot;
    private bool p2CanShoot;

    private int p1AmmoGained;
    private int p2AmmoGained;

    private void Start()
    {
        currentPlayer = Player.Player1;
        lastShooter = Player.None;

        p1CanShoot = false;
        p2CanShoot = false;

        p1AmmoGained = 0;
        p2AmmoGained = 0;

        // Initialize board to empty
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                board[r, c] = CellState.Empty;
            }
        }
    }

    #region Player Actions

    public bool PlacePiece(int column)
    {
        if (!TryGetLowestEmptyRow(column, out int row))
            return false;

        board[row, column] = currentPlayer == Player.Player1 ? CellState.Player1 : CellState.Player2;
        lastShooter = Player.None;

        CheckWinCondition(board[row, column]);
        AmmoThresholdCheck(currentPlayer);
        StartCoroutine(SwitchTurn());

        return true;
    }

    private bool TryGetLowestEmptyRow(int column, out int row)
    {
        for (int y = 0; y < rows; y++)
        {
            if (board[y, column] == CellState.Empty)
            {
                row = y;
                return true;
            }
        }
        row = -1;
        return false;
    }

    #endregion

    #region Piece Checking

    private void AmmoThresholdCheck(Player player)
    {
        int ownedCount = CountPiecesOnOwnedSide(player);

        if (player == Player.Player1)
        {
            for (int i = 0 + p1AmmoGained; i < ammoThresholds.Length; i++)
            {
                if (ownedCount == ammoThresholds[i])
                {
                    p1CanShoot = true;
                    return;
                }
            }
        }
        else if (player == Player.Player2)
        {
            for (int i = 0 + p2AmmoGained; i < ammoThresholds.Length; i++)
            {
                if (ownedCount == ammoThresholds[i])
                {
                    p2CanShoot = true;
                    return;
                }
            }
        }
    }

    private int CountPiecesOnOwnedSide(Player player)
    {
        int count = 0;

        if (player == Player.Player1)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < Mathf.Floor(columns / 2f); c++)
                {
                    if (board[r, c] == CellState.Player1)
                    {
                        count++;
                    }
                }
            }
        }
        else if (player == Player.Player2)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = Mathf.CeilToInt(columns / 2f); c < columns; c++)
                {
                    if (board[r, c] == CellState.Player2)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    #endregion

    #region Turn Management

    private IEnumerator SwitchTurn()
    {
        Player wasPlayer = currentPlayer;
        currentPlayer = Player.None;
        boardButtonsManager.ToggleRowSelectors(false);
        yield return new WaitForSeconds(1f);
        currentPlayer = wasPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
        boardButtonsManager.ToggleRowSelectors(true);
    }

    #endregion

    private bool CheckWinCondition(CellState player)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (board[r, c] == player)
                {
                    // Check horizontal
                    if (c + 3 < columns &&
                        board[r, c + 1] == player &&
                        board[r, c + 2] == player &&
                        board[r, c + 3] == player)
                    {
                        return true;
                    }

                    // Check vertical
                    if (r + 3 < rows &&
                        board[r + 1, c] == player &&
                        board[r + 2, c] == player &&
                        board[r + 3, c] == player)
                    {
                        return true;
                    }

                    // Check diagonal (bottom-left to top-right)
                    if (r + 3 < rows && c + 3 < columns &&
                        board[r + 1, c + 1] == player &&
                        board[r + 2, c + 2] == player &&
                        board[r + 3, c + 3] == player)
                    {
                        return true;
                    }

                    // Check diagonal (top-left to bottom-right)
                    if (r - 3 >= 0 && c + 3 < columns &&
                        board[r - 1, c + 1] == player &&
                        board[r - 2, c + 2] == player &&
                        board[r - 3, c + 3] == player)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }
}