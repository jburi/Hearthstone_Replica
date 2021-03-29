using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        #region Turn Variables
        public enum Turn { MyTurn, AITurn };        // Turn enumerator
        public Turn turn;                           // Turn variable
        public float m_StartDelay = 3f;             // The delay between the start of PlayerTurnStarting and PlayerTurnActive phases.
        public float m_EndDelay = 3f;               // The delay between the end of PlayerTurnActive and PlayerTurnEnding phases.
        public float TurnTime = 30f;                // The time the player has on their turn.
        public GameObject endTurnObject;            // Slider to show the player how much time is left.
        private int turnNumber;                     // Which round the game is currently on.
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
        private EndTurn endTurn;                    // Reference to the EndTurn script.
        public bool endTurnButton;
        public bool AIDone;
        private Timer timer;                        // Reference to the Timer script.
        #endregion

        public int prog;

        #region Canvas Variables
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject MessageBox;               
        public Text m_DrawText;
        public GameObject DrawBox;
        public Text MyManaText;
        public Text AIManaText;
        public GameObject initialCanvas;            //Initial Draw Canvas
        public GameObject playerHand;
        public GameObject playerTable;
        public GameObject AIHand;
        public GameObject AITable;
        #endregion

        #region Decks
        public List<Card> MyDeckCards = new List<Card>();
        public List<GameObject> MyHandCards = new List<GameObject>();
        public List<GameObject> MyTableCards = new List<GameObject>();

        public List<Card> AIDeckCards = new List<Card>();
        public List<GameObject> AIHandCards = new List<GameObject>();
        public List<GameObject> AITableCards = new List<GameObject>();
		#endregion

		#region Players
		public Player Player;                    // A collection of managers for enabling and disabling different aspects of the tanks.
        public Player AIplayer;
        public int AILEVEL = 0;
        public bool enableTutorial;
        public bool initialDraw;
        int maxMana;
        public int MyMana = 0;
        int AIMana = 0;
        #endregion

        #region Managers
        public static GameManager gm;
        public GameObject AIStateCalculator;
        private GameTutorialManager tutorialManager;
        DrawCards drawCards;
        #endregion

        #region Attack Variables
        public GameObject currentCard;
        public GameObject targetCard;
        public Player currentHero;
        public Player targetHero;
        public LayerMask layer;
		#endregion

		public List<Hashtable> boardHistory = new List<Hashtable>();

        void Awake()
        {
            //Declare Singleton
            gm = this;
        }

        //Set Starting Data and the Game
        private void Start()
        {
            prog = 0;

            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);
            initialDraw = true;
            timer = gameObject.GetComponent<Timer>();

            //Set Start Data
            turn = Turn.MyTurn;
            maxMana = 1;
            MyMana = 1;
            AIMana = 1;
            endTurnButton = false;
            AIDone = false;

            //Managers
            tutorialManager = gameObject.GetComponent<GameTutorialManager>();
            drawCards = gameObject.GetComponent<DrawCards>();
            UpdateGame();

            //Slider
            endTurn = endTurnObject.GetComponent<EndTurn>();
            endTurnObject.SetActive(false);

            //Disable TextBoxes
            MessageBox.SetActive(false);
            DrawBox.SetActive(false);

            //Check if tutorial is enabled
            if (tutorialManager.GetTutorialStatus() == true)
            {
                StartCoroutine(tutorialManager.RunTutorial());
            }

            //Draw Cards for your hand
            InitialDraw();
        }

        public void AddHistory(GameObject a, GameObject b)
        {
            Hashtable hash = new Hashtable();

            hash.Add(a.name, b.name);

            boardHistory.Add(hash);
            currentCard = null;
            targetCard = null;
            currentHero = null;
            targetHero = null;
        }
        void UpdateGame()
        {
            MyManaText.text = MyMana.ToString() + "/" + maxMana;
            AIManaText.text = AIMana.ToString() + "/" + maxMana;

            if (Player.health <= 0)
                EndGame(Player);
            if (AIplayer.health <= 0)
                EndGame(AIplayer);
        }

		#region Initial Draw
		private void InitialDraw()
        {
            Debug.Log("Initial Draw");
            // As soon as the round starts reset the tanks and make sure they can't move.
            DisableCardControl();
            initialCanvas.SetActive(true);

            // Increment the round number and display text showing the players what round it is.
            m_DrawText.text = "Initial Drawing";
        }
        //Initial Draw Done Button
        public void OnClick()
        {
            initialCanvas.SetActive(false);
            initialDraw = false;
            drawCards.currentCard = 0;
            drawCards.oppCurrentCard = 0;
            StartCoroutine(GameLoop());
        }
        public void EndTurnButton()
        {
            if (timer)
            {
                endTurn.TakeDamage(timer.GetTime());
            }

            if (turn == Turn.MyTurn)
            {
                endTurnButton = true;
            }
            else endTurnButton = false;
        }
		#endregion

		#region Game Loop
		// This is called from start and will run each phase of the game one after another.
		private IEnumerator GameLoop()
        {
            //Start Player Turn
            yield return StartCoroutine(PlayerTurn());

            // Start AI Turn
            yield return StartCoroutine(AITurn());
            AIDone = false;
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            yield return StartCoroutine(GameLoop());
            
        }
        void NewTurn()
        {
            maxMana += (turnNumber) % 2;
            if (maxMana >= 10) maxMana = 10;
            MyMana = maxMana;
            AIMana = maxMana;
            turnNumber += 1;

            foreach (GameObject card in MyTableCards)
                card.GetComponent<CardDisplay>().card.canPlay = true;

            foreach (GameObject card in AITableCards)
                card.GetComponent<CardDisplay>().card.canPlay = true;
            Player.canAttack = true;
            AIplayer.canAttack = true;
        }

        #region Player Turn
        private IEnumerator PlayerTurn()
        {
            Debug.Log("Player Turn");
            turn = Turn.MyTurn;
            //endTurnButton = false;

            // Start off by running the 'PlayerTurnStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(PlayerTurnStarting());

            // Once the 'PlayerTurnStarting' coroutine is finished, run the 'PlayerTurnActive' coroutine but don't return until it's finished.
            yield return StartCoroutine(PlayerTurnActive());
        }

        // Display which turn it is and temp disable controls
        private IEnumerator PlayerTurnStarting()
        {
            if (DrawBox.activeSelf)
            {
                DrawBox.SetActive(false);
            }
            endTurnButton = false;
            MessageBox.SetActive(true);

            // As soon as the round starts reset the tanks and make sure they can't move.
            DisableCardControl();

            // Increment the round number and display text showing the players what round it is.
            NewTurn();
            m_MessageText.text = "Player Turn " + turnNumber;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }

        private IEnumerator PlayerTurnActive()
        {
            // As soon as the round begins playing let the players control the tanks.
            EnableCardControl();
            MessageBox.SetActive(false);

            if (turnNumber == 1)
            {
                for(int i = 0; i < 3; i++)
                {
                    drawCards.Draw();
                }
            }
            else
            {
                drawCards.Draw();
            }

            UpdateGame();

            //Start Timer
            timer.SetTime(30f);

            // While time is not out...
            while (timer.GetTime() >= 0)
            {
                if (timer.GetTime() < 10f && timer.GetTime() > 9f)
                {
                    endTurnObject.SetActive(true);
                }
                if (timer.GetTime() <= 10f)
                {
                    //Display Time Left
                    endTurn.TakeDamage(timer.GetTime());
                }
                if (endTurnButton) break;

                // ... return on the next frame.
                yield return null;
            }
            yield return null;
        }
        #endregion

        #region AI Turn
        private IEnumerator AITurn()
        {
            Debug.Log("AI Turn");
            turn = Turn.AITurn;

            // Start off by running the 'PlayerTurnStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(AITurnStarting());

            // Once the 'PlayerTurnStarting' coroutine is finished, run the 'PlayerTurnActive' coroutine but don't return until it's finished.
            yield return StartCoroutine(AITurnActive());
        }
        private IEnumerator AITurnStarting()
        {
            // Get a message based on the scores and whether or not there is a game winner and display it.
            MessageBox.SetActive(true);

            // As soon as the round starts reset the tanks and make sure they can't move.
            DisableCardControl();

            // Increment the round number and display text showing the players what round it is.
            NewTurn();
            m_MessageText.text = "AI Turn " + turnNumber;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }
        private IEnumerator AITurnActive()
        {
            MessageBox.SetActive(false);
            //Debug.Log("AIActive");
            UpdateGame();
            AIthink();
            yield return null;
        }
		#endregion
		
		#region AI Think
		private void AIthink()
        {
            AIState.AllStates.Clear();
            AIGetPlacing();
            AIGetAttacks();
            EndTurnButton();
            endTurn.TakeDamage(timer.GetTime());
            AIDone = true;
        }
        public GameObject CreateAICalculator()
        {
            Transform newParent = AIStateCalculator.transform;
            GameObject newAIState = new GameObject();
            newAIState.transform.SetParent(newParent);
            newAIState.AddComponent<AIState>();
            newAIState.name = "prog " + prog.ToString();
            return newAIState;
        }
        void AIGetPlacing()
        {
            GameObject Calculator = CreateAICalculator();
            AIState InitialState = Calculator.GetComponent<AIState>();
            InitialState.name = InitialState.Index.ToString();
            InitialState.NewState2(MyTableCards, AIHandCards, AITableCards, Player, AIplayer, maxMana, MyMana, AIMana, turn, null);
            InitialState.GetAllPlacingActions();
            //Find Best Score
            float MaxScore = float.MinValue;
            GameObject BestStateCalc = CreateAICalculator();
            AIState BestState = BestStateCalc.GetComponent<AIState>();
            foreach (AIState item in AIState.AllStates)
            {
                if (item.State_Score > MaxScore)
                {
                    MaxScore = item.State_Score;
                    BestState = item;
                }
            }
            BestState.name = "Best";
            int count = BestState.Actions.Count;
            //Place count amount of Cards as GameObjects
            for (int i = 0; i < count; i++)
            {
                AIState.Action a;
                a = BestState.Actions.Dequeue();
                if (a.OpCode == 0)
                {
                    foreach (var item in AIHandCards)
                    {
                        if (item.GetComponent<CardDisplay>().card.name == a.Card1)
                        {
                            Debug.LogError("GM playing: " + a.Card1);
                            PlaceCard(item);
                            break;
                        }
                    }
                }
            }
            
            AIState.AllStates.Clear();
        }        
        void AIGetAttacks()
        {
            AIState InitialState = AIStateCalculator.AddComponent<AIState>();
            InitialState.NewState2(MyTableCards, AIHandCards, AITableCards, Player, AIplayer, maxMana, MyMana, AIMana, turn, null);
            InitialState.GetAllAttackingActions(AILEVEL);
            //Find Best Score
            float MaxScore = float.MinValue;
            AIGameState BestState = new AIGameState();
            foreach (AIGameState item in AIGameState.AllStates)
            {
                if (item.State_Score > MaxScore)
                {
                    MaxScore = item.State_Score;
                    BestState = item;
                }
            }
            //Debug.Log("Best choice Index" + BestState.Index);
            int count = BestState.Actions.Count;
            //GetActions
            for (int i = 0; i < count; i++)
            {
                AIGameState.Action a;
                a = BestState.Actions.Dequeue();
                if (a.OpCode == 1)
                {
                    foreach (var item in AITableCards)//Find Card1
                    {
                        if (item.GetComponent<CardDisplay>().card.name == a.Card1)
                        {
                            currentCard = item;
                            break;
                        }
                    }
                    foreach (var item in MyTableCards)//Find Card2
                    {
                        if (item.GetComponent<CardDisplay>().card.name == a.Card2)
                        {
                            targetCard = item;
                            break;
                        }
                    }
                    if (currentCard != null && targetCard != null)//MakeAction
                    {
                        currentCard.GetComponent<CardDisplay>().card.AttackCard(currentCard.GetComponent<CardDisplay>().card, targetCard.GetComponent<CardDisplay>().card, true, delegate
                        {
                            currentCard.GetComponent<CardDisplay>().card.canPlay = false;
                        });
                    }
                }
                else if (a.OpCode == 2)
                {
                    foreach (var item in AITableCards)//Find Card1
                    {
                        if (item.GetComponent<CardDisplay>().card.name == a.Card1)
                        {
                            currentCard = item;
                            break;
                        }
                    }
                    if (a.Hero == "MyHero")
                    {
                        //targetHero = MyHero;
                    }
                    if (currentCard != null && targetHero != null)
                    {
                        //currentCard.AttackHero(currentCard, MyHero, true, delegate
                        //{
                            //currentCard.canPlay = false;
                        //});
                    }
                }
                else if (a.OpCode == 3)
                {
                    foreach (var item in AITableCards)//Find Card1
                    {
                        if (item.GetComponent<CardDisplay>().card.name == a.Card1)
                        {
                            currentCard = item;
                            break;
                        }
                    }
                    foreach (var item in MyTableCards)//Find Card2
                    {
                        if (item.GetComponent<CardDisplay>().card.name == a.Card2)
                        {
                            targetCard = item;
                            break;
                        }
                    }
                    if (currentCard != null && targetCard != null)//MakeAction
                    {
                        currentCard.GetComponent<CardDisplay>().card.AddToMonster(currentCard.GetComponent<CardDisplay>().card, targetCard.GetComponent<CardDisplay>().card, true, delegate
                        {
                            currentCard.GetComponent<CardDisplay>().Destroy(currentCard.GetComponent<CardDisplay>().card);
                        });
                    }
                }
            }
            //AIGameState.AllStates=new List< AIGameState > ();
        }
        public void PlaceCard(GameObject _card)
        {
            CardDisplay card = _card.GetComponentInChildren<CardDisplay>();
            DragDrop dragDrop = _card.GetComponent<DragDrop>();

            if (card.card.owner == Card.Owner.My && MyMana - card.card.manaCost >= 0 && MyTableCards.Count < 10)
            {
                //Move the Card
                _card.transform.SetParent(playerTable.transform, false);

                MyHandCards.Remove(_card);
                MyTableCards.Add(_card);

                card.currentState = CardDisplay.State.board;
                //PlaySound(cardDrop);

                if (card.card.cardType == Card.CardType.Magic)///Apply Magic Effect 
                {
                    card.card.canPlay = true;
                    if (card.card.cardeffect == Card.CardEffect.ToAll)
                    {
                        card.card.AddToAll(card.card, true, delegate { card.Destroy(card.card); });
                    }
                    else if (card.card.cardeffect == Card.CardEffect.ToEnemies)
                    {
                        card.card.AddToEnemies(card.card, AITableCards, true, delegate { card.Destroy(card.card); });
                    }
                }

                MyMana -= card.card.manaCost;
            }
            else if (card.card.owner == Card.Owner.My)
            {
                _card.transform.position = dragDrop.startPosition;
                _card.transform.SetParent(dragDrop.startParent.transform, false);
            }
            else if (card.card.owner == Card.Owner.AI && AIMana - card.card.manaCost >= 0 && AITableCards.Count < 10)
            {
                //Move the Card
                _card.transform.SetParent(AITable.transform, false);

                AIHandCards.Remove(_card);
                AITableCards.Add(_card);

                //card.SetCardStatus(Card.CardStatus.OnTable);
                card.currentState = CardDisplay.State.board;
                //PlaySound(cardDrop);

                if (card.card.cardType == Card.CardType.Magic)///Apply Magic Effect 
                {
                    card.canPlay = true;
                    if (card.card.cardeffect == Card.CardEffect.ToAll)
                    {
                        card.card.AddToAll(card.card, true, delegate { card.Destroy(card.card); });
                    }
                    else if (card.card.cardeffect == Card.CardEffect.ToEnemies)
                    {
                        card.card.AddToEnemies(card.card, MyTableCards, true, delegate { card.Destroy(card.card); });
                    }
                }

                AIMana -= card.card.manaCost;
            }

            UpdateGame();
        }
        #endregion

        #endregion

        #region Win Condition Funtions
        public void EndGame(Player winner)
        {
            if (winner == Player)
            {
                Debug.Log("Player");
                Time.timeScale = 0;
                EndMessage("You Won");
                //Destroy(this);
            }

            if (winner == AIplayer)
            {
                Time.timeScale = 0;
                Debug.Log("AIPlayer");
                EndMessage("You Losse");
                //Destroy(this);
            }
        }
        // This function is to find out if there is a winner of the game.
        private Player GetGameWinner()
        {
            

            // If no tanks have enough rounds to win, return null.
            return null;
        }


        // Display a message in the main textbox.
        private void EndMessage(string msg)
        {
            MessageBox.SetActive(true);

            // By default when a round ends there are no winners so the default end message is a draw.
            m_MessageText.text = msg;
        }

		#endregion

		#region Card Control
		private void EnableCardControl()
        {
            Player.EnableControl();
        }


        private void DisableCardControl()
        {
            Player.DisableControl();
        }

		#endregion
	}
}