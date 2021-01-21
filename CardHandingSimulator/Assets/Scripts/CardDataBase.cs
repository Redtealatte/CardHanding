using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public static List<CardInit> cardList = new List<CardInit>();
    
    void Awake()
    {
        cardList.Add(new CardInit(0, "A", 1, 0, "This is A."));
        cardList.Add(new CardInit(1, "B", 1, 1, "This is B."));
        cardList.Add(new CardInit(2, "C", 2, 1, "This is C."));
        cardList.Add(new CardInit(3, "D", 2, 2, "This is D."));
        cardList.Add(new CardInit(4, "E", 3, 3, "This is E."));
    }
}
