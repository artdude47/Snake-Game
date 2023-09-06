using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ButtonTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float scaleMultiplier = 1.1f;
    [SerializeField]
    private float animationDuration = 0.2f;

    private Vector3 initialScale;

    private void Start()
    {
        initialScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(initialScale * scaleMultiplier, animationDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(initialScale, animationDuration);
    }
}
