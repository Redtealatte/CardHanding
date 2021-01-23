using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Card card;
    private Transform handTf, canvasTf;

    /// <summary>
    /// 카드가 드랍존 위에  있는지.
    /// </summary>
    public bool isOnDropZone;

    /// <summary>
    /// 실제로 드래그가 되었는지.
    /// </summary>
    public bool mousePointFollow;    
    public bool avoidOverlap;
    private bool mouseOver;//중복실행 방지
    private float yPos2BeExtended;//확장된 카드가 세워질 고정 y 좌표.
    

    void Start()
    {
        canvasTf = GameObject.Find("Canvas").GetComponent<Transform>();
        handTf = canvasTf.Find("Hand").GetComponent<Transform>();
        card = GetComponent<Card>();

        //카드의 scale을 변경했을 때, 해상도에 따라 카드가 놓일 Y좌표를 계산.
        float cardHalfY = card.transform.GetComponent<RectTransform>().rect.height * (HandingManager.Instance.cardMaxSize + 0.5f) / 2f;
        float canvasHalfY = canvasTf.GetComponent<RectTransform>().rect.height / 2f;
        yPos2BeExtended = cardHalfY - canvasHalfY;

        isOnDropZone = mousePointFollow = avoidOverlap = false;
    }

    void Update()
    {
        //카드가 드래그 되었을 때, 마우스 포인터를 천천히 따라다니도록 효과를 줌.
        if (mousePointFollow)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);            
            pos.z = 0f;
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (HandingManager.Instance.endDraw)
        {
            transform.SetParent(canvasTf);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            mousePointFollow = HandingManager.Instance.dragCard = avoidOverlap = true;
        }        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //카드의 이동을 OnDrag에 넣었을때 마우스 포인터를 천천히 따라가게 하면 끊기며 이동하기 때문에 이동은 Update에서 처리한다. 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (HandingManager.Instance.endDraw && mousePointFollow)
        {
            mousePointFollow = false;
            transform.SetParent(handTf);
            transform.SetSiblingIndex(card.order);//히에라키에서 순서 변경.
            if (!isOnDropZone)
            {
                ChangeTransform(ObjectControl.RotationAngle(gameObject, card.angle), HandingManager.Instance.cardMaxSize, card.targetPos);
                HandingManager.Instance.RollBackGapCards(card.order);
                StartCoroutine(Timer(0.1f));
            }
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            HandingManager.Instance.dragCard = false;
        }
    }

    //마우스를 올렸을때 카드 회전, 확대
    void OnMouseOver()
    {        
        if (!isOnDropZone && card.isDraggable && !mousePointFollow && HandingManager.Instance.endDraw && !HandingManager.Instance.dragCard && !mouseOver && !avoidOverlap)
        {
            mouseOver = true;
            transform.SetAsLastSibling();
            ChangeTransform(ObjectControl.RotationAngle(gameObject, 0f), HandingManager.Instance.cardMaxSize + 0.5f, new Vector3(transform.localPosition.x, yPos2BeExtended, -1f));
            HandingManager.Instance.ExpandGapSelectedCard(card.order);
        }
    }

    //마우스를 치웠을때 카드 회전, 축소
    void OnMouseExit()
    {
        if (!isOnDropZone && card.isDraggable && !mousePointFollow && HandingManager.Instance.endDraw && !HandingManager.Instance.dragCard && !avoidOverlap)
        {
            transform.SetSiblingIndex(GetComponent<Card>().order);
            ChangeTransform(ObjectControl.RotationAngle(gameObject, card.angle), HandingManager.Instance.cardMaxSize, card.targetPos);
            HandingManager.Instance.RollBackGapCards(card.order);                      
        }
        mouseOver = false;
    }

    //카드의 크기, 회전, 이동을 즉시 변경.
    void ChangeTransform(float varAngle, float varScale, Vector3 varPos)
    {
        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + varAngle);
        transform.localScale = new Vector3(varScale, varScale, 0f);
        transform.localPosition = varPos;
    }

    //drag를 종료했을때, OnMouseOver과 OnMouseExit가 실행되는걸 막기위해 타이머를 추가.
    IEnumerator Timer(float time)
    {
        float curTime = 0f;
        while (curTime < time)
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        avoidOverlap = false;
    }
}
