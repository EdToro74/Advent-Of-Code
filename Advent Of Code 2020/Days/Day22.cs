using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day22
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(22);

            var players = new Dictionary<int, Queue<int>>();

            var playerIndex = -1;
            var playerMatch = new Regex("^Player (?<number>[0-9]+)\\:$");

            foreach (var line in input)
            {
                if (playerMatch.TryMatch(line, out var match))
                {
                    playerIndex = int.Parse(match.Groups["number"].Captures[0].Value);
                    players.Add(playerIndex, new Queue<int>());
                    continue;
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else
                {
                    var card = int.Parse(line);
                    players[playerIndex].Enqueue(card);
                }
            }

            var part1Players = CloneHands(players);

            var part1WinnerId = PlayGame(1, part1Players,
                plays => plays.OrderByDescending(play => play.card).First().player,
                OnePlayerLeft
            );

            ShowPostGame(part1Players, part1WinnerId);

            var part2Players = CloneHands(players);

            var part2WinnerId = PlayGame(1, part2Players, plays => Part2RoundWinner(1, plays, part2Players), (roundId, currentPlayers) => Part2GameWinner(1, roundId, currentPlayers));

            ShowPostGame(part2Players, part2WinnerId);
        }

        private static int OnePlayerLeft(int roundId, Dictionary<int, Queue<int>> players)
        {
            var haveCards = players.Where(player => player.Value.Count > 0).ToList();
            if (haveCards.Count == 1)
            {
                return haveCards[0].Key;
            }

            return 0;
        }


        private static int Part2GameWinner(int gameId, int roundId, Dictionary<int, Queue<int>> currentPlayers)
        {
            int winnerId;

            if (roundId != 1 && currentPlayers.Any(currentPlayer => _gameHistory[gameId][currentPlayer.Key].Any(oldHand => oldHand.SequenceEqual(currentPlayer.Value))))
            {
                // Infinite recusion fail-safe
                winnerId = 1;
            }
            else
            {
                winnerId = OnePlayerLeft(roundId, currentPlayers);
            }

            if (winnerId != 0)
            {
                _ = _gameHistory.Remove(gameId);
            }
            else
            {
                if (!_gameHistory.TryGetValue(gameId, out var history))
                {
                    history = new Dictionary<int, List<Queue<int>>>();
                    foreach (var currentPlayer in currentPlayers)
                    {
                        history.Add(currentPlayer.Key, new List<Queue<int>>());
                    }

                    _gameHistory[gameId] = history;
                }

                foreach (var player in currentPlayers)
                {
                    history[player.Key].Add(new Queue<int>(player.Value));
                }
            }

            return winnerId;
        }

        private static readonly Dictionary<int, Dictionary<int, List<Queue<int>>>> _gameHistory = new Dictionary<int, Dictionary<int, List<Queue<int>>>>();

        private static int Part2RoundWinner(int gameId, IEnumerable<(int player, int card)> plays, Dictionary<int, Queue<int>> currentPlayers)
        {
            if (plays.All(play => currentPlayers[play.player].Count >= play.card))
            {
                //Console.WriteLine("Playing a sub - game to determine the winner...\r\n");

                var nextPlayers = CloneHands(currentPlayers, (playerId, hand) => new Queue<int>(hand.Take(plays.Single(play => play.player == playerId).card)));

                var nextGameId = gameId + 1;
                var winnerId = PlayGame(
                    nextGameId,
                    nextPlayers,
                    plays => Part2RoundWinner(nextGameId, plays, nextPlayers),
                    (roundId, currentNestedPlayers) => Part2GameWinner(nextGameId, roundId, currentNestedPlayers)
                );

                //Console.WriteLine($"...anyway, back to game {gameId}.\r\n");

                return winnerId;
            }

            return plays.OrderByDescending(play => play.card).First().player;
        }

        private static void ShowPostGame(Dictionary<int, Queue<int>> players, int winnerId)
        {
            //Console.WriteLine("== Post-game results ==");
            foreach (var player in players)
            {
                //Console.WriteLine($"Player {player.Key}'s deck: {string.Join(", ", player.Value)}");
            }

            var winningDeck = players[winnerId];

            Console.WriteLine($"\r\nPlayer {winnerId} wins with a score of {winningDeck.Reverse().Select((card, index) => (card, index: index + 1)).Sum(item => item.card * item.index)}\r\n");
        }

        private static int PlayGame(int gameId, Dictionary<int, Queue<int>> players, Func<IEnumerable<(int player, int card)>, int> determineRoundWinner, Func<int, Dictionary<int, Queue<int>>, int> determineGameWinner)
        {
            //Console.WriteLine($"=== Game {gameId} ===\r\n");

            var round = 1;

            int gameWinnerId;
            while ((gameWinnerId = determineGameWinner(round, players)) == 0)
            {
                //Console.WriteLine($"-- Round {round} (Game {gameId}) --");
                foreach (var player in players)
                {
                    //Console.WriteLine($"Player {player.Key}'s deck: {string.Join(", ", player.Value)}");
                }

                var plays = players.Select(kvp => (player: kvp.Key, card: kvp.Value.Dequeue())).ToList(); ;

                foreach (var play in plays.OrderBy(play => play.player))
                {
                    //Console.WriteLine($"Player {play.player} plays: {play.card}");
                }

                var winnerId = determineRoundWinner(plays);
                var winner = plays.Single(play => play.player == winnerId);

                //Console.WriteLine($"Player {winnerId} wins round {round} of game {gameId}!");

                players[winner.player].Enqueue(winner.card);
                foreach (var play in plays.Where(play => play.player != winner.player))
                {
                    players[winner.player].Enqueue(play.card);
                }

                round++;
                //Console.WriteLine();
            }

            //Console.WriteLine($"The winner of game {gameId} is player {gameWinnerId}\r\n");
            return gameWinnerId;
        }

        private static Dictionary<int, Queue<int>> CloneHands(Dictionary<int, Queue<int>> players, Func<int, Queue<int>, Queue<int>> modifier = null) => new Dictionary<int, Queue<int>>(
                players.Select(player => new KeyValuePair<int, Queue<int>>(player.Key, new Queue<int>(modifier != null ? modifier(player.Key, player.Value) : player.Value)))
            );
    }
}