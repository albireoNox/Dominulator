﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Ambassador
       : Card
    {
        public static Ambassador card = new Ambassador();

        private Ambassador()
            : base("Ambassador", Expansion.Seaside, coinCost: 3, isAction: true, attackDependsOnPlayerChoice: true, isAttack:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.RequestPlayerRevealCardFromHand(acceptableCard => true, gameState);
            PlayerState.AttackAction attackAction = this.DoEmptyAttack;            

            if (revealedCard != null)
            {                
                int maxReturnCount = Math.Max(currentPlayer.Hand.CountOf(revealedCard), 2);            
            
                int returnCount = currentPlayer.actions.GetCountToReturnToSupply(revealedCard, gameState);
                returnCount = Math.Min(returnCount, maxReturnCount);
                returnCount = Math.Max(returnCount, 0);                       

                for (int i = 0; i < returnCount; ++i)
                {
                    if (currentPlayer.hand.HasCard(revealedCard))
                    {
                        currentPlayer.ReturnCardFromHandToSupply(revealedCard, gameState);
                    }
                }

                attackAction = delegate(PlayerState currentPlayer2, PlayerState otherPlayer, GameState gameState2)
                {
                    otherPlayer.GainCardFromSupply(gameState, revealedCard);
                };
            }

            currentPlayer.AttackOtherPlayers(gameState, attackAction);
        }
    }

    public class Bazaar 
        : Card 
    { 
        public static Bazaar card = new Bazaar();

        private Bazaar() 
            : base("Bazaar", Expansion.Seaside, coinCost: 5, isAction: true, plusCoins: 1, plusCards: 1, plusActions: 2) 
        { 
        } 
    }

    public class Caravan
       : Card
    {
        public static Caravan card = new Caravan();

        private Caravan()
            : base("Caravan", Expansion.Seaside, coinCost: 4, isAction: true, isDuration: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(1, gameState);
        }
    }

    public class Cutpurse
      : Card
    {
        public static Cutpurse card = new Cutpurse();

        private Cutpurse()
            : base("Cutpurse", Expansion.Seaside, coinCost: 4, isAction: true, plusCoins: 2, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            if (!otherPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card == Copper.card, isOptional: false))
            {
                otherPlayer.RevealHand();
            }
        }
    }

    public class Embargo
       : Card
    {
        public static Embargo card = new Embargo();

        private Embargo()
            : base("Embargo", Expansion.Seaside, coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.MoveCardFromPlayToTrash(gameState);
            PileOfCards cardPile = currentPlayer.RequestPlayerEmbargoPileFromSupply(gameState);
            gameState.AddEmbargoTokenToPile(cardPile);
        }
    }

    public class Explorer
      : Card
    {
        public static Explorer card = new Explorer();

        private Explorer()
            : base("Explorer", Expansion.Seaside, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerRevealCardFromHand(card => card == Province.card, gameState) != null)
            {
                currentPlayer.GainCardFromSupply(Gold.card, gameState, DeckPlacement.Hand);
            }
            else
            {
                currentPlayer.GainCardFromSupply(Silver.card, gameState, DeckPlacement.Hand);
            }
        }
    }

    public class FishingVillage
       : Card
    {
        public static FishingVillage card = new FishingVillage();

        private FishingVillage()
            : base("Fishing Village", Expansion.Seaside, coinCost: 3, isAction: true, isDuration: true, plusCoins: 1, plusActions: 2)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoins(1);
            currentPlayer.AddActions(1);
        }
    }

    public class GhostShip
       : Card
    {
        public static GhostShip card = new GhostShip();

        private GhostShip()
            : base("Ghost Ship", Expansion.Seaside, coinCost: 5, isAction: true, isAttack: true, plusCards: 2)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            while (otherPlayer.Hand.Count > 3)
            {
                otherPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard => true, isOptional: false);
            }
        }
    }

    public class Haven
       : Card
    {
        public static Haven card = new Haven();

        private Haven()
            : base("Haven", Expansion.Seaside, coinCost: 2, isAction: true, isDuration: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDeferCardFromHandtoNextTurn(gameState);
        }
    }

    public class Island
       : Card
    {
        public static Island card = new Island();

        private Island()
            : base("Island", Expansion.Seaside, coinCost: 4, isAction: true, victoryPoints: playerState => 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.MoveCardFromPlayedCardToIslandMat(this);

            if (!currentPlayer.hand.Any)
                return;            
            Card cardType = currentPlayer.actions.GetCardFromHandToIsland(gameState);
            if (currentPlayer.Hand.Any && cardType == null)
                throw new Exception("Player must island a card from his hand");
            if (cardType != null)
            {
                currentPlayer.MoveCardFromHandToIslandMat(cardType);
                gameState.gameLog.PlayerPlacedCardOnIslandMat(currentPlayer, cardType);
            }
        }
    }

    public class Lighthouse
       : Card
    {
        public static Lighthouse card = new Lighthouse();

        private Lighthouse()
            : base("Lighthouse", Expansion.Seaside, coinCost: 2, isAction: true, isDuration: true, plusCoins: 1, plusActions: 1)
        {
        }

        public override bool DoReactionToAttackWhileInPlayAcrossTurns(PlayerState currentPlayer, GameState gameState)
        {
            return false;
        }
    }

    public class Lookout
       : Card
    {
        public static Lookout card = new Lookout();

        private Lookout()
            : base("Lookout", Expansion.Seaside, coinCost: 3, isAction: true, plusActions: 1)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(3, gameState);            
            currentPlayer.RequestPlayerTrashRevealedCard(gameState, acceptableCard => true);
            currentPlayer.RequestPlayerDiscardRevealedCard(gameState);
            currentPlayer.MoveRevealedCardToTopOfDeck();            
        }
    }

    public class MerchantShip
       : Card
    {
        public static MerchantShip card = new MerchantShip();

        private MerchantShip()
            : base("Merchant Ship", Expansion.Seaside, coinCost: 5, isAction: true, isDuration: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoins(2);
        }
    }

    public class NativeVillage
       : Card
    {
        public static NativeVillage card = new NativeVillage();

        private NativeVillage()
            : base("Native Village", Expansion.Seaside, coinCost: 2, isAction: true, plusActions: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.PutNativeVillageMatInHand ||
                    acceptableChoice == PlayerActionChoice.SetAsideTopCardOnNativeVillageMat);

            if (choice == PlayerActionChoice.PutNativeVillageMatInHand)
            {
                currentPlayer.MoveNativeVillageMatToHand();
            }
            else if (choice == PlayerActionChoice.SetAsideTopCardOnNativeVillageMat)
            {
                currentPlayer.MoveCardFromPlayedCardToNativeVillageMatt(this);
                currentPlayer.PutOnNativeVillageMatCardFromTopOfDeck(gameState);
            }
        }
    }

    public class Navigator
       : Card
    {
        public static Navigator card = new Navigator();

        private Navigator()
            : base("Navigator", Expansion.Seaside, coinCost: 4, isAction: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.LookAtCardsFromDeck(5, gameState);
            PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.Discard ||
                                    acceptableChoice == PlayerActionChoice.TopDeck);

            if (choice == PlayerActionChoice.TopDeck)
            {
                currentPlayer.RequestPlayerTopDeckRevealedCardsInAnyOrder(gameState);
            }
            else if (choice == PlayerActionChoice.Discard)
            {
                currentPlayer.MoveRevealedCardsToDiscard(gameState);
            }
        }
    }

    public class Outpost
       : Card
    {
        public static Outpost card = new Outpost();

        private Outpost()
            : base("Outpost", Expansion.Seaside, coinCost: 5, isAction: true, isDuration: true)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {            
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (!currentPlayer.durationCards.HasCard(Outpost.card))
            {
                gameState.doesCurrentPlayerNeedOutpostTurn = true;
            }
        }
    }

    public class PearlDiver
       : Card
    {
        public static PearlDiver card = new PearlDiver();

        private PearlDiver()
            : base("Pearl Diver", Expansion.Seaside, coinCost: 2, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.LookAtBottomCardFromDeck(gameState);

            if (currentPlayer.actions.ShouldPutCardOnTopOfDeck(card, gameState))
            {
                currentPlayer.MoveCardFromBottomOfDeckToTop();                    
            }
        }
    }

    public class PirateShip
       : Card
    {
        public static PirateShip card = new PirateShip();

        private PirateShip()
            : base("Pirate Ship", Expansion.Seaside, coinCost: 4, isAction: true, isAttack: true, attackDependsOnPlayerChoice:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice choice = currentPlayer.RequestPlayerChooseBetween(gameState,
                acceptableChoice =>
                    acceptableChoice == PlayerActionChoice.PlusCoin ||
                    acceptableChoice == PlayerActionChoice.Trash);

            PlayerState.AttackAction attackAction = this.DoEmptyAttack;

            bool wasACardTrashed = false;

            if (choice == PlayerActionChoice.PlusCoin)
            {
                currentPlayer.AddCoins(currentPlayer.pirateShipTokenCount);                
            }
            else if (choice == PlayerActionChoice.Trash)
            {
                attackAction = delegate (PlayerState currentPlayer2, PlayerState otherPlayer, GameState gameState2)
                {
                    otherPlayer.RevealCardsFromDeck(2, gameState);
                    Card trashedCard = currentPlayer2.RequestPlayerTrashOtherPlayersRevealedCard(gameState2, card => card.isTreasure, otherPlayer);
                    otherPlayer.MoveRevealedCardsToDiscard(gameState);
                    wasACardTrashed |= trashedCard != null;
                };
            }

            currentPlayer.AttackOtherPlayers(gameState, attackAction);

            if (wasACardTrashed)
            {
                currentPlayer.pirateShipTokenCount++;
            }
        }        
    }

    public class Salvager
       : Card
    {
        public static Salvager card = new Salvager();

        private Salvager()
            : base("Salvager", Expansion.Seaside, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: false);
            if (card != null)
            {
                currentPlayer.AddCoins(card.CurrentCoinCost(currentPlayer));
            }
        }
    }

    public class SeaHag
       : Card
    {
        public static SeaHag card = new SeaHag();

        private SeaHag()
            : base("Sea Hag", Expansion.Seaside, coinCost: 4, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.DiscardCardFromTopOfDeck(gameState);
            otherPlayer.GainCardFromSupply(Curse.card, gameState, DeckPlacement.TopOfDeck);
        }
    }

    public class Smugglers
       : Card
    {
        public static Smugglers card = new Smugglers();

        private Smugglers()
            : base("Smugglers", Expansion.Seaside, coinCost: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card smuggledCard = null;

            var smuggleTargets = gameState.players.PlayerRight.CardsGainedThisTurn.Where(card =>
                card.CurrentCoinCost(currentPlayer) <= 6 && card.potionCost == 0 && gameState.CardGameSubset.HasCard(card));

            if (smuggleTargets.Count() == 1)
            {
                smuggledCard = currentPlayer.GainCardFromSupply(gameState, smuggleTargets.First());
            }

            
            if (smuggleTargets.Count() > 1)
            {
                smuggledCard = currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => smuggleTargets.Contains(card),
                    "Choose a card to smuggle", isOptional: false);
            }
        }
    }   

    public class Tactician
       : Card
    {
        public static Tactician card = new Tactician();

        private Tactician()
            : base("Tactician", Expansion.Seaside, coinCost: 5, isAction: true, isDuration: true)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {    
            // intentionally left empty.
            // an action is added conditionally on the state of the players hand in the previous turn.
            // still needs to be a duration card so that the tactician stays in play.
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.hand.Any)
            {
                currentPlayer.DiscardHand(gameState);
                currentPlayer.actionsToExecuteAtBeginningOfNextTurn.Add(DelayedAction);                
            }
        }

        private static void DelayedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(5, gameState);
            currentPlayer.AddBuys(1);
            currentPlayer.AddActions(1);
        }
    }

    public class TreasureMap
       : Card
    {
        public static TreasureMap card = new TreasureMap();

        private TreasureMap()
            : base("Treasure Map", Expansion.Seaside, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.MoveCardFromPlayToTrash(gameState))
            {
                if (currentPlayer.TrashCardFromHandOfType(TreasureMap.card, gameState, guaranteeInHand: false) != null)
                {
                    currentPlayer.GainCardsFromSupply(gameState, Gold.card, 4, DeckPlacement.TopOfDeck);
                }
            }
        }
    }

    public class Treasury
       : Card
    {
        public static Treasury card = new Treasury();

        private Treasury()
            : base("Treasury", Expansion.Seaside, coinCost: 5, isAction: true, plusActions: 1, plusCards: 1, plusCoins: 1)
        {
            this.doSpecializedCleanupAtStartOfCleanup = DoSpecializedCleanupAtStartOfCleanup;
        }

        private new void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            if (!currentPlayer.CardsBoughtThisTurn.AnyWhere(card => card.isVictory))
            {
                currentPlayer.RequestPlayerTopDeckCardFromCleanup(this, gameState);                
            }
        }
    }


    public class Warehouse
       : Card
    {
        public static Warehouse card = new Warehouse();

        private Warehouse()
            : base("Warehouse", Expansion.Seaside, coinCost: 3, isAction: true, plusActions: 1, plusCards: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 3, isOptional: false);
        }
    }

    public class Wharf
       : Card
    {
        public static Wharf card = new Wharf();

        private Wharf()
            : base("Wharf", Expansion.Seaside, coinCost: 5, isAction: true, isDuration: true, plusCards: 2, plusBuy: 1)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(2, gameState);
            currentPlayer.AddBuys(1);
        }
    }
}