using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapPlayer : MonoBehaviour
{
    [SerializeField] private CanvasGroup visualsGroup;
    [SerializeField] private Slider healthSlider;

    // ---------- public methods

    public void UpdateHealth(float value)
    {
        healthSlider.value = value;
    }

    public void OnMove(float normalizedPosition, int line)
    {
        if (GameManager.Instance.minimapEvaluator != null)
            GameManager.Instance.minimapEvaluator.OnMove(transform, normalizedPosition, line);
    }

    public void Hide()
    {
        visualsGroup.alpha = 0f;
    }
}
