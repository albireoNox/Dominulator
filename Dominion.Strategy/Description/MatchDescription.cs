﻿using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Strategy.Description
{
    public class MatchDescription
    {
        // e.g. CountAllOwned(Province.card, gameState) < 6
        public readonly Card cardType;
        public readonly CountSource countSource;
        public readonly Comparison comparison;
        public int countThreshHold;

        public MatchDescription(Card card)
        {
            this.cardType = card;
            this.countSource = CountSource.Always;
            this.comparison = Comparison.Equals;
            this.countThreshHold = 1;
        }

        public MatchDescription(CountSource countSource, Card cardType, Comparison comparison, int threshhold)
        {
            this.cardType = cardType;
            this.countSource = countSource;
            this.comparison = comparison;
            this.countThreshHold = threshhold;
        }

        public MatchDescription Clone()
        {
            return new MatchDescription(this.countSource, this.cardType, this.comparison, this.countThreshHold);
        }

        public CardAcceptance ToCardAcceptance()
        {
            return CardAcceptance.For(this.cardType, this.GameStatePredicate);
        }

        public bool GameStatePredicate(GameState gameState)
        {
            int countOfTheSource;

            switch (countSource)
            {
                case CountSource.CountOfPile:
                    countOfTheSource = Strategy.CountOfPile(this.cardType, gameState);
                    break;
                case CountSource.CountAllOwned:
                    countOfTheSource = Strategy.CountAllOwned(this.cardType, gameState);
                    break;
                case CountSource.InHand:
                    countOfTheSource = Strategy.CountInHand(this.cardType, gameState);
                    break;
                case CountSource.Always:
                    return true;
                default:
                    throw new Exception("Unhandled source case");
            }

            switch (this.comparison)
            {
                case Comparison.GreaterThan:
                    {
                        return countOfTheSource > this.countThreshHold;
                    }
                case Comparison.LessThan:
                    {
                        return countOfTheSource < this.countThreshHold;
                    }
                case Comparison.GreaterThanEqual:
                    {
                        return countOfTheSource >= this.countThreshHold;
                    }
                case Comparison.LessThanEqual:
                    {
                        return countOfTheSource <= this.countThreshHold;
                    }
                case Comparison.Equals:
                    {
                        return countOfTheSource == this.countThreshHold;
                    }
                default:
                    throw new Exception("Unhandled comparison case");
            }
        }

        public void WriteText(System.IO.TextWriter writer)
        {
            if (this.countThreshHold > 0)
            {
                writer.Write("({0})", this.countThreshHold);
            }
        }        
    }
}