using System.Collections.Generic;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaCards;

namespace Blockjuck361
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WindowWidth = 800;
        const int WindowHeight = 600;

        // max valid blockjuck score for a hand
        const int MaxHandValue = 21;

        // deck and hands
        Deck deck;
        List<Card> dealerHand = new List<Card>();
        List<Card> playerHand = new List<Card>();

        // hand placement
        const int TopCardOffset = 100;
        const int HorizontalCardOffset = 150;
        const int VerticalCardSpacing = 125;

        // messages
        SpriteFont messageFont;
        const string ScoreMessagePrefix = "Score: ";
        Message playerScoreMessage;
        Message dealerScoreMessage;
        Message winnerMessage;
		List<Message> messages = new List<Message>();

        // message placement
        const int ScoreMessageTopOffset = 25;
        const int HorizontalMessageOffset = HorizontalCardOffset;
        Vector2 winnerMessageLocation = new Vector2(WindowWidth / 2,
            WindowHeight / 2);
        Vector2 dealerScoreLocation = new Vector2(WindowWidth - HorizontalMessageOffset, ScoreMessageTopOffset);
        Vector2 quitMenuLocation = new Vector2(HorizontalMenuButtonOffset, QuitMenuButtonOffset);

        // menu buttons
        Texture2D quitButtonSprite;
        Texture2D standButtonSprite;
        Texture2D hitButtonSprite;
        MenuButton standMenuButton;
        MenuButton quitMenuButton;
        MenuButton hitMenuButton;
        List<MenuButton> menuButtons = new List<MenuButton>();

        // menu button placement
        const int TopMenuButtonOffset = TopCardOffset;
        const int QuitMenuButtonOffset = WindowHeight - TopCardOffset;
        const int HorizontalMenuButtonOffset = WindowWidth / 2;
        const int VertcalMenuButtonSpacing = 125;

        // use to detect hand over when player and dealer didn't hit
        bool playerHit = false;
        bool dealerHit = false;

        // game state tracking
        static GameState currentState = GameState.WaitingForPlayer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution and show mouse
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create and shuffle deck
            deck = new Deck(Content, 0, 0);
            deck.Shuffle();

            // first player card
            Deal(Players.Player, true);

            // first dealer card
            Deal(Players.Dealer, false);


            // second player card
            Deal(Players.Player, true);

            // second dealer card
            Deal(Players.Dealer, true);



            // load sprite font, create message for player score and add to list
            messageFont = Content.Load<SpriteFont>(@"fonts\Arial24");
            playerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(playerHand).ToString(),
                messageFont,
                new Vector2(HorizontalMessageOffset, ScoreMessageTopOffset));

            messages.Add(playerScoreMessage);

            // load quit button sprite for later use
			quitButtonSprite = Content.Load<Texture2D>(@"graphics\quitbutton");

            // create hit button and add to list
            hitButtonSprite = Content.Load<Texture2D>(@"graphics\hitbutton");
            hitMenuButton = new MenuButton(
                hitButtonSprite,
                new Vector2(HorizontalMenuButtonOffset, TopMenuButtonOffset),
                GameState.PlayerHitting);

            menuButtons.Add(hitMenuButton);

            // create stand button and add to list
            standButtonSprite = Content.Load<Texture2D>(@"graphics\standbutton");
            standMenuButton = new MenuButton(
                standButtonSprite,
                new Vector2(HorizontalMenuButtonOffset, VertcalMenuButtonSpacing + TopMenuButtonOffset),
                GameState.WaitingForDealer);

            menuButtons.Add(standMenuButton);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Get mouse state to use the MenuButton update
            MouseState mouseState = Mouse.GetState();

            //Update each menu button respective of the current game state
            foreach (MenuButton menuButton in menuButtons)
            {
                if (currentState == GameState.WaitingForPlayer ||
                    currentState == GameState.DisplayingHandResults)
                {
                    menuButton.Update(mouseState);
                }
            }

            // game state-specific processing
            TableGameState();


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Goldenrod);
						
            spriteBatch.Begin();

            // draw hands
            foreach(Card card in dealerHand)
            {
                card.Draw(spriteBatch);
            }

            foreach(Card card in playerHand)
            {
                card.Draw(spriteBatch);
            }

            // draw messages
            foreach(Message message in messages)
            {
                message.Draw(spriteBatch);
            }

            // draw menu buttons
            foreach(MenuButton menuButton in menuButtons)
            {
                menuButton.Draw(spriteBatch);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates the Blockjuck score for the given hand
        /// </summary>
        /// <param name="hand">the hand</param>
        /// <returns>the Blockjuck score for the hand</returns>
        private int GetBlockjuckScore(List<Card> hand)
        {
            // add up score excluding Aces
            int numAces = 0;
            int score = 0;
            foreach (Card card in hand)
            {
                if (card.Rank != Rank.Ace)
                {
                    score += GetBlockjuckCardValue(card);
                }
                else
                {
                    numAces++;
                }
            }

            // if more than one ace, only one should ever be counted as 11
            if (numAces > 1)
            {
                // make all but the first ace count as 1
                score += numAces - 1;
                numAces = 1;
            }

            // if there's an Ace, score it the best way possible
            if (numAces > 0)
            {
                if (score + 11 <= MaxHandValue)
                {
                    // counting Ace as 11 doesn't bust
                    score += 11;
                }
                else
                {
                    // count Ace as 1
                    score++;
                }
            }

            return score;
        }

        /// <summary>
        /// Gets the Blockjuck value for the given card
        /// </summary>
        /// <param name="card">the card</param>
        /// <returns>the Blockjuck value for the card</returns>
        private int GetBlockjuckCardValue(Card card)
        {
            switch (card.Rank)
            {
                case Rank.Ace:
                    return 11;
                case Rank.King:
                case Rank.Queen:
                case Rank.Jack:
                case Rank.Ten:
                    return 10;
                case Rank.Nine:
                    return 9;
                case Rank.Eight:
                    return 8;
                case Rank.Seven:
                    return 7;
                case Rank.Six:
                    return 6;
                case Rank.Five:
                    return 5;
                case Rank.Four:
                    return 4;
                case Rank.Three:
                    return 3;
                case Rank.Two:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        /// <param name="newState">the new game state</param>
        public static void ChangeState(GameState newState)
        {
            currentState = newState;
        }
        
        /// <summary>
        /// This function determines the next state of the game depending on the current state
        /// of the game. 
        /// </summary>
        private void TableGameState()
        {
            switch(currentState)
            {
                //Check the conditions of dealer hand and act accordingly
                case GameState.WaitingForDealer:
                    {
                        if (GetBlockjuckScore(dealerHand) <= 16)
                        {
                            currentState = GameState.DealerHitting;
                        }
                        else
                        {
                            currentState = GameState.CheckingHandOver;
                        }
                    }
                    break;

                    //Dealer is currently hitting
                case GameState.DealerHitting:
                    {
                        Deal(Players.Dealer, true);
                        dealerHit = true;
                        currentState = GameState.CheckingHandOver;                        
                    }
                    break;

                    //Player is currently hitting
                case GameState.PlayerHitting:
                    {
                        //Deal card to player and print player score
                        Deal(Players.Player, true);
                        playerScoreMessage.Text = ScoreMessagePrefix + GetBlockjuckScore(playerHand).ToString();
                        playerHit = true;
                        currentState = GameState.WaitingForDealer;
                       
                    }
                    break;

                // Get Logic
                case GameState.CheckingHandOver:
                    {
                        //Text to display winner
                        string winnerText;
                        
                        //if both players "stand"
                        if (!dealerHit && !playerHit)
                        {
                            //Check each players hand and output result
                            if (GetBlockjuckScore(playerHand) > GetBlockjuckScore(dealerHand))
                            {
                                winnerText = "Player Wins!";
                            }
                            else if (GetBlockjuckScore(dealerHand) == GetBlockjuckScore(playerHand))
                            {
                                winnerText = "Tie!";
                            }
                            else
                            {
                                winnerText = "Dealer Wins!";
                            }

                            //Output results and change display options
                            Results(winnerText);
                        }
                        else
                        {
                            //Checks to see if any player hit and busted
                            if (playerHit && Busted(playerHand) || dealerHit && Busted(dealerHand))
                            {
                                //Player busted, dealer wins or dealer busted, player wins
                                if (playerHit && Busted(playerHand))
                                {
                                    winnerText = "Dealer Wins!";
                                }
                                else
                                {
                                    winnerText = "Player Wins!";
                                }

                                //Output results and change display options
                                Results(winnerText);

                            }
                           
                            //Reset players "hit" flag and wait for play  
                            playerHit = false;
                            dealerHit = false;
                            currentState = GameState.WaitingForPlayer;

                        }
                        
                    }
                    break;

                    //Exists the game
                case GameState.Exiting:
                    Exit();
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// This function dispalys the results of the game 
        /// </summary>
        /// <param name="winnerText"></param>
        private void Results(string winnerText)
        {
            //Flip the first card of the dealer
            dealerHand[0].FlipOver();

            //Display the message for the winner
            winnerMessage = new Message(winnerText, messageFont, winnerMessageLocation);
            messages.Add(winnerMessage);

            //Display the score for the dealer
            dealerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(dealerHand).ToString(),
                messageFont, dealerScoreLocation);
            messages.Add(dealerScoreMessage);

            //Remove the stand and hit button to replace with quit
            menuButtons.Clear();

            //Add quit menu button to the list to update display options
            quitMenuButton = new MenuButton(quitButtonSprite, quitMenuLocation, GameState.Exiting);
            menuButtons.Add(quitMenuButton);

            //Change state
            currentState = GameState.DisplayingHandResults;
        }


        /// <summary>
        /// This function checks to see if the appropriate player's hand 
        /// has busted or not.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private bool Busted(List<Card> hand)
        {   
            //Busted or not
            if (GetBlockjuckScore(hand) > MaxHandValue)
                return true;
            else
                return false;
        }

        /// <summary>
        /// The function deals the cards to the appropriate players, sets display the display
        /// of the card face up or down and sets the position for the card. 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="faceUp"></param>
        private void Deal(Players player, bool faceUp)
        {

            //Get top card
            Card card = deck.TakeTopCard();

            //Flips card
            if (faceUp)
                card.FlipOver();

            //Check which player is getting dealt the card, and set the card in correct location
            //respective of the player
            if (player == Players.Player)
            {
                card.X = HorizontalCardOffset;
                card.Y = (VerticalCardSpacing * playerHand.Count) + TopCardOffset;
                playerHand.Add(card);
            }
            else
            {
                card.X = WindowWidth - HorizontalCardOffset;
                card.Y = card.Y = (VerticalCardSpacing * dealerHand.Count) + TopCardOffset;
                dealerHand.Add(card);
            }
        }

           
    }
}
