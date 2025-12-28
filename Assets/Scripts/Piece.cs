using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private BoardManager boardManager;
    private int row;
    private int column;

    // Sets the reference to the BoardManager from the BoardButtonsManager
    public void SetBoardManager(BoardManager bm)
    {
        boardManager = bm;
    }

    public void Initialize(int r, int c)
    {
        row = r;
        column = c;
    }

    // Called when the piece is shot by the TargetAiming system
    public void PieceShot()
    {
        if (!boardManager.BreakPiece(row, column))
            return;

        gameObject.SetActive(false);
    }

    public void ShowTarget()
    {
        spriteRenderer.enabled = true;
    }

    public void HideTarget()
    {
        spriteRenderer.enabled = false;
    }
}
