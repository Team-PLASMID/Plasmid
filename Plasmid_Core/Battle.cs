using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Plasmid_Core
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
            foreach (Effect effect in card.Effects)
                switch(effect.Type)
                {
                    //case EffectType.Block:
                    case EffectType.Damage:
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
            microbe.HP -= amount;
            if (microbe.HP < 0)
                microbe.HP = 0;
            //Debug.WriteLine(microbe.HP);
        }


    }
}
