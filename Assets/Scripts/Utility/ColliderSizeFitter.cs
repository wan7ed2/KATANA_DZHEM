using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class ColliderSizeFitter : MonoBehaviour
{
    [ContextMenu("Fit")]
    private void Fit()
    {
        var colldier = GetComponent<BoxCollider2D>();    
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.drawMode == SpriteDrawMode.Simple)
            return;

        colldier.size = spriteRenderer.size;
    }
}
