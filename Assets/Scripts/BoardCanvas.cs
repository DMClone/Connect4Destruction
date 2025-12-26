using TMPro;
using UnityEngine;

public class BoardCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentPlayerText;
    [SerializeField] private TextMeshProUGUI p1AmmoText;
    [SerializeField] private TextMeshProUGUI p2AmmoText;

    void Start()
    {
        currentPlayerText.text = "Current Player: Player 1";
        p1AmmoText.text = "Player 1 cannot shoot";
        p2AmmoText.text = "Player 2 cannot shoot";
    }

    public void UpdateCurrentPlayerText(string player)
    {
        currentPlayerText.text = $"Current Player: {player}";
    }

    public void UpdatePlayerOneAmmoText(string ammoStatus)
    {
        p1AmmoText.text = ammoStatus;
    }

    public void UpdatePlayerTwoAmmoText(string ammoStatus)
    {
        p2AmmoText.text = ammoStatus;
    }
}
