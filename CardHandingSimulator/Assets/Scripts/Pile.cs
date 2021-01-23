using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pile : MonoBehaviour
{
    public List<CardInit> pile;

    protected Text pileCount;

    /// <summary>
    /// cardInit의 데이터를 pile List에 추가하고, List 원소의 개수를 pileCount에 표시한다.
    /// </summary>
    public void AddToPile(CardInit cardInit)
    {
        CardInit t = new CardInit();
        t = cardInit;
        pile.Add(t);
        pileCount.text = pile.Count.ToString();
    }

    /// <summary>
    /// cardInit의 데이터를 pile List에서 제거하고, List 원소의 개수를 pileCount에 표시한다.
    /// </summary>
    public CardInit TakeOutOfPile(int index)
    {
        CardInit temp = pile[index];
        pile.RemoveAt(index);
        pileCount.text = pile.Count.ToString();
        return temp;
    }
}
