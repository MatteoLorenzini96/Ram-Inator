using UnityEngine;
using UnityEngine.EventSystems;

public class SwipePanelHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private LevelButtonSpawner levelManager;


   void Start()
    {
        levelManager = FindFirstObjectByType<LevelButtonSpawner>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Registra la posizione iniziale del tocco
        startTouchPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Puoi usare questa funzione se desideri fornire un feedback visivo durante il drag
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Registra la posizione finale del tocco
        endTouchPosition = eventData.position;

        // Calcola la direzione dello swipe
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        float swipeDistanceX = endTouchPosition.x - startTouchPosition.x;
        float swipeDistanceY = endTouchPosition.y - startTouchPosition.y;

        // Controlla se lo swipe è principalmente orizzontale
        if (Mathf.Abs(swipeDistanceX) > Mathf.Abs(swipeDistanceY))
        {
            if (swipeDistanceX > 0)
            {
                OnSwipeRight();
            }
            else
            {
                OnSwipeLeft();
            }
        }
    }

    private void OnSwipeRight()
    {
        //Debug.Log("Swipe verso destra!");
        levelManager.PreviousPage();
    }

    private void OnSwipeLeft()
    {
        //Debug.Log("Swipe verso sinistra!");
        levelManager.NextPage();
    }
}
