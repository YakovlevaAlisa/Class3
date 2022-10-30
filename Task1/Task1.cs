// Колода
using System;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Deck = System.Collections.Generic.List<Card>;
// Набор карт у игрока
using Hand = System.Collections.Generic.List<Card>;
// Набор карт, выложенных на стол
using Table = System.Collections.Generic.List<Card>;

// Масть
internal enum Suit
{
    Diamonds,
    Hearts,
    Clubs,
    Spades
}

// Значение
internal enum Rank
{
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14
}

// Карта
record Card(Rank Rank, Suit Suit);

// Тип для обозначения игрока (первый, второй)
internal enum Player
{
    Player1 = 1,
    Player2 = 2
}

namespace Task1
{
    public class Task1
    {

        /*
        * Реализуйте игру "Пьяница" (в простейшем варианте, на колоде в 36 карт)
        * https://ru.wikipedia.org/wiki/%D0%9F%D1%8C%D1%8F%D0%BD%D0%B8%D1%86%D0%B0_(%D0%BA%D0%B0%D1%80%D1%82%D0%BE%D1%87%D0%BD%D0%B0%D1%8F_%D0%B8%D0%B3%D1%80%D0%B0)
        * Рука — это набор карт игрока. Карты выкладываются на стол из начала "рук" и сравниваются
        * только по значениям (масть игнорируется). При равных значениях сравниваются следующие карты.
        * Набор карт со стола перекладывается в конец руки победителя. Шестерка туза не бьёт.
        *
        * Реализация должна сопровождаться тестами.
        */

        // Размер колоды
        internal const int DeckSize = 36;

        // Возвращается null, если значения карт совпадают
        internal static Player? RoundWinner(Card card1, Card card2)
        {
            return card1.Rank == card2.Rank ? null : card1.Rank > card2.Rank ? Player.Player1 : Player.Player2;
        }

        // Возвращает полную колоду (36 карт) в фиксированном порядке
        internal static Deck FullDeck()
        {
            Deck listCards = new Deck(DeckSize);

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    listCards.Add(new Card(rank, suit));
            }

            return listCards;
        }
        //случайная раздача карт одному игроку
        private static Hand Shuffle(Deck deck, Hand hand)
        {
            for (int i = 0; i < DeckSize / 2; i++)
            {
                int cardIndex = new Random().Next(0, deck.Count);

                hand.Add(deck[cardIndex]);
                deck.RemoveAt(cardIndex);
            }
            return hand;
        }

        // Раздача карт: случайное перемешивание (shuffle) и деление колоды пополам
        internal static Dictionary<Player, Hand> Deal(Deck deck)
        {
            var hands = new Dictionary<Player, Hand>();

            foreach(Player player in Enum.GetValues(typeof(Player)))
                hands.Add(player, new Hand());

            foreach (Player player in hands.Keys)
                Shuffle(deck, hands[player]);

            return hands;
        }

        // Один раунд игры (в том числе спор при равных картах).
        // Возвращается победитель раунда и набор карт, выложенных на стол.

        private static Card PutTheCardsOnTheTable(Hand playerHand, Table table)
        {
            Card playerCard = playerHand[0];

            table.Add(playerCard);
            playerHand.Remove(playerCard);

            return playerCard;
        }

        // Добавляет карты со стола в руку победителя, возвращает победителя.
        private static Player AddingСardsFromTheTable(Dictionary<Player, Hand> hands, Table table, Player? winner)
        {
            if (winner == null)
            {
                if (hands[Player.Player1].Count == 0)
                    winner = Player.Player2;

                if (hands[Player.Player2].Count == 0)
                    winner = Player.Player1;
            }

            for (int index = table.Count - 1; index >= 0; index--)
                hands[(Player)winner].Add(table[index]);

            return (Player)winner;
        }

        internal static Tuple<Player, Table> Round(Dictionary<Player, Hand> hands, int whichRound)
        {
            Player? winner = null;
            Table table = new Table();

            while (hands[Player.Player1].Count != 0 && hands[Player.Player2].Count != 0 && winner == null)
            {
                Card card1 = PutTheCardsOnTheTable(hands[(Player)((whichRound + 1) % 2 + 1)], table);
                Card card2 = PutTheCardsOnTheTable(hands[(Player)(whichRound % 2 + 1)], table);

                winner = (whichRound % 2 == 1) ? RoundWinner(card1, card2) : RoundWinner(card2, card1);
            }
            
            if (hands[Player.Player1].Count != 0 || hands[Player.Player2].Count != 0 || winner != null)
                winner = AddingСardsFromTheTable(hands, table, winner);
            else
                winner = Player.Player1;

            return new Tuple<Player, Table>((Player)winner, table);
        }

        // Полный цикл игры (возвращается победивший игрок)
        // в процессе игры печатаются ходы

        private static void Output(Dictionary<Player, Hand> hands, Tuple<Player, Table> tupleAfterRound,
                                    Player playerWhoLaidTheFirstCard, Player winner)
        {
            if (hands[Player.Player1].Count == 0 && hands[Player.Player2].Count == 0)
                Console.WriteLine($"Ничья ({playerWhoLaidTheFirstCard} положил первую карту).");
            else
                Console.WriteLine(winner + $" ({playerWhoLaidTheFirstCard} положил первую карту).");

            foreach (Card card in tupleAfterRound.Item2)
                Console.WriteLine(card);
            Console.WriteLine();
        }

        internal static Player Game(Dictionary<Player, Hand> hands, int whichRound)
        {
            Player winner = Player.Player1;

            while (hands[Player.Player1].Count != 0 && hands[Player.Player2].Count != 0)
            {
                var tupleAfterRound = Round(hands, whichRound);
                winner = tupleAfterRound.Item1;
                Player playerWhoLaidTheFirstCard = (whichRound % 2 == 1) ? Player.Player1 : Player.Player2;

                Output(hands, tupleAfterRound, playerWhoLaidTheFirstCard, winner);

                whichRound++;
            }
            return winner;
        }

        public static void Main(string[] args)
        {
            var deck = FullDeck();
            var hands = Deal(deck);
            var winner = Game(hands, 1);
            if (hands[Player.Player1].Count != 0 || hands[Player.Player2].Count != 0)
                Console.WriteLine($"Победитель: {winner}");
        }
    }
}