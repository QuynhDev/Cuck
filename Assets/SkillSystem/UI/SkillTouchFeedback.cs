using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Effects
{
    [RequireComponent(typeof(RectTransform))]
    public class SkillTouchFeedback : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private float punchScale = 0.25f;
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private int vibrato = 8;
        [SerializeField] private float elasticity = 0.9f;

        public void OnPointerDown(PointerEventData eventData)
        {
            PopUpEffect.Play(transform, punchScale, duration, vibrato, elasticity);
        }
    }
}
