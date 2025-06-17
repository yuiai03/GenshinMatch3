using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private Slider _hPSlider;
    private TextMeshProUGUI _hPText;

    private Coroutine _setHPCoroutine;

    private void Awake()
    {
        if (!_hPSlider) _hPSlider = GetComponent<Slider>();
        if (!_hPText) _hPText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetMaxHP(float value)
    {
        _hPSlider.maxValue = value;
        _hPSlider.value = value;

        SetHPText(value);
    }

    public void SetHP(float value)
    {
        if (_setHPCoroutine != null) StopCoroutine(_setHPCoroutine);
        _setHPCoroutine = StartCoroutine(SetHPCoroutine(_hPSlider, value));
    }
    private IEnumerator SetHPCoroutine(Slider slider, float targetValue)
    {
        float startValue = slider.value;
        float timeCount = 0f;
        float duration = 0.5f; 

        while (timeCount < duration)
        {
            timeCount += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetValue, timeCount / duration);
            SetHPText(slider.value);
            yield return null;
        }

        slider.value = targetValue;
    }

    private void SetHPText(float value)
    {
        if (_hPText) _hPText.text = $"{(int)value}/{_hPSlider.maxValue}";
    }
}
