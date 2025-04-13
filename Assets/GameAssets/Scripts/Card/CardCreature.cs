using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CardGameProject {

    [CreateAssetMenu(fileName = "NewCreatureClass", menuName = "Custom/CreatureCard")]
    public class CardCreature : Card 
    {
        public float health;
        public float attack;
        public int movementRange = 1;
        public int attackRange = 1; 


        private void onEnable(){
            type = CardType.Creature; 
        }
    }
}