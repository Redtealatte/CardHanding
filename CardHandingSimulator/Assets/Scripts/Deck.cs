using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;//ToList를 사용하기 위해 사용.

public class Deck : Pile
{
    List<CardInit> saveDeck;
    // Start is called before the first frame update
    void Start()
    {
        pileCount = transform.Find("Count").GetComponent<Text>();
        saveDeck = new List<CardInit>();

        for(int i = 0; i < 11; i++)
        {
            if (i < 3)
                saveDeck.Add(CardDataBase.cardList[0]);
            else if(i<5)
                saveDeck.Add(CardDataBase.cardList[1]);
            else if (i < 7)
                saveDeck.Add(CardDataBase.cardList[2]);
            else if (i < 9)
                saveDeck.Add(CardDataBase.cardList[3]);
            else if (i < 11)
                saveDeck.Add(CardDataBase.cardList[4]);
        }

        pile = saveDeck.ToList();
        Shuffle.shuffle(pile);
        pileCount.text = pile.Count.ToString();
    }

    /// <summary>
    /// pile List내의 원소를 무작위로 섞는다.
    /// </summary>
    public void ShufflePile()
    {
        Shuffle.shuffle(pile);
    }
}
