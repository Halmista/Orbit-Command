using UnityEngine;

public class HexAutoGrid : MonoBehaviour
{
    public GameObject hexPrefab;

    public float hexWidth = 120f;
    public float hexHeight = 104f;

    [Header("Spacing Adjustments")]
    public float horizontalAdjust = 1.1f;
    public float verticalAdjust = 0.65f;

    void Start()
    {
        RectTransform container = GetComponent<RectTransform>();

        float width = container.rect.width;
        float height = container.rect.height;

        float horizontalSpacing = hexWidth * horizontalAdjust;
        float verticalSpacing = hexHeight * verticalAdjust;

        int columns = Mathf.CeilToInt(width / horizontalSpacing) + 2;
        int rows = Mathf.CeilToInt(height / verticalSpacing) + 2;

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                GameObject hex = Instantiate(hexPrefab, transform);
                RectTransform rect = hex.GetComponent<RectTransform>();

                float x = col * horizontalSpacing;
                float y = -row * verticalSpacing;

                if (col % 2 == 1)
                    y -= verticalSpacing / 2f;

                rect.anchoredPosition = new Vector2(x, y);
            }
        }
    }
}