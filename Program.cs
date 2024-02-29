using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.IO;
class Program
{
    static void Main(string[] args)
    {
        var filename = "deckofcards.txt";
        Deck deck = new Deck();
        deck.Shuffle();
        for (int i = deck.Count - 1; i > 10; i--)
        deck.RemoveAt(i);
        deck.WriteCards(filename);

        Deck cardsToRead = new Deck(filename);
        foreach (var card in cardsToRead)
            Console.WriteLine(card.Name);


    }
}

class Card : IComparable<Card>
{
    public int CompareTo(Card other)
    {
        return new CardComparerByValue().Compare(this, other);
    }

    public Values Value { get; private set; }
    public Suits Suit { get; private set; }

    public Card(Values value, Suits suit)
    {
        this.Suit = suit;
        this.Value = value;
    }
    public string Name
    {
        get { return $"{Value} of {Suit}"; }
    }

    public override string ToString()
    {
        return Name;
    }

}

class Deck : ObservableCollection<Card>
{
    private static Random random = new Random();

    public Deck()
    {
        Reset();
    }

    public Card Deal(int index)
    {
        Card cardToDeal = base[index];
        RemoveAt(index);
        return cardToDeal;
    }

    public void Reset()
    {
        Clear();
        for (int suit = 0; suit <= 3; suit++)
            for (int value = 1; value <= 13; value++)
                Add(new Card((Values)value, (Suits)suit));
    }

    public Deck Shuffle()
    {
        List<Card> copy = new List<Card>(this);
        Clear();
        while (copy.Count > 0)
        {
            int index = random.Next(copy.Count);
            Card card = copy[index];
            copy.RemoveAt(index);
            Add(card);
        }

        return this;
    }

    public void Sort()
    {
        List<Card> sortedCards = new List<Card>(this);
        sortedCards.Sort(new CardComparerByValue());
        Clear();
        foreach (Card card in sortedCards)
        {
            Add(card);
        }
    }
    public void WriteCards(string filename)
    {
        using (var writer = new StreamWriter(filename))
        {
            for (int i = 0; i < Count; i++)
            {
                writer.WriteLine(this[i].Name);
            }
        }
    }
    public Deck(string filename)
    {
        using (var reader = new StreamReader(filename))
        {
            while (!reader.EndOfStream)
            {
                var nextCard = reader.ReadLine();
                var cardParts = nextCard.Split(new char[] { ' ' });
                var value = cardParts[0] switch
                {
                    "Ace" => Values.Ace,
                    "Two" => Values.Two,
                    "Three" => Values.Three,
                    "Four" => Values.Four,
                    "Five" => Values.Five,
                    "Six" => Values.Six,
                    "Seven" => Values.Seven,
                    "Eight" => Values.Eight,
                    "Nine" => Values.Nine,
                    "Ten" => Values.Ten,
                    "Jack" => Values.Jack,
                    "Queen" => Values.Queen,
                    "King" => Values.King,
                    _ => throw new InvalidDataException($"Unrecognized card value: {cardParts[0]}")
                };
                var suit = cardParts[2] switch
                {
                    "Spades" => Suits.Spades,
                    "Clubs" => Suits.Clubs,
                    "Hearts" => Suits.Hearts,
                    "Diamond" => Suits.Diamonds,
                    _ => throw new InvalidDataException($"Unrecognized card suit: {cardParts[2]}"),
                };
                Add(new Card(value, suit));

            }
        }
    }
}
enum Values
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
}
enum Suits
{
    Diamonds,
    Clubs,
    Hearts,
    Spades,
}
class CardComparerByValue : IComparer<Card>
{
    public int Compare(Card x, Card y)
    {
        if (x.Suit < y.Suit)
            return -1;
        if (x.Suit > y.Suit)
            return 1;
        if (x.Value < y.Value)
            return -1;
        if (x.Value > y.Value)
            return 1;
        return 0;
    }
}