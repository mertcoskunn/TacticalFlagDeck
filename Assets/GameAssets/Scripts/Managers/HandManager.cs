
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardGameProject;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handTransform;
    public float fanSpread = 7.5f;
    public float cardSpacing = 100f;
    public float verticalSpacing = 100f;
    public int maxHandSize = 12; 

    private List<GameObject> cardsInHand = new List<GameObject>();


    
    public void UpdateHandVisual(){
        int cardCount = cardsInHand.Count; 

        if(cardCount == 1){
            
            cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            return;
          
        }


        for(int i=0; i<cardCount; i++){
            float rotationAngle = (fanSpread * (i-(cardCount-1)/2f));
            cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

            float horizontalOffset = (cardSpacing * (i-(cardCount-1)/2f));
            float normalizedPosition = (2f * i/ (cardCount-1) -1f);
            
            float verticalOffset = verticalSpacing *  (1 - normalizedPosition*normalizedPosition);

            cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
            cardsInHand[i].GetComponent<CardInteraction>().transform.SetSiblingIndex(cardsInHand[i].GetComponent<CardInteraction>().GetSiblingIndex());

        }
    }
    
    public void AddCardToHand(CardCreature cardData){

        if(cardsInHand.Count< maxHandSize){
            
            GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
            cardsInHand.Add(newCard);
            newCard.GetComponent<CardDisplay>().cardData = cardData;
            newCard.GetComponent<CardInteraction>().SetSiblingIndex(-1*(cardsInHand.Count));
            newCard.GetComponent<CardDisplay>().UpdateCardDisplay();
        }
        
        UpdateHandVisual();
    }   
    public List<GameObject> GetCardsInHand()
    {
        return cardsInHand;
    }
    public void SetLockedCardsInHand(bool var)
    {
        for(int i=0; i<cardsInHand.Count; i++)
        {
            cardsInHand[i].GetComponent<CardInteraction>().SetIsLocked(var); 
        }
    }
    public void RemoveCardFromHand(GameObject obj)
    {
        cardsInHand.Remove(obj);
    }

    public void Reset()
    {
        foreach (var obj in cardsInHand)
        {
                if (obj != null)
                {           
                    Destroy(obj);
                }
        }

        cardsInHand.Clear(); 
        

    }

}
