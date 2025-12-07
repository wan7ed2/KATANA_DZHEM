using UnityEngine;

public class AnimatorComponentsProvider : MonoBehaviour
{
    [SerializeField] private Animator goodAnimator;
    [SerializeField] private Animator badAnimator;
    [SerializeField] private SpriteRenderer goodSpriteRenderer;
    [SerializeField] private SpriteRenderer badSpriteRenderer;

    public Animator Animator { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        ChangeAnimator(false);
    }

    public void ChangeAnimator(bool isBad)
    {
        Debug.Log($"Changed animator. Is bad: {isBad}");
        Animator = isBad ? badAnimator : goodAnimator;
        SpriteRenderer = isBad ? badSpriteRenderer : goodSpriteRenderer;
        badAnimator.gameObject.SetActive(isBad);
        goodAnimator.gameObject.SetActive(!isBad);

    }
}