using UnityEngine;

public class MaterialScroller : MonoBehaviour
{
    private Renderer rend;

    [SerializeField]
    private float xScrollSpeed = 0.0f, yScrollSpeed = 0.0f;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        float offsetX = xScrollSpeed * Time.deltaTime / 100;
        float offsetY = yScrollSpeed * Time.deltaTime / 100;

        float currentOffsetX = rend.material.GetTextureOffset("_MainTex").x;
        float currentOffsetY = rend.material.GetTextureOffset("_MainTex").y;

        rend.material.SetTextureOffset("_MainTex", new Vector2(offsetX + currentOffsetX, offsetY + currentOffsetY));
    }
}
