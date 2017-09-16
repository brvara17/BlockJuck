using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCards
{
    /// <summary>
    /// A playing card
    /// </summary>
    public class Card
    {
        #region Fields

        Rank rank;
        Suit suit;
        bool faceUp;

        Rectangle drawRectangle;
        Texture2D cardFaceSprite;
        Texture2D cardBackSprite;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a card with the given rank and suit centered on the given x and y
        /// </summary>
        /// <param name="Content">the content manager for loading content</param>
        /// <param name="rank">the rank</param>
        /// <param name="suit">the suit</param>
        /// <param name="x">the x location</param>
        /// <param name="y">the y location</param>
        public Card(ContentManager Content, Rank rank, Suit suit, int x, int y)
        {
            this.rank = rank;
            this.suit = suit;
            faceUp = false;

            // Load our sprite.
            cardFaceSprite = Content.Load<Texture2D>(@"graphics/" + suit + @"/" + rank);
            cardBackSprite = Content.Load<Texture2D>(@"graphics/back");

            // Calculate our drawing rectangle.
            drawRectangle = new Rectangle(x - cardFaceSprite.Width / 2,
                y - cardFaceSprite.Height / 2, cardFaceSprite.Width,
                cardFaceSprite.Height);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the card rank
        /// </summary>
        public Rank Rank
        {
            get { return rank; }
        }

        /// <summary>
        /// Gets the card suit
        /// </summary>
        public Suit Suit
        {
            get { return suit; }
        }

        /// <summary>
        /// Gets whether or not the card is face up
        /// </summary>
        public bool FaceUp
        {
            get { return faceUp; }
        }

        /// <summary>
        /// Gets the width of the card
        /// </summary>
        public int Width
        {
            get { return drawRectangle.Width;  }
        }

        /// <summary>
        /// Sets the centered x location of the card.
        /// </summary>
        public int X
        {
            set { drawRectangle.X = value - cardFaceSprite.Width / 2; }
        }

        /// <summary>
        /// Sets the centered y location of the card.
        /// </summary>
        public int Y
        {
            set { drawRectangle.Y = value - cardFaceSprite.Height / 2; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Flips the card over
        /// </summary>
        public void FlipOver()
        {
            faceUp = !faceUp;
        }

        /// <summary>
        /// Draws the card
        /// </summary>
        /// <param name="spriteBatch">the sprite batch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (FaceUp)
                spriteBatch.Draw(cardFaceSprite, drawRectangle, Color.White);
            else
                spriteBatch.Draw(cardBackSprite, drawRectangle, Color.White);
        }
        #endregion
    }
}
