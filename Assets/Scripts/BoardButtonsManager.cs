using UnityEngine;

public class BoardButtonsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject[] rowSelectors;

    public void ToggleRowSelectors(bool isActive)
    {
        for (int i = 0; i < rowSelectors.Length; i++)
            rowSelectors[i].SetActive(isActive);
    }

    public void TryPlacePiece(int column)
    {
        if (!boardManager.PlacePiece(column))
            RowSelectorError(column);
    }

    private void RowSelectorError(int column)
    {
        rowSelectors[column].GetComponent<Animator>().SetTrigger("Error");
    }
}
