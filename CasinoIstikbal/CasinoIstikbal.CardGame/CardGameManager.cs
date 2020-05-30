using System;
using System.Collections.Generic;
using System.Linq;

namespace CasinoIstikbal.CardGame
{
    public class CardGameManager
    {
        private readonly string[] _deckColors = { "S", "K", "M" };
        private const int DeckLimit = 5;
        private const int DeckStart = 1;
        private const int JokerCount = 1;
        private Random rnd;
        private readonly int _playerCardSize = (DeckLimit - DeckStart + 1) + JokerCount;
        public string LastCard = null;
        public string LastCardColor => LastCard?.Substring(0, 1);

        public int LastCardNumber =>
            int.TryParse(LastCard?.Substring(1, 1) ?? string.Empty, out var cardNumber) ? cardNumber : 0;
        public int Tour = 0;

        public CardGameManager()
        {
            rnd = new Random();
        }


        private object[,] CreateDeck()
        {
            var colorDeckSize = (DeckLimit - DeckStart + 1);
            var deckSize = (colorDeckSize + JokerCount) * _deckColors.Length;
            var decks = new object[deckSize, 2];

            var index = 0;
            foreach (var deckColor in _deckColors)
            {
                var cardNumber = DeckStart;
                for (var i = 0; i < colorDeckSize; i++)
                {
                    decks[index, 0] = $"{deckColor}{cardNumber}";
                    decks[index, 1] = CardType.Standard;
                    index++;
                    cardNumber++;
                }
                for (var i = 0; i < JokerCount; i++)
                {
                    decks[index, 0] = $"RD";
                    decks[index, 1] = CardType.Joker;
                    index++;
                }
            }



            return decks;
        }

        private void Shuffle(object[,] deck)
        {


            var count = deck.GetLength(0);
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = rnd.Next(i, count);
                var tmp = new object[1, 2];
                tmp[0, 0] = deck[i, 0];
                tmp[0, 1] = deck[i, 1];
                deck[i, 0] = deck[r, 0];
                deck[i, 1] = deck[r, 1];
                deck[r, 0] = tmp[0, 0];
                deck[r, 1] = tmp[0, 1];
            }
        }

        private object[,,] CardDistribution(object[,] deck)
        {
            var players = new object[_deckColors.Length, _playerCardSize, 4];
            var index = 0;
            for (var i = 1; i <= _deckColors.Length; i++)
            {
                for (int j = 1; j <= _playerCardSize; j++)
                {
                    players[i - 1, j - 1, 0] = deck.GetValue(index, 0);
                    players[i - 1, j - 1, 1] = deck.GetValue(index, 1);
                    players[i - 1, j - 1, 2] = false;
                    players[i - 1, j - 1, 3] = 0;
                    index++;
                }
            }

            return players;
        }

        public object[,,] GetDeck()
        {
            var deck = CreateDeck();
            Shuffle(deck);
            return CardDistribution(deck);
        }
        public void WriteDeck(object[,,] deck, int playerIndex)
        {
            Console.Write("Elinizdeki Kartlar:[");
            for (var i = 0; i < _playerCardSize; i++)
            {
                bool.TryParse(deck[playerIndex, i, 2]?.ToString(), out bool used);
                Console.Write($"{deck[playerIndex, i, 0]}{(used ? "x" : string.Empty)}");
                if (i != _playerCardSize - 1)
                    Console.Write("  ");

            }
            Console.Write("]\n");
        }
        public Tuple<bool, string> ValidateCard(object[,,] deck, int playerIndex, string input)
        {
            var rdCounter = 00;
            for (var i = 0; i < _playerCardSize; i++)
            {
                var card = deck[playerIndex, i, 0]?.ToString()?.ToUpper();
                if (card != input.Substring(0, 2)) continue;

                bool.TryParse(deck[playerIndex, i, 2]?.ToString(), out bool used);
                if (used )
                {
                    if (card.StartsWith("RD") && rdCounter<2)
                        rdCounter++;
                    else
                        return new Tuple<bool, string>(false, "Kartı daha önce kullandınız");
                }

                var cardColor = input.Substring(0, 1);
                int.TryParse(input.Substring(1, 1), out var cardNumber);

                if (Tour == 1 && input.StartsWith("RD"))
                    return new Tuple<bool, string>(false, "İlk Tur için Renk Değiştiren katını kullanamazsınız!"); ;

                if (string.IsNullOrWhiteSpace(LastCard))
                    return new Tuple<bool, string>(true, string.Empty);

                if (input.StartsWith("RD"))
                    return new Tuple<bool, string>(true, string.Empty);

                if (cardColor == LastCardColor)
                    return new Tuple<bool, string>(true, string.Empty);

                if (cardNumber == LastCardNumber && LastCardNumber != 0)
                    return new Tuple<bool, string>(true, string.Empty);

                return new Tuple<bool, string>(false, "Girmiş olduğunuz kart yerdeki kart ile uyuşmuyor. Lütfen aynı renk veya aynı numaraya ait bir kart seçiniz!");

            }

            return new Tuple<bool, string>(false, "Girmiş olduğunuz değer elinizdeki kartlar içerisinde bulunamadı. Lütfen elinizdeki kartlardan birini kullanarak tekrar deneyiniz!");
        }

        public string GetComputerInput(object[,,] deck, int playerIndex)
        {
            var sameColor = new List<string>();
            var colors = new List<string>();
            var sameNumber = new List<string>();
            bool joker = false;
            for (var i = 0; i < _playerCardSize; i++)
            {
                bool.TryParse(deck[playerIndex, i, 2]?.ToString(), out var used);
                if (used) continue;

                var card = deck[playerIndex, i, 0]?.ToString()?.ToUpper();
                if (card == "RD")
                    joker = true;
                else
                {
                    var cardColor = card.Substring(0, 1);

                    colors.Add(cardColor);
                    int.TryParse(card.Substring(1, 1), out var cardNumber);

                    if (cardColor == LastCardColor)
                        sameColor.Add(card);

                    if (cardNumber == LastCardNumber && LastCardNumber != 0)
                        sameNumber.Add(card);
                }

            }

            if (sameColor.Any())
            {
                var colorIndex = rnd.Next(0, sameColor.Count);
                return sameColor[colorIndex];
            }

            if (sameNumber.Any())
            {
                var numberIndex = rnd.Next(0, sameNumber.Count);
                return sameNumber[numberIndex];
            }

            if (colors.Any())
            {
                var randomColorIndex = rnd.Next(0, colors.Count);
                return joker ? $"RD_{colors[randomColorIndex]}" : "PAS";
            }
            var randIndex = rnd.Next(0, 3);
            return joker ? $"RD_{(randIndex == 0 ? "S" : randIndex == 1 ? "K" : "M")}" : "PAS";

        }

        public void PlayCard(object[,,] deck, int playerIndex, string input)
        {
            var color = string.Empty;
            if (input == "PAS")
                return;
            if (input.StartsWith("RD"))
            {
                color = input.Substring(3, 1);
                input = "RD";
            }


            for (var i = 0; i < _playerCardSize; i++)
            {
                if (deck[playerIndex, i, 0]?.ToString()?.ToUpper() != input) continue;

                deck[playerIndex, i, 2] = true;
                deck[playerIndex, i, 3] = Tour;
                break;
            }

            LastCard = input.StartsWith("RD") ? $"{color}{LastCardNumber}" : input;
        }

        public bool CheckWinner(object[,,] deck, int playerIndex)
        {
            for (var i = 0; i < _playerCardSize; i++)
            {
                bool.TryParse(deck[playerIndex, i, 2]?.ToString(), out bool used);
                if (used) continue;

                return false;
            }

            return true;
        }
    }

    public static class CardType
    {
        public static string Standard = "Standard";
        public static string Joker = "Joker";
    }
}