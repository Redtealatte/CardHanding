using System.Collections;
using System.Collections.Generic;
using System;

public class Shuffle
{
    public static void shuffle<T>(List<T> data)
    {
        for(int i =0; i < data.Count; i++)
        {
            Random r = new Random();
            int ranValue = r.Next(0, data.Count);
            T temp = data[i];
            data[i] = data[ranValue];
            data[ranValue] = temp;
        }
    }

}
