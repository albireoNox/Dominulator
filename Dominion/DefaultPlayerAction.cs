﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class DefaultPlayerAction
        : IPlayerAction
    {

        private Card NoCard()
        {
            return null;
        }

        private void NoDefaultAction()
        {

        }

        private Card PlayerMustMakeCardChoice()
        {
            throw new NotImplementedException();
        }

        private bool PlayerMustMakeChoice()
        {
            throw new NotImplementedException();
        }

        private PlayerActionChoice PlayerMustMakeActionChoice()
        {
            throw new NotImplementedException();
        }

        private DeckPlacement PlayerMustMakeDeckPlacement()
        {
            throw new NotImplementedException();
        }

        private int PlayerMustChooseNumber()
        {
            throw new NotImplementedException();
        }

        private Card NoCardIfOptional(bool isOptional)
        {
            if (isOptional)
            {
                return NoCard();
            }

            return PlayerMustMakeCardChoice();
        }

        virtual public string PlayerName 
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        virtual public void BeginTurn()
        {
            NoDefaultAction();
        }

        virtual public void EndTurn()
        {
            NoDefaultAction();
        }

        virtual public Card BanCardForCurrentPlayerRevealedCards(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card BanCardForCurrentPlayerPurchase(GameState gameState)
        {            
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardPileFromSupply(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return NoCardIfOptional(isOptional);
        }

        virtual public Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate cardPredicate)
        {
            return NoCard();
        }

        virtual public Card GuessCardTopOfDeck(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card NameACard(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return NoCardIfOptional(isOptional);   
        }

        virtual public Card GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromRevealedCardsToTrash(GameState gameState, PlayerState player, CardPredicate acceptableCard)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromDiscardToTopDeck(GameState gameState, PlayerState playerState, bool isOptional)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromHandToPassLeft(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, PlayerState player, bool isOptional)
        {
            return NoCardIfOptional(isOptional);
        }

        // always optional
        virtual public Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement)
        {
            deckPlacement = DeckPlacement.Default;
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldPlayerDiscardCardFromHand(GameState gameState, PlayerState playerState, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool WantToResign(GameState gameState)
        {
            return false;
        }        

        virtual public bool ShouldPutDeckInDiscard(GameState gameState)
        {
            return PlayerMustMakeChoice();
        }        

        virtual public bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldTrashCard(GameState gameState, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldGainCard(GameState gameState, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            return PlayerMustMakeActionChoice();
        }

        virtual public DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            return PlayerMustMakeDeckPlacement();
        }

        virtual public int GetNumberOfCardsFromDiscardToPutInHand(GameState gameState, int maxNumber)
        {
            return PlayerMustChooseNumber();
        }

        virtual public Card GetCardFromHandToDeferToNextTurn(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromHandToIsland(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            return PlayerMustChooseNumber();
        }

        virtual public int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            return PlayerMustChooseNumber();
        }

        virtual public int GetCoinAmountToSpendInBuyPhase(GameState gameState)
        {
            return PlayerMustChooseNumber();
        }

        virtual public int GetCoinAmountToUseInButcher(GameState gameState)
        {
            return PlayerMustChooseNumber();
        }

        virtual public Card GetCardFromPlayToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
        {
            return PlayerMustMakeCardChoice();
        }
    }    
}
