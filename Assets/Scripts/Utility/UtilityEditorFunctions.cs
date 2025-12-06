using UnityEditor;
using UnityEngine;

public class UtilityEditorFunctions
{
    [MenuItem("Utilities/Fix colliders")]
    static void DoSomething()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            if (!gameObject.TryGetComponent(out BoxCollider2D boxCollider) ||
                !gameObject.TryGetComponent(out SpriteRenderer spriteRenderer) ||
                spriteRenderer.drawMode == SpriteDrawMode.Simple)
                continue;

            var size = spriteRenderer.size;
            boxCollider.size = spriteRenderer.size;
            boxCollider.offset = new Vector2(Mathf.Abs(size.x / 2f), Mathf.Abs(size.y / 2f));
        }
    }
}
