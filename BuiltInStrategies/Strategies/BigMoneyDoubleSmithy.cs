﻿using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{    
    public class BigMoneyDoubleSmithy
        : Strategy 
    {

        public static PlayerAction Player()
        {
            return CustomPlayer();
        }
            
        public static PlayerAction CustomPlayer(int secondSmithy = 15)
        {
            return new PlayerAction(
                        "BigMoneyDoubleSmithy",                            
                        purchaseOrder: PurchaseOrder(secondSmithy));
        }

        private static CardPickByPriority PurchaseOrder(int secondSmithy)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Smithy, gameState => CountAllOwned(Cards.Smithy, gameState) < 1),
                        CardAcceptance.For(Cards.Smithy, gameState => CountAllOwned(Cards.Smithy, gameState) < 2 &&
                                                                            gameState.Self.AllOwnedCards.Count >= secondSmithy),
                        CardAcceptance.For(Cards.Silver));

        }           
    }
}
