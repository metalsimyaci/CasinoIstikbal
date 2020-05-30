using System;
using System.Collections.Generic;
using System.Linq;

namespace CasinoIstikbal.CardGame
{
    public class Program
    {
        private static object[,,] _distributedDeck;
        public static CardGameManager Gm;
        public static List<string> tourResult=new List<string>();
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome To Casino Istikbal!");

            Gm = new CardGameManager();
            _distributedDeck = Gm.GetDeck();
            WriteDeck(_distributedDeck);

            Play();
            Console.ReadLine();
        }

        static void WriteInfo()
        {
            Console.WriteLine($"\nYerdeki Kart {Gm.LastCard}, Tur:{Gm.Tour}");
        }
        static void WriteDeck(object[,,] deck)
        {

            for (var i = 0; i < deck.GetLength(0); i++)
            {
                Console.Write($"Player {i + 1}:[");
                for (var j = 0; j < deck.GetLength(1); j++)
                    Console.Write($"{deck.GetValue(i, j, 0)}{deck.GetValue(i, j, 2)}  ");
                Console.Write($"]\n");
            }
        }

        public static void Play()
        {
            var result = false;
            var winnerMessage = string.Empty;
            string lastTourResult=null;
            int scorelessFlagCounter = 0;
            do
            {

                Gm.Tour++;
                Console.WriteLine($"\n\n{Gm.Tour}. El başlatıldı");
                for (var i = 0; i < _distributedDeck.GetLength(0); i++)
                {
                    WriteInfo();
                    string input;
                    if (i == 0)
                    {
                        Gm.WriteDeck(_distributedDeck, i);
                        Console.WriteLine($"Oyuncu {i} oynuyor...");
                        input = ReadPlayerInput(i);
                    }
                    else
                    {
                        Console.WriteLine($"Bilgisayar {i} oynuyor...");
                        input = ReadComputerInput(i);
                    }

                    Gm.PlayCard(_distributedDeck, i, input);
                    result = Gm.CheckWinner(_distributedDeck, i);
                    if (result)
                    {
                        winnerMessage = $"\n\nPlayer {i} kazandı";
                        break;
                    }
                }

                if (lastTourResult == Gm.LastCard)
                {
                    scorelessFlagCounter++;
                }
                else
                {
                    lastTourResult = Gm.LastCard;
                    scorelessFlagCounter = 0;

                }
            } while (!result && scorelessFlagCounter<3);

            if (scorelessFlagCounter >= 3)
            {
                winnerMessage = "3 Tur sonuç değişmediği için berabere sonuçlanmıştır";
            }
            Console.WriteLine(winnerMessage);

        }

        private static string ReadPlayerInput(int playerIndex)
        {
            string input;
            do
            {
                Console.Write("Lütfen bir kart atınız:");
                input = Console.ReadLine()?.ToUpper();
                if (input == "RD")
                {
                    var color = ReadColorInput();
                    input = $"RD_{color}";
                }
            } while (!ValidateCardPlayer(input, playerIndex));
            Console.Write($"Oyuncu{playerIndex}: {input} kartını oynadı");
            return input;
        }
        private static string ReadColorInput()
        {
            string input;
            do
            {
                Console.Write("Lütfen bir Renk Seçiniz:");
                input = Console.ReadLine()?.ToUpper();
            } while (!(input == "S" || input == "K" || input == "M"));

            return input;
        }
        private static string ReadComputerInput(int playerIndex)
        {
            string input = Gm.GetComputerInput(_distributedDeck, playerIndex);
            Console.Write($"Bilgisayar{playerIndex}: {input} kartını oynadı");

            return input;
        }
        private static bool ValidateCardPlayer(string input, int playerIndex)
        {
            if (Gm.Tour == 1 && (input == "PAS" || input.StartsWith("RD")))
            {
                Console.WriteLine("İlk Tur için PAS veya RD değerlerini kullanamazsınız");
                return false;
            }

            if (Gm.Tour != 1 && input == "PAS")
            {
                return true;
            }

            if (Gm.Tour != 1 && input.StartsWith("RD"))
            {
                return ValidateCard(input, playerIndex);
            }


            if (string.IsNullOrWhiteSpace(input) || input.Length != 2)
            {
                Console.WriteLine("Lütfen Geçerli bir card seçiniz");
                return false;
            }

            var validColor = new[] { "S", "K", "M" };

            var validCharacters = new[] { "1", "2", "3", "4", "5" };
            var characterTwo = input.Substring(1, 1);
            var characterOne = input.Substring(0, 1);
            if (!(validCharacters.Contains(characterTwo) && validColor.Contains(characterOne)))
            {
                Console.WriteLine(
                    "Card Tanımlamanız {S,K,M} Renk ve {1,2,3,4,5} rakamları sınırlıdır. Lütfen Geçerli bir kart belirterek tekrar deneyiniz...");
                return false;
            }

            return ValidateCard(input, playerIndex);


        }

        private static bool ValidateCard(string bigInput, int playerIndex)
        {
            var result = Gm.ValidateCard(_distributedDeck, playerIndex, bigInput);
            if (!result.Item1)
            {
                Console.WriteLine(result.Item2);

            }

            return result.Item1;
        }

    }
}
