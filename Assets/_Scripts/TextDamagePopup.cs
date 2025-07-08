using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextDamagePopup : MonoBehaviour
{
    private TextMeshPro text;
    private Coroutine hideCoroutine;
    private Sequence hideSequence;
    private void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshPro>();
    }
    private void OnEnable()
    {
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HidePopup());
    }

    private void OnDisable()
    {
        if (hideSequence != null) hideSequence.Kill();
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
    }
    public void SetTakeDamageData(int textValue, Color color)
    {
        text.text = $"-{textValue}";
        text.color = color;
        
        float randomX = Random.Range(-0.5f, 0.5f);
        float randomY = Random.Range(-0.5f, 0.5f);
        Vector3 randomOffset = new Vector3(randomX, randomY, 0f);
        transform.position += randomOffset;
    }

    private IEnumerator HidePopup()
    {
        text.alpha = 0f;
        yield return new WaitForSeconds(0.1f);
        hideSequence = DOTween.Sequence();
        hideSequence.Append(text.DOFade(1f, 0.5f));
        hideSequence.Join(transform.DOMoveY(transform.position.y + 3f, 1f));
        hideSequence.Append(text.DOFade(0f, 0.5f));
        hideSequence.OnComplete(() =>
        {
            PoolManager.Instance.ReturnObject(PoolType.TextDamagePopup, gameObject);
        });
    }
}
