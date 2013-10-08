﻿using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class DarkAgesBigMoney
    {
        static void Run()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=6281.0
            Program.ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoney.Player(2));
            //ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Catacombs>.Player(1, 2), Strategies.BigMoney.Player(2));
            //ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Count>.Player(1), Strategies.BigMoney.Player(2));
            Program.ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.HuntingGrounds>.Player(1), Strategies.BigMoney.Player(2));
        }
    }
}