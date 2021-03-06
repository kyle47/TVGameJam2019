﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Game.Assets
{
    public class TurnManager
    {
        protected int _round = 0;
        protected List<Player> _players = new List<Player>();
        protected List<Player> _playersInTurn = new List<Player>();
        protected Player _activePlayer;

        public Action<int, Action> OnRoundStart;
        public Action<Player, Action> OnTurnStart;

        public TurnManager(List<Player> players)
        {
            _players = players;
        }

        public void NextRound()
        {
            _round++;
            OnRoundStart.Invoke(_round, StartRound);
        }

        protected void StartRound()
        {
            _playersInTurn.Clear();
            _players.ForEach(player => _playersInTurn.Add(player));
            NextTurn();
        }

        public void NextTurn()
        {
            EndTurn();
            if(_playersInTurn.Count < 1)
            {
                NextRound();
                return;
            }

            _activePlayer = _playersInTurn[0];
            _playersInTurn.RemoveAt(0);

            OnTurnStart.Invoke(_activePlayer, StartTurn);
        }

        protected void StartTurn()
        {
            var map = GameManager.Instance.LevelData.Map;
            var pawns = map
                .GetAgents()
                .Select(agent => (Pawn)agent)
                .Where(pawn => pawn.Owner == _activePlayer)
                .ToList();

            pawns.ForEach(pawn =>
            {
                pawn.CanMove = true;
            });

            _activePlayer.TakeTurn(NextTurn);
        }

        protected void EndTurn()
        {
            var map = GameManager.Instance.LevelData.Map;
            var pawns = map.GetAgents().Select(agent => (Pawn)agent).ToList();
            pawns.ForEach(pawn => {
                pawn.PawnComponent.OnTurnEnd();
                pawn.CanMove = false;
            });
        }
    }
}