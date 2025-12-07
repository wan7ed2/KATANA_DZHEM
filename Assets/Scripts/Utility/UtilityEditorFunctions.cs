using UnityEditor;
using UnityEngine;

public class UtilityEditorFunctions
{
    [MenuItem("Utilities/Fix colliders")]
    static void FixColliders()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            if (!gameObject.TryGetComponent(out BoxCollider2D boxCollider) ||
                !gameObject.TryGetComponent(out SpriteRenderer spriteRenderer) ||
                spriteRenderer.drawMode == SpriteDrawMode.Simple)
                continue;

            var size = spriteRenderer.size;
            boxCollider.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            boxCollider.offset = size / 2f;
            EditorUtility.SetDirty(gameObject);
        }
    }

    [MenuItem("Utilities/Randomize sprites")]
    static void RandomizeSprites()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Sprites/Props/Props");
        if (allSprites.Length == 0)
        {
            Debug.LogError("Sprites was not found");
            return;
        }

        foreach (var gameObject in Selection.gameObjects)
        {
            if (!gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
                continue;

            var sprite = allSprites[Random.Range(0, allSprites.Length)];
            spriteRenderer.sprite = sprite;
            EditorUtility.SetDirty(gameObject);
        }
    }
}
