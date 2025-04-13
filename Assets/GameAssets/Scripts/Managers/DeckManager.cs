using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport; 

using UnityEngine;
using CardGameProject;


public class DeckManager : MonoBehaviour
{
   
   public CardDataBase cardDB; 
   
   private List<Card> allCards = new List<Card>();
   private int currentIndex = 0;
    

    private void Start()
    {
       cardDB.Init();
       //GameStart();
    }

   public void DrawCard(HandManager handManager){
    if(allCards.Count == 0){
        return; 
    }
    Card nextCard = allCards[currentIndex];
    handManager.AddCardToHand((CardCreature)nextCard);
    currentIndex = (currentIndex +1)%allCards.Count;
   }
  
    public void Init(int numberOfCardInHand)
    {
        List<int> cardIndexes = GenerateRandomList(8, 8);
        
        for(int i=0; i<cardIndexes.Count; i++){                
                Card card = cardDB.GetCardByIndex(cardIndexes[i]);
                allCards.Add(card);
        } 
        
        HandManager hand = FindObjectOfType<HandManager>();
        for(int i = 0; i<numberOfCardInHand; i++){
                DrawCard(hand);
        }
        
    
    }


    public void Reset()
    {
        allCards.Clear(); 
    }

    public List<int> GenerateRandomList(int totalCount, int range)
    {
        List<int> randomList = new List<int>();

        for (int i = 0; i < totalCount; i++)
        {
            int randomNumber = Random.Range(0, range); 
            randomList.Add(randomNumber);
        }

        return randomList;
    }

}
