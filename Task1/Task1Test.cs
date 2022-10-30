using NUnit.Framework;
using static NUnit.Framework.Assert;
using static Task1.Task1;
using System.Linq;
using System.Collections.Specialized;
using System.Numerics;

namespace Task1;

public class Tests
{
    [Test]
    public void RoundWinnerTest()
    {
        var deck = FullDeck();
        
        foreach (Card card1 in deck)
        {
            foreach (Card card2 in deck)
            {
                if (card1.Rank > card2.Rank)
                    That(RoundWinner(card1, card2), Is.EqualTo(Player.Player1));
                else if (card1.Rank < card2.Rank)
                    That(RoundWinner(card1, card2), Is.EqualTo(Player.Player2));
                else 
                    That(RoundWinner(card1, card2), Is.EqualTo(null));
            }    
        }
    }

    [Test]
    public void FullDeckTest()
    {
        var deck = FullDeck();
        That(deck, Has.Count.EqualTo(DeckSize));
        foreach(Card card in deck)
        {
            That((deck.Where(card1 => card1 == card)).Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public void RoundTest()
    {
        Card six = new Card(Rank.Six, Suit.Diamonds);
        Card ace = new Card(Rank.Ace, Suit.Spades);
        Dictionary<Player, List<Card>> hands = new Dictionary<Player, List<Card>>
        {
            { Player.Player1, new List<Card> {six} },
            { Player.Player2, new List<Card> {ace} },
        };

        var tuple = Round(hands, 1);
        That(tuple.Item1, Is.EqualTo(Player.Player2));
        That(tuple.Item2[0], Is.EqualTo(six));
        That(tuple.Item2[1], Is.EqualTo(ace));
        That(tuple.Item2.Count, Is.EqualTo(2));
    }

    [Test]
    public void Game2CardsTest()
    {
        Card six = new Card(Rank.Six, Suit.Diamonds);
        Card ace = new Card(Rank.Ace, Suit.Spades);
        Dictionary<Player, List<Card>> hands = new Dictionary<Player, List<Card>>
        {
            { Player.Player1, new List<Card> {six} },
            { Player.Player2, new List<Card> {ace} },
        };

        var gameWinner = Game(hands, 1);
        That(gameWinner, Is.EqualTo(Player.Player2));
    }
    
}