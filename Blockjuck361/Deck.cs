using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCards
{
    /// <summary>
    /// A deck of cards
    /// </summary>
    public class Deck
    {
        #region Fields

        List<Card> cards = new List<Card>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Deck(ContentManager Content, int x, int y)
        {
            // fill the deck with cards
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards.Add(new Card(Content, rank, suit, x, y));
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of cards in the deck.
        /// </summary>
        public int Count
        {
            get { return cards.Count; }
        }

        /// <summary>
        /// Gets whether or not the deck is empty
        /// </summary>
        public bool Empty
        {
            get { return cards.Count == 0; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Cuts the deck of cards at the given location
        /// </summary>
        /// <param name="location">the location at which to cut the deck</param>
        public void Cut(int location)
        {
            //int cutIndex = cards.Count - location;
            //List<Card> newCards = new List<Card>(cards.Count);
            //for (int i = cutIndex; i < cards.Count; i++)
            //{
            //    newCards.Add(cards[i]);
            //}
            //for (int i = 0; i < cutIndex; i++)
            //{
            //    newCards.Add(cards[i]);
            //}
            //cards = newCards;

            int cutIndex = cards.Count - location;
            Card[] newCards = new Card[cards.Count];
            cards.CopyTo(cutIndex, newCards, 0, location);
            cards.CopyTo(0, newCards, location, cutIndex);
            cards.Clear();
            cards.InsertRange(0, newCards);
        }

        /// <summary>
        /// Shuffles the deck
        /// 
        /// Reference: http://download.oracle.com/javase/1.5.0/docs/api/java/util/Collections.html#shuffle%28java.util.List%29
        /// </summary>
        public void Shuffle()
        {
            Random rand = new Random();
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int randomIndex = rand.Next(i + 1);
                Card tempCard = cards[i];
                cards[i] = cards[randomIndex];
                cards[randomIndex] = tempCard;
            }
        }

        /// <summary>
        /// Takes the top card from the deck. If the deck is empty, returns null
        /// </summary>
        /// <returns>the top card</returns>
        public Card TakeTopCard()
        {
            if (!Empty)
            {
                Card topCard = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                return topCard;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Draw the deck, which is really just drawing the top card on the deck.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            cards[cards.Count - 1].Draw(spriteBatch);
        }

        #endregion
    }
}