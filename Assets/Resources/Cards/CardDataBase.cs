using UnityEngine;
using CardGameProject;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Card/Card Database")]
public class CardDataBase : ScriptableObject
{
    public Card[] cards; // index sırasına göre dizilir

    public Card GetCardByIndex(int index)
    {
        if (index >= 0 && index < cards.Length)
            return cards[index];
        else
            return null;
    }

    public void Init()
    {
        for(int i = 0; i<cards.Length;i++)
        {   
            if(cards[i] != null){
                cards[i].SetCardIndex(i); 
            }
        }
    }
}
