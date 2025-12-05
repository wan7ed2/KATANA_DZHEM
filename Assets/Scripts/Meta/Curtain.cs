using UnityEngine;

public class Curtain : MonoBehaviour
{
    public void Show() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);
}
