﻿using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{    
    static class DefaultResponses
    {
        public static MapOfCardsFor<IPlayerAction> GetCardResponses(PlayerAction playerAction)
        {
            var result = new MapOfCardsFor<IPlayerAction>();

            result[Cards.Ambassador] = new Ambassador(playerAction);
            result[Cards.Alchemist] = new Alchemist(playerAction);
            result[Cards.BandOfMisfits] = new BandOfMisfits(playerAction);
            result[Cards.Cartographer] = new Cartographer(playerAction);
            result[Cards.Catacombs] = new Catacombs(playerAction);
            result[Cards.Chancellor] = new Chancellor(playerAction);
            result[Cards.Count] = new Count(playerAction);
            result[Cards.Golem] = new Golem(playerAction);
            result[Cards.HorseTraders] = new HorseTraders(playerAction);
            result[Cards.IllGottenGains] = new IllGottenGainsAlwaysGainCopper(playerAction);
            result[Cards.Library] = new Library(playerAction);
            result[Cards.MarketSquare] = new MarketSquare(playerAction);
            result[Cards.Mystic] = new Mystic(playerAction);
            result[Cards.Rebuild] = new Rebuild(playerAction);
            result[Cards.Trader] = new Trader(playerAction);
            result[Cards.Watchtower] = new Watchtower(playerAction);

            return result;
        }

        public static MapOfCardsFor<GameStatePlayerActionPredicate> GetCardShouldPlayDefaults(PlayerAction playerAction)
        {
            var result = new MapOfCardsFor<GameStatePlayerActionPredicate>();

            result[Cards.Remodel] = Strategies.HasCardToTrashInHand;
            result[Cards.Salvager] = Strategies.HasCardToTrashInHand;
            result[Cards.Lookout] = Lookout.ShouldPlay;

            return result;
        }
    }
}
