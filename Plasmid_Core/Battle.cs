using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Plasmid.Cards;
using Plasmid.Microbes;

namespace Plasmid
{
    class Battle
    {
        public Microbe PlayerMicrobe { get; set; }
        public Microbe OpponentMicrobe { get; set; }
        public bool PlayerTurn { get; set; }

        public Battle(Microbe player, Microbe opponent)
        {
            PlayerMicrobe = player;
            OpponentMicrobe = opponent;
            PlayerTurn = true;
        }
        
        public void PlayCard(Card card)
        {
            foreach (CardEffect effect in card.CardEffects)
                switch(effect.Type)
                {
                    //case EffectType.Block:
                    case CardEffectType.Damage:
                        if (PlayerTurn)
                            Damage(OpponentMicrobe, effect.Value);
                        else
                            Damage(PlayerMicrobe, effect.Value);
                        break;
                }

        }

        public void Damage(Microbe microbe, int amount)
        {
            //Debug.Write(amount + " damage. " + microbe.HP + " -> ");
            //microbe.hp -= amount;
            //if (microbe.hp < 0)
            //    microbe.hp = 0;
            //Debug.WriteLine(microbe.HP);
        }


    }
}
