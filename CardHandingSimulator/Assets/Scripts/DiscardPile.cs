using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiscardPile : Pile
{
    // Start is called before the first frame update
    void Start()
    {
        pile = new List<CardInit>();

        pileCount = transform.Find("Count").GetComponent<Text>();
        pileCount.text = pile.Count.ToString();
    }
}
