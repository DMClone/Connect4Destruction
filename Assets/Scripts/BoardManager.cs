using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private BoardButtonsManager boardButtonsManager;
    [SerializeField] private BoardCanvas boardCanvas;

    [Header("Settings")]
    [SerializeField] private int[] ammoThresholds = new int[2] { 3, 6 };

    private bool isAiming;

    private Player currentPlayer;
    private Player lastShooter;

    private int rows = 6;
    private int columns = 7;

    private CellState[,] board = new CellState[6, 7];

    private bool p1CanShoot;
    private bool p2CanShoot;

    private int p1AmmoGained;
    private int p2AmmoGained;

    private void OnEnable()
    {
        InputAction aimAction = inputActions.FindAction("Aim");
        aimAction.performed += Aim;
        aimAction.Enable();
    }

    private void OnDisable()
    {
        InputAction aimAction = inputActions.FindAction("Aim");
        aimAction.performed -= Aim;
        aimAction.Disable();
    }

    private void Start()
    {
        isAiming = false;

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

        if (CheckWinCondition(board[row, column]))
        {
            PlayerWin(currentPlayer);
            return true;
        }
        boardButtonsManager.lastPieceRow = row;
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

    private void Aim(InputAction.CallbackContext context)
    {
        Debug.Log("Aim input received");
        if (context.performed && isAiming)
        {
            isAiming = false;
            boardButtonsManager.Aiming(false);
        }

        else if (context.performed && !isAiming &&
            ((currentPlayer == Player.Player1 && p1CanShoot) ||
             (currentPlayer == Player.Player2 && p2CanShoot)))
        {
            isAiming = true;
            boardButtonsManager.Aiming(true);
        }
    }

    public bool ShootPiece(int row, int column)
    {
        if ((currentPlayer == Player.Player1 && !p1CanShoot) ||
            (currentPlayer == Player.Player2 && !p2CanShoot))
        {
            return false; // Cannot shoot
        }

        if (board[row, column] == CellState.Empty)
        {
            return false; // No piece to shoot
        }

        // Remove the piece
        board[row, column] = CellState.Empty;
        lastShooter = currentPlayer;

        // Update ammo status
        if (currentPlayer == Player.Player1)
        {
            p1CanShoot = false;
            boardCanvas.UpdatePlayerOneAmmoText("Player 1 cannot shoot");
        }
        else if (currentPlayer == Player.Player2)
        {
            p2CanShoot = false;
            boardCanvas.UpdatePlayerTwoAmmoText("Player 2 cannot shoot");
        }

        PieceGravity(row, column);

        if (CheckWinCondition(currentPlayer == Player.Player1 ? CellState.Player2 : CellState.Player1))
        {
            PlayerWin(currentPlayer == Player.Player1 ? Player.Player2 : Player.Player1);
            return true;
        }
        else if (CheckWinCondition(board[row, column]))
        {
            PlayerWin(currentPlayer);
            return true;
        }




        StartCoroutine(SwitchTurn());
        return true;
    }

    private void PieceGravity(int row, int column)
    {
        for (int r = row; r < rows - 1; r++)
        {
            if (board[r + 1, column] == CellState.Empty)
            {
                board[r + 1, column] = board[r, column];
                board[r, column] = CellState.Empty;
            }
            else
            {
                break;
            }
        }
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
                    p1AmmoGained++;
                    boardCanvas.UpdatePlayerOneAmmoText("Player 1 can shoot");
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
                    p2AmmoGained++;
                    boardCanvas.UpdatePlayerTwoAmmoText("Player 2 can shoot");
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
        boardCanvas.UpdateCurrentPlayerText(currentPlayer == Player.Player1 ? "Player 1" : "Player 2");
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

    private void PlayerWin(Player player)
    {
        UnityEngine.Debug.Log($"{player} wins!");
        // Additional win handling logic can be added here
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }
}