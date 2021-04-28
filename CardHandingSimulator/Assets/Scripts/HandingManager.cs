using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandingManager : MonoBehaviour
{    
    private static HandingManager instance = null;//카드의 조작을 담당하는 매니저이므로 싱글톤으로 작성.

    //- Waypoint : Draw
    private Deck deck;
    private Transform drawP1, drawP2;

    //- Waypoint : Hand
    private Transform handP0, handP1, handP2, handP3;

    //- Waypoint : Drop
    private DiscardPile discardPile;
    private Transform dropP1, dropP2;

    private Transform reloadPileTf;//버린 카드를 다시 덱으로 되돌릴 때, 핸드의 카드보다 뒤에 랜더링되기 위해 부모로 지정한다.
    private List<Card> cardList;

    [SerializeField]
    private GameObject cardPrefab = null;
    //카드의 스케일을 결정.
    public float cardMaxSize;
    public float cardMinSize;
    //드로우한 카드를 붙일 오브젝트의 Transform.
    private Transform hand;

    public float Be2CardAngle;//손패에 있는 카드들의 사이각.
    public int maxCardCount = 10;
    public int drawableCount;//드로우할 카드의 수.

    /// <summary>
    /// 드로우 종료를 확인한다.
    /// </summary>
    public bool endDraw;
    private int endDrawCount;

    /// <summary>
    /// 카드를 drag 중일때 다른 카드는 drag되지 못해야 한다.
    /// </summary>
    public bool dragCard = false;
    private bool isDrawable;//드로우 도중 Re-draw 버튼 중복 클릭 제한.    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public static HandingManager Instance
    {
        get
        {
            if (instance == null)
                return null;
            return instance;
        }
    }

    void Start()
    {
        deck = GameObject.Find("Canvas").transform.Find("Deck").GetComponent<Deck>();
        drawP1 = GameObject.Find("Canvas").transform.Find("CurvePosition").transform.Find("Draw1").GetComponent<Transform>();
        drawP2 = GameObject.Find("Canvas").transform.Find("CurvePosition").transform.Find("Draw2").GetComponent<Transform>();

        handP0 = GameObject.Find("Canvas").transform.Find("CurvePosition").transform.Find("P0").GetComponent<Transform>();
        handP1 = GameObject.Find("Canvas").transform.Find("CurvePosition").transform.Find("P1").GetComponent<Transform>();
        handP2 = GameObject.Find("Canvas").transform.Find("CurvePosition").transform.Find("P2").GetComponent<Transform>();
        handP3 = GameObject.Find("Canvas").transform.Find("CurvePosition").transform.Find("P3").GetComponent<Transform>();

        discardPile = GameObject.Find("Canvas").transform.Find("DiscardPile").GetComponent<DiscardPile>();
        dropP1 = GameObject.Find("Canvas").transform.Find("CurvePosition").transform.Find("Drop1").GetComponent<Transform>();
        dropP2 = GameObject.Find("Canvas").transform.Find("CurvePosition").transform.Find("Drop2").GetComponent<Transform>();

        reloadPileTf = GameObject.Find("Canvas").transform.Find("ReloadPile").GetComponent<Transform>();
        hand = GameObject.Find("Canvas").transform.Find("Hand").GetComponent<Transform>();

        cardList = new List<Card>();
        isDrawable = true;
    }

    //드로우하는 과정. 
    //뽑을 카드 더미에 카드가 존재하면, 프리팹을 생성 -> cardList에 추가 -> 손패로 이동 -> 손패 정렬.
    //뽑을 카드 더미에 카드가 없다면, 버린 카드 더미에서 카드를 가져와 뽑을 카드 더미에 넣고, 뽑을 카드 더미를 섞는다.
    private IEnumerator DrawRoutineC()
    {
        endDrawCount = 0;
        endDraw = false;

        for (int i = 0; i < drawableCount; i++)
        {
            if (deck.pile.Count == 0)
            {
                yield return StartCoroutine(ReloadAllCardC());
            }
            Card card = GeneratedCard(deck.transform.localPosition, Vector3.zero, new Vector3(0f, 0f, 180f), hand);
            card.SetInitializeData(deck.TakeOutOfPile(deck.pile.Count - 1));
            cardList.Add(card);
            cardList[i].order = i;
            StartCoroutine(DrawCardC(0.6f, cardList[i].order));

            yield return new WaitForSeconds(0.5f);
        }
        yield return StartCoroutine(CheckEndDrawC());
        isDrawable = true;
        //각 카드들의 드래그 허용.
        for (int i = 0; i < drawableCount; i++)
            cardList[i].isDraggable = true;
    }

    //모든 드로우가 종료됬는지 확인한다.
    private IEnumerator CheckEndDrawC()
    {
        while (endDrawCount < drawableCount)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        endDraw = true;
    }

    //다른 오브젝트의 자식으로 카드를 생성해서 초기정보 부여.
    private Card GeneratedCard(Vector3 pos, Vector3 scale, Vector3 rot, Transform parent)
    {
        GameObject obj = Instantiate(cardPrefab);
        Card temp = obj.GetComponent<Card>();
        temp.transform.SetParent(parent);
        temp.transform.localPosition = pos;
        temp.transform.localScale = scale;
        temp.transform.localRotation = Quaternion.Euler(rot);
        return temp;
    }

    //생성된 카드를 베지어 곡선에 따라 이동시킨다.
    private IEnumerator DrawCardC(float time, int index)
    {
        StartCoroutine(ObjectControl.RotationToC(0.3f, new Vector3(0f, 0f, -180f), cardList[index].gameObject));
        SetCurveRate(index);
        cardList[index].targetPos = Curve.BezierCurve(cardList[index].handCurveRate, handP0.localPosition, handP1.localPosition, handP2.localPosition, handP3.localPosition);

        StartCoroutine(ObjectControl.ChangeSizeC(0.3f, new Vector3(cardMaxSize, cardMaxSize, 0.0f), cardList[index].gameObject));
        yield return StartCoroutine(ObjectControl.CurveMoveObjC(time, deck.transform.localPosition, drawP1.localPosition, drawP2.localPosition, cardList[index].targetPos, cardList[index].gameObject));

        StartCoroutine(cardList[index].SetActiveOfTrailC(0.3f, false));

        //손패 정렬 실행.        
        SortAllCard();
        endDrawCount++;
    }

    //cardList의 모든 카드를 각각의 targetPos로 이동시키고 angle만큼 회전시킨다.
    public void SortAllCard()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            SetAngleToCard(i);
            SetCurveRate(i);
            SortCard(0.3f, i);
            StartCoroutine(ObjectControl.RotationToC(0.1f, new Vector3(0f, 0f, ObjectControl.RotationAngle(cardList[i].gameObject, cardList[i].angle)), cardList[i].gameObject));
        }
    }

    //time 시간동안 손패 내의 카드를 일정한 속도로 targetPos까지 이동시킨다.
    private void SortCard(float time, int index)
    {
        Vector3 start = cardList[index].transform.localPosition;
        cardList[index].targetPos = Curve.BezierCurve(cardList[index].handCurveRate, handP0.localPosition, handP1.localPosition, handP2.localPosition, handP3.localPosition);
        StartCoroutine(ObjectControl.MoveObjC(time, start, cardList[index].targetPos, cardList[index].gameObject));
    }

    //카드의 rate(베지어 곡선상의 위치)를 정해준다.
    //cardList의 크기가 6미만이면 0.13f, 6이상이면 0.1f만큼 베지어곡선의 카드 사이 간격 비율을 정한다. 
    private void SetCurveRate(int index)
    {        
        float interval = (cardList.Count < 6) ? 0.13f : 0.1f;
        
        if (cardList.Count % 2 == 0)
        {
            int i = index - cardList.Count / 2;
            cardList[index].handCurveRate = (float)i * interval + 0.55f;
        }
        else
        {
            int i = index - cardList.Count / 2;
            cardList[index].handCurveRate = (float)i * interval + 0.5f;
        }                
    }

    //손패의 카드들이 얼마나 기울어 질것인가를 계산한다.
    //카드마다 Be2CardAngle만큼 회전하도록 card의 angle을 정한다.
    private void SetAngleToCard(int index)
    {
        int center = cardList.Count / 2;

        if (cardList.Count % 2 == 1)
            cardList[index].angle = (center - index) * Be2CardAngle;
        else
        {
            if (index < center)
                cardList[index].angle = (float)(center - index - 1) * Be2CardAngle + Be2CardAngle / 2f;
            else
                cardList[index].angle = (float)(center - index) * Be2CardAngle - Be2CardAngle / 2;
        }           
    }

    /// <summary>
    /// time 시간동안 cardList의 index에 해당하는 카드를 현재 위치에서 discardPile의 위치로 베지어 곡선을 따라 이동시킨다.
    /// </summary>
    public void DropCard(float time, int index)
    {
        Card card = cardList[index];
        cardList.RemoveAt(index);

        for (int i = card.order; i < cardList.Count; i++)
            cardList[i].order--;

        float y = 100f;
        float x = (discardPile.transform.localPosition.x - card.transform.localPosition.x) / 3;

        Vector3 p1 = new Vector3(card.transform.localPosition.x + x, card.transform.localPosition.y + y, 0f);
        Vector3 p2 = new Vector3(card.transform.localPosition.x + x * 2, card.transform.localPosition.y + y, 0f);

        StartCoroutine(ObjectControl.ChangeSizeC(time, new Vector3(cardMinSize, cardMinSize, 0f), card.gameObject));
        StartCoroutine(ObjectControl.RotationToC(time, new Vector3(0f, 0f, ObjectControl.RotationAngle(card.gameObject, -180f)), card.gameObject));
        StartCoroutine(DropCardC(time, card, card.transform.localPosition, p1, p2, discardPile.transform.localPosition));
    }

    //time 시간동안 card를 베지어 곡선을 따라 이동시킨다.
    //카드의 trail효과를 활성화시키고, 이동이 종료되면 card.init을 discardPile에 추가하며 객체를 Destroy한다.
    private IEnumerator DropCardC(float time, Card card, Vector3 start, Vector3 p1, Vector3 p2, Vector3 end)
    {
        card.isDraggable = false;
        StartCoroutine(card.SetActiveOfTrailC(0f, true));
        yield return StartCoroutine(ObjectControl.CurveMoveObjC(time, start, p1, p2, end, card.gameObject));
        discardPile.AddToPile(card.init);
        Destroy(card.gameObject, 0.5f);
    }

    //손패의 모든 카드를 버리고 리스트를 비운 다음 드로우 루틴을 실행.
    private IEnumerator DropAllCardC()
    {
        float time = 0.7f;
        foreach(Card card in cardList)
        {
            StartCoroutine(ObjectControl.ChangeSizeC(time, new Vector3(cardMinSize, cardMinSize, 0f), card.gameObject));
            StartCoroutine(ObjectControl.RotationToC(time, new Vector3(0f, 0f, ObjectControl.RotationAngle(card.gameObject, -180f)), card.gameObject));
            StartCoroutine(DropCardC(time, card, card.transform.localPosition, dropP1.localPosition, dropP2.localPosition, discardPile.transform.localPosition));
        }
        if (cardList.Count > 0)
            yield return new WaitForSeconds(time + 0.5f);
        cardList.Clear();
        StartCoroutine(DrawRoutineC());
    }

    //discardPile의 모든 카드를 가져와 뽑을 카드 더미에 넣고 섞는다.
    private IEnumerator ReloadAllCardC()
    {
        float time = 0.5f;
        float count = 0f;
        while (discardPile.pile.Count > 0)
        {
            count += 1f;
            Card reloadCard = GeneratedCard(discardPile.transform.localPosition, new Vector3(0.2f, 0.2f, 0.0f), Vector3.zero, reloadPileTf);
            reloadCard.init = discardPile.TakeOutOfPile(0);
            StartCoroutine(ReloadCardC(time, reloadCard));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(count * 0.1f + time);
        deck.ShufflePile();
    }

    //time 시간동안 Card를 베지어 곡선에 따라 이동하여, 뽑을 카드 더미에 card.init 데이터를 전달한 후, card.GameObject를 Destroy한다.
    private IEnumerator ReloadCardC(float time, Card card)
    {
        float h = Random.Range(-2, 3) * 80;
        float addH = 3 * 80;
        float x = discardPile.transform.localPosition.x + (deck.transform.localPosition.x - discardPile.transform.localPosition.x) / 5;
        Vector3 r1 = new Vector3(x, discardPile.transform.localPosition.y + addH + h, 0f);
        Vector3 r2 = new Vector3(x, discardPile.transform.localPosition.y + addH, 0f);

        StartCoroutine(ObjectControl.RotationToC(time, new Vector3(0f, 0f, ObjectControl.RotationAngle(card.gameObject, 120f)), card.gameObject));
        yield return StartCoroutine(ObjectControl.CurveMoveObjC(time, discardPile.transform.localPosition, r1, r2, deck.transform.localPosition, card.gameObject));

        deck.AddToPile(card.init);
        Destroy(card.gameObject, 0.5f);
    }

    //카드를 확대시킬 때, index의 카드를 제외한 나머지 카드들을 xGap만큼 이동시켜 확대된 카드에 가려지지 않게 한다.
    public void ExpandGapSelectedCard(int index)
    {
        float xGap = (cardList.Count < 6) ? 30f : 45f;
        
        for (int i = 0; i < cardList.Count; i++)
        {
            if (i != index)
            {                
                float x = (i < index) ? xGap * -1 : xGap;
                cardList[i].transform.localPosition = new Vector3(cardList[i].targetPos.x + x, cardList[i].targetPos.y, 0f);                
            }
        }
    }

    //cardList에서 index에 해당하는 카드를 제외한 나머지 카드
    public void RollBackGapCards(int index)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            if (i != index)
                SortCard(0f, i);
        }
    }

    //UI의 버튼 클릭 이벤트에 사용될, Re-draw 함수이다.
    public void ReDraw()
    {
        if (isDrawable)
        {
            isDrawable = false;
            StartCoroutine(DropAllCardC());
        }
    }
}
