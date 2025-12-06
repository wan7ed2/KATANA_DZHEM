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
            boxCollider.offset = size / 2f;
        }
    }
}
