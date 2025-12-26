using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardButtonsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject[] rowSelectors;

    [SerializeField] private GameObject[] redPiecePrefabs;
    [SerializeField] private GameObject[] bluePiecePrefabs;

    private List<Button> rowButtons;

    private int redPieceIndex;
    private int bluePieceIndex;

    private void Start()
    {
        rowButtons = new List<Button>();
        for (int i = 0; i < rowSelectors.Length; i++)
        {
            Button button = rowSelectors[i].GetComponent<Button>();
            rowButtons.Add(button);
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

    private void RowSelectorError(int column)
    {
        rowSelectors[column].GetComponent<Animator>().SetTrigger("Error");
    }

    private void EnablePiece(int column, BoardManager.Player player)
    {
        if (player == BoardManager.Player.Player1)
        {
            if (redPieceIndex >= redPiecePrefabs.Length)
                redPieceIndex = 0;
            redPiecePrefabs[redPieceIndex].SetActive(true);
            redPiecePrefabs[redPieceIndex].transform.position = rowSelectors[column].transform.position + Vector3.up * 2;
            redPieceIndex++;
        }
        else if (player == BoardManager.Player.Player2)
        {
            if (bluePieceIndex >= bluePiecePrefabs.Length)
                bluePieceIndex = 0;
            bluePiecePrefabs[bluePieceIndex].SetActive(true);
            bluePiecePrefabs[bluePieceIndex].transform.position = rowSelectors[column].transform.position + Vector3.up * 2;
            bluePieceIndex++;
        }
    }
}
