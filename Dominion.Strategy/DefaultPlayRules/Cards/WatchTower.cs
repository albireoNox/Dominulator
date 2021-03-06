﻿using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Watchtower
        : DerivedPlayerAction
    {
        public Watchtower(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }

        public override DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            if (playerAction.gainOrder.DoesCardPickerMatch(gameState, card))
                return DeckPlacement.TopOfDeck;
            
            return DeckPlacement.Trash;            
        }
    }
}