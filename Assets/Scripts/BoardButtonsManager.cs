using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardButtonsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject[] rowSelectors;

    [SerializeField] private GameObject[] redPieces;
    [SerializeField] private GameObject[] bluePieces;

    private List<Button> rowButtons;
    private List<Piece> pieces;

    private int redPieceIndex;
    private int bluePieceIndex;

    public int lastPieceRow;

    private void Start()
    {
        rowButtons = new List<Button>();
        for (int i = 0; i < rowSelectors.Length; i++)
        {
            Button button = rowSelectors[i].GetComponent<Button>();
            rowButtons.Add(button);
        }

        pieces = new List<Piece>();
        for (int i = 0; i < redPieces.Length; i++)
        {
            Piece piece = redPieces[i].GetComponent<Piece>();
            piece.SetBoardManager(boardManager);
            pieces.Add(piece);
        }
        for (int i = 0; i < bluePieces.Length; i++)
        {
            Piece piece = bluePieces[i].GetComponent<Piece>();
            piece.SetBoardManager(boardManager);
            pieces.Add(piece);
        }
    }

    public void ToggleRowSelectors(bool isActive)
    {
        for (int i = 0; i < rowButtons.Count; i++)
            rowButtons[i].interactable = isActive;

        if (isActive)
            EventSystem.current.SetSelectedGameObject(rowSelectors[0]);
    }

    public void TryPlacePiece(int column)
    {
        BoardManager.Player currentPlayer = boardManager.GetCurrentPlayer();
        if (!boardManager.PlacePiece(column))
            RowSelectorError(column);
        else
            EnablePiece(column, currentPlayer);
    }

    public void SetPieceLocation(int column, Vector3 position)
    {
        rowSelectors[column].transform.position = position;
    }

    public void Aiming(bool isAiming)
    {
        if (isAiming)
        {
            ToggleRowSelectors(false);
            ShowPieceTargets();
            SetFirstTargetInteractable();
        }
        else
        {
            ToggleRowSelectors(true);
            HidePieceTargets();
        }
    }

    private void ShowPieceTargets()
    {
        for (int i = 0; i < redPieces.Length; i++)
            redPieces[i].GetComponent<Piece>().ShowTarget();
        for (int i = 0; i < bluePieces.Length; i++)
            bluePieces[i].GetComponent<Piece>().ShowTarget();
    }

    private void HidePieceTargets()
    {
        for (int i = 0; i < redPieces.Length; i++)
            redPieces[i].GetComponent<Piece>().HideTarget();
        for (int i = 0; i < bluePieces.Length; i++)
            bluePieces[i].GetComponent<Piece>().HideTarget();
    }

    private void SetFirstTargetInteractable()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].gameObject.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(pieces[i].transform.GetChild(0).gameObject);
                Debug.Log("First target set to " + i);
                break;
            }
        }
    }

    private void RowSelectorError(int column)
    {
        rowSelectors[column].GetComponent<Animator>().SetTrigger("Error");
    }

    private void EnablePiece(int column, BoardManager.Player player)
    {
        if (player == BoardManager.Player.Player1)
        {
            if (redPieceIndex >= redPieces.Length)
                redPieceIndex = 0;
            redPieces[redPieceIndex].SetActive(true);
            redPieces[redPieceIndex].transform.position = rowSelectors[column].transform.position + Vector3.up * 2;
            redPieces[redPieceIndex].GetComponent<Piece>().Initialize(lastPieceRow, column);
            redPieceIndex++;
        }
        else if (player == BoardManager.Player.Player2)
        {
            if (bluePieceIndex >= bluePieces.Length)
                bluePieceIndex = 0;
            bluePieces[bluePieceIndex].SetActive(true);
            bluePieces[bluePieceIndex].transform.position = rowSelectors[column].transform.position + Vector3.up * 2;
            bluePieces[bluePieceIndex].GetComponent<Piece>().Initialize(lastPieceRow, column);
            bluePieceIndex++;
        }
    }
}
