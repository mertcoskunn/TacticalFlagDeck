using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CardGameProject{
    public class Card : ScriptableObject
    {
        public string cardName; 
        public string cardDescription;
        protected CardType type;  
        

        public Sprite charSprite; 
        public RuntimeAnimatorController  charAnimator; 
        //public Sprite cardSprite;
        public GameObject prefab;

        private int cardIndex; 
         

        public enum CardType
        {
            Creature,
            Spell 
        }


        public void SetCardIndex(int val)
        {
            cardIndex = val; 
        }

        public int GetCardIndex()
        {
            return cardIndex; 
        }

    }
} 