using UnityEngine;
using UnityEngine.EventSystems;

public class JumpTouchArea : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private CatController catController;

    public void OnPointerDown(PointerEventData eventData)
    {
        catController.Jump();
    }
}