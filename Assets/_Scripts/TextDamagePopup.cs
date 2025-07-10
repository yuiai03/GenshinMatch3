using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextDamagePopup : MonoBehaviour
{
    private TextMeshPro text;
    private Coroutine hideCoroutine;
    private Sequence hideSequence;
    private float distance = 1f;
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
    public void SetTakeDamageData(Color color, ElementalReactionData data)
    {
        text.text = $"-{data.damage} {Helper.ReactionText(data.reactionType)}";
        text.color = color;
        
        float randomX = Random.Range(-distance, distance);
        float randomY = Random.Range(-distance, distance);
        Vector3 randomOffset = new Vector2(randomX, randomY);
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
