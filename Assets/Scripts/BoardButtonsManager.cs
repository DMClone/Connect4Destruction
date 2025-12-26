using UnityEngine;

public class BoardButtonsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] rowSelectors;

    public void ToggleRowSelectors(bool isActive)
    {
        for (int i = 0; i < rowSelectors.Length; i++)
            rowSelectors[i].SetActive(isActive);
    }
}
