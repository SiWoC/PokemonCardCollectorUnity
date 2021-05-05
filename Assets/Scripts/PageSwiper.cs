using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public UnityEvent PageChangedEvent;

    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    public int totalPages = 1;
    public int currentPage = 1;

    private Vector3 panelLocation;

    // Start is called before the first frame update
    void Start(){
        panelLocation = transform.position;
    }

    public void OnDrag(PointerEventData eventData){
        float difference = eventData.pressPosition.x - eventData.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
    }

    public void OnEndDrag(PointerEventData eventData){
        float percentage = (eventData.pressPosition.x - eventData.position.x) / Screen.width;
        if(Mathf.Abs(percentage) >= percentThreshold){
            Vector3 newLocation = panelLocation;
            if(percentage > 0 && currentPage < totalPages){
                currentPage++;
                PageChangedEvent?.Invoke();
                newLocation += new Vector3(-Screen.width, 0, 0);
            }else if(percentage < 0 && currentPage > 1){
                currentPage--;
                PageChangedEvent?.Invoke();
                newLocation += new Vector3(Screen.width, 0, 0);
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }else{
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
        }
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds){
        float t = 0f;
        while(t <= 1.0){
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    public void BackToPage1()
    {
        currentPage = 1;
        PageChangedEvent?.Invoke();
        transform.position += new Vector3(-transform.position.x, 0, 0);
        panelLocation = transform.position;
    }
}