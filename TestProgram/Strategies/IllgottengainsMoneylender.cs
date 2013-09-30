﻿using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        public static class IllgottengainsMoneylender
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("IllgottengainsMoneylender",
                            playerNumber,
                            purchaseOrder: PurchaseOrder())
                {
                }              
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.IllGottenGains>(),
                    CardAcceptance.For<CardTypes.Gold>(gameState => CountOfPile<CardTypes.Province>(gameState) >= 6),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2));                    

                var buildOrder = new CardPickByBuildOrder(
                    new CardTypes.Moneylender(),
                    new CardTypes.Silver());

                var lowPriority = new CardPickByPriority(                           
                           CardAcceptance.For<CardTypes.Silver>(),
                           CardAcceptance.For<CardTypes.Copper>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }                        
        }

        public static class MoneyLender
        {

            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("MoneyLender",
                            playerNumber,
                            purchaseOrder: PurchaseOrder())
                {
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                    CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 4),                           
                    CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2),
                    CardAcceptance.For<CardTypes.Gold>(),
                    CardAcceptance.For<CardTypes.Stables>(),
                    CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                    CardAcceptance.For<CardTypes.Moneylender>(gameState => CountAllOwned<CardTypes.Moneylender>(gameState) < 1),
                    CardAcceptance.For<CardTypes.Silver>());
            }
        }
    }
}
