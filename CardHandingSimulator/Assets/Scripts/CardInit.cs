using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class CardInit
{
    public int id;
    public string cardName;
    public int cost;
    public int power;
    public string cardDescription;

    public CardInit()
    {

    }

    public CardInit(int Id, string CardName, int Cost, int Power, string CardDescription)
    {
        id = Id;
        cardName = CardName;
        cost = Cost;
        power = Power;
        cardDescription = CardDescription;
    }

}
