using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using CardGameProject; 


public class CardDisplay : MonoBehaviour
{
    public CardCreature cardData;
    public Image cardImage; 
    public TMP_Text nameText; 
    public TMP_Text healthText;
    public TMP_Text damageText;
    public TMP_Text movementRangeText;
    public TMP_Text attackRangeText; 
    public Image[] typeImage; 

    

    // Update is called once per frame
    public void UpdateCardDisplay()
    {
        
        nameText.text = cardData.cardName;
        healthText.text = cardData.health.ToString(); 
        damageText.text = cardData.attack.ToString(); 
        movementRangeText.text = cardData.movementRange.ToString();
        attackRangeText.text = cardData.attackRange.ToString(); 
        
        cardImage.sprite = cardData.charSprite;
           
    }
}
