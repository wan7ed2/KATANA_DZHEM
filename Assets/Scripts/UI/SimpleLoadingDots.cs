using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SimpleLoadingDots : MonoBehaviour
{
    [SerializeField] private float _dotInterval = 0.5f;

    private WaitForSeconds _waitForSeconds;

    private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _waitForSeconds = new WaitForSeconds(_dotInterval);
    }

    private void OnEnable() => StartCoroutine(AnimationCoroutine());

    private void OnDisable() => StopAllCoroutines();

    private IEnumerator AnimationCoroutine()
    {
        while (true)
        {
            _textMeshPro.text = ".";
            yield return new WaitForSeconds(_dotInterval);
            _textMeshPro.text = "..";
            yield return new WaitForSeconds(_dotInterval);
            _textMeshPro.text = "...";
            yield return new WaitForSeconds(_dotInterval);
            _textMeshPro.text = "";
            yield return new WaitForSeconds(_dotInterval);
        }
    }
}