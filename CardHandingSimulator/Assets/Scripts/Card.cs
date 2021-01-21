using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardInit init;

    Text _name;
    Text _description;
    Text _cost;
    Text _power;

    TrailRenderer trailObj;

    public float handCurveRate;
    public float moveCurveRate;
    public float angle;
    public int order;

    public bool isDraggable = false;

    /// <summary>
    /// hand 내의 좌표.
    /// </summary>
    public Vector3 targetPos;
    
    void Awake()
    {  
        moveCurveRate = 0.0f;
        angle = 0.0f;
        trailObj = GetComponent<TrailRenderer>();
    }

    /// <summary>
    /// Card의 CardInit 데이터를 입력받아 초기화한다.
    /// </summary>
    public void SetInitializeData(CardInit init)
    {
        this.init = init;
        //프리팹이 생성될때 기존의 연결이 모두 끊어져서 init을 초기화할 때 같이 초기화했다.
        _name = transform.Find("Name").transform.Find("Text").GetComponent<Text>();
        _description = transform.Find("Description").transform.Find("Text").GetComponent<Text>();
        _cost = transform.Find("Cost").GetComponent<Text>();
        _power = transform.Find("Power").GetComponent<Text>();
        SetTextData();
    }

    public void SetTextData()
    {
        _name.text = init.cardName;
        _description.text = "" + init.cardDescription;
        _cost.text = "" + init.cost.ToString();
        _power.text = "" + init.power.ToString();
    }

    /// <summary>
    /// trail을 time시간 후에 활성/비활성화 한다.
    /// </summary>
    public IEnumerator SetActiveOfTrailC(float time, bool tf)
    {        
        float curtime = 0.0f;
        while (curtime < time)
        {
            curtime += Time.deltaTime;
            yield return null;
        }
        trailObj.Clear();
        trailObj.enabled = tf;        
    }

}
