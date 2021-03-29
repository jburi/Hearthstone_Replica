using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class AIState : MonoBehaviour
{
	[SerializeField]
	public static List<AIState> AllStates = new List<AIState>();
	public static float TimePased;
	public AIState ParentState;
	public List<AIState> ChildsStatus = new List<AIState>();
	public int Index;
	public Player PlayerHero;
	public List<Card> PlayerTableCards = new List<Card>();

	public Player AIHero;
	public List<Card> AIHandCards = new List<Card>();
	public List<Card> AITableCards = new List<Card>();

	//Used to test output of ProducePlacing()
	public List<List<Card>> temp = new List<List<Card>>();		

	[System.Serializable]
	public class ListofCards
	{
		public List<Card> temp1 = new List<Card>();
	}
	public ListofCards loc = new ListofCards();
	public List<ListofCards> inspector = new List<ListofCards>();
	

	public int maxMana;
	public int PlayerMana;
	public int AIMana;


	public Complete.GameManager.Turn turn;
	public Complete.GameManager gm;

	public float State_Score = 0;
	float AI_Table_Score = 0;
	float Player_Table_Score = 0;
	float AI_Hand_Score = 0;
	float Player_Health_Score = 0;
	float AI_Health_Score = 0;

	float Attackweight;
	float Healthweight;
	float Manaweight;
	float HeroHealthweight;

	[System.Serializable]
	public struct Action
	{
		public string Card1;
		public string Card2;
		public string Hero;
		[SerializeField] public int OpCode;
	}
	public Queue<Action> Actions;

	public void Awake()
	{
		gm = Complete.GameManager.gm;
		Actions = new Queue<Action>();
		State_Score = 0;
		AI_Table_Score = 0;
		Player_Table_Score = 0;
		AI_Hand_Score = 0;
		Player_Health_Score = 0;
		AI_Health_Score = 0;

		Attackweight = 1;
		Healthweight = 1;
		Manaweight = 0.1f;
		HeroHealthweight = 0.1f;
	}
	public void NewState(
		List<Card> PlayerTable,
		List<Card> AIHand,
		List<Card> AITable,
		Player _PlayerHero,
		Player _AIHero,
		int _MaxMana,
		int _PlayerMana,
		int _AIMana,
		Complete.GameManager.Turn _Turn,
		AIState Parent
		)
	{
		ParentState = Parent;
		if (ParentState == null)
		{
			Index = 0;
		}
		else
		{
			Index = ParentState.Index + 1;
			Actions = new Queue<Action>(ParentState.Actions);
		}
		PlayerTableCards = CardListCopier.DeepCopy(PlayerTable);
		PlayerHero = _PlayerHero.Clone() as Player;

		AIHandCards = CardListCopier.DeepCopy(AIHand);
		AITableCards = CardListCopier.DeepCopy(AITable);
		AIHero = _AIHero.Clone() as Player;
		maxMana = _MaxMana;
		PlayerMana = _PlayerMana;
		AIMana = _AIMana;
		turn = _Turn;
		State_Score = Calculate_State_Score();
		//if (Index <= BoardBehaviourScript.instance.AILEVEL)
		//{
		//    GetAllPlacingAction();
		//    GetAllAttackingActions();
		//}
		AllStates.Add(this);
	}
	public void NewState2(
		List<GameObject> PlayerTable,
		List<GameObject> AIHand,
		List<GameObject> AITable,
		Player _PlayerHero,
		Player _AIHero,
		int _MaxMana,
		int _PlayerMana,
		int _AIMana,
		Complete.GameManager.Turn _Turn,
		AIState Parent
		)
	{
		ParentState = Parent;
		if (ParentState == null)
		{
			Index = 0;
		}
		else
		{
			Index = ParentState.Index + 1;
			Actions = new Queue<Action>(ParentState.Actions);
		}

		List<Card> _tempPlayerTable = new List<Card>();
		foreach (var item in PlayerTable) _tempPlayerTable.Add(item.GetComponent<CardDisplay>().card);
		PlayerTableCards = CardListCopier.DeepCopy(_tempPlayerTable);

		PlayerHero = _PlayerHero.Clone() as Player;

		List<Card> _tempAIHand = new List<Card>();
		foreach (var item in AIHand) _tempAIHand.Add(item.GetComponent<CardDisplay>().card);
		AIHandCards = CardListCopier.DeepCopy(_tempAIHand);

		List<Card> _tempAITable = new List<Card>();
		foreach (var item in AITable) _tempAITable.Add(item.GetComponent<CardDisplay>().card);
		AITableCards = CardListCopier.DeepCopy(_tempAITable);

		AIHero = _AIHero.Clone() as Player;
		maxMana = _MaxMana;
		PlayerMana = _PlayerMana;
		AIMana = _AIMana;
		turn = _Turn;
		State_Score = Calculate_State_Score();
		//if (Index<=BoardBehaviourScript.instance.AILEVEL)
		//{
		//    GetAllPlacingAction();
		//    GetAllAttackingActions();
		//}
		AllStates.Add(this);
	}
	float Calculate_State_Score()
	{
		State_Score = 0;
		AI_Table_Score = 0;
		Player_Table_Score = 0;
		AI_Hand_Score = 0;
		Player_Health_Score = 0;
		AI_Health_Score = 0;

		foreach (Card Card in AITableCards)
		{
			AI_Table_Score += Card.attack * Attackweight + Card.health * Healthweight;
			if (Card.cardType == CardSO.CardType.Magic && PlayerTableCards.Count > 0)
			{
				AI_Table_Score += 2;
			}
		}


		foreach (Card Card in PlayerTableCards)
		{
			Player_Table_Score -= Card.attack * Attackweight + Card.health * Healthweight;
		}

		//Depend On Mana
		foreach (Card Card in AIHandCards)
		{
			AI_Hand_Score += Card.manaCost * Manaweight;
		}


		if (PlayerHero.health <= 0) Player_Table_Score = float.MaxValue;
		else
			Player_Health_Score -= PlayerHero.health * HeroHealthweight;



		if (AIHero.health <= 0) AI_Health_Score = float.MinValue;
		else
			AI_Health_Score += AIHero.health * HeroHealthweight;

		State_Score = AI_Table_Score + Player_Table_Score + AI_Hand_Score + Player_Health_Score + AI_Health_Score;
		return State_Score;
	}

	public void GetAllPlacingActions()
	{
		if (turn == Complete.GameManager.Turn.AITurn)
		{
			if (AIHandCards.Count == 0)
			{
				Debug.Log("No Cards");
				//EndTurn Nothing To Play
			}
			else
			{
				//Generate All Possible Placing
				List<List<Card>> temp = ProducePlacing(AIHandCards);
				Debug.Log("ProducePlacing");
				for (int i = 0; i < temp.Count; i++)
				{
					AIState State = gameObject.AddComponent<AIState>();
					State.NewState(PlayerTableCards, AIHandCards, AITableCards, PlayerHero, AIHero, maxMana, PlayerMana, AIMana, turn, this);
					//if(temp[i].Count>0)
					for (int j = 0; j < temp[i].Count; j++)
					{
						Debug.LogWarning("Place Card");
						State.PlaceCard(temp[i][j]);

					}
					State.Calculate_State_Score();
					ChildsStatus.Add(State);
				}
			}
		}
		Debug.Log("DonePlacing");
	}
	public void PlaceCard(Card thisCard)
	{
		//
		//Find That Card
		//
		Card card = AIHandCards.Find(item => item.name == thisCard.name);
		GameObject cardObject = gm.AIHandCards.Find(item => item.GetComponent<CardDisplay>().card.name == thisCard.name);
		Debug.Log(thisCard.name);
		Debug.LogError(cardObject.name);
		//card.owner == Card.Owner.AI && 
		if (AIMana - card.manaCost >= 0 && AITableCards.Count < 10)
		{
			Debug.Log("Playing Card: " + card.name);
			AIHandCards.Remove(card);
			AITableCards.Add(card);
			Action a;
			a.Card1 = card.name;
			a.Card2 = "";
			a.Hero = "";
			a.OpCode = 0;
			Actions.Enqueue(a);
			//card.SetCardStatus(Card.CardStatus.OnTable);
			thisCard.cardStatus = Card.CardStatus.OnTable;
			if (card.cardType == Card.CardType.Magic)//Apply Magic Effect 
			{
				thisCard.canPlay = true;
				if (card.cardeffect == Card.CardEffect.ToAll)
				{
					card.AddToAll(card, false, delegate { Destroy(card); });
				}
				else if (card.cardeffect == Card.CardEffect.ToEnemies)
				{
					card.AddToEnemies(card, Complete.GameManager.gm.MyTableCards, false, delegate { Destroy(card); });
				}
			}

			AIMana -= card.manaCost;
		}
	}
	// Get All Attacking Actions (int AILEVEL)
	public void GetAllAttackingActions(int AILEVEL)
	{
		_GetAllAttackingActions();
		_GetAllAttackingHeroActions();
		TimePased += Time.deltaTime;
		if (Index + 1 < AILEVEL)
		{
			foreach (var item in ChildsStatus)
			{
				item.GetAllAttackingActions(AILEVEL);
			}


		}
		else
		{
			TimePased = 0;
		}
	}
	void _GetAllAttackingActions()
	{
		if (turn == Complete.GameManager.Turn.AITurn)
		{
			//Generate All Possible Placing
			List<List<Card>> temp = ProduceAllAttackCombinations(AITableCards);
			foreach (Card PlayerCard in PlayerTableCards)
			{
				for (int i = 0; i < temp.Count; i++)
				{
					AIState State = gameObject.AddComponent<AIState>();
					State.NewState(PlayerTableCards, AIHandCards, AITableCards, PlayerHero, AIHero, maxMana, PlayerMana, AIMana, turn, this);
					for (int j = 0; j < temp[i].Count; j++)
					{
						if (temp[i][j].canPlay)
						{
							if (temp[i][j].cardType == Card.CardType.Monster)
							{
								State.CardAttackCard(temp[i][j], PlayerCard);
							}
							else if (temp[i][j].cardType == Card.CardType.Magic)
							{
								if (temp[i][j].cardeffect == Card.CardEffect.ToSpecific)
								{
									State.CardMagicCard(temp[i][j], PlayerCard);
								}
							}
						}


					}
					State.Calculate_State_Score();
					ChildsStatus.Add(State);
				}
			}

		}
	}
	void _GetAllAttackingHeroActions()
	{
		List<List<Card>> temp = ProduceAllAttackCombinations(AITableCards);
		for (int i = 0; i < temp.Count; i++)
		{
			AIState State = gameObject.AddComponent<AIState>();
			State.NewState(PlayerTableCards, AIHandCards, AITableCards, PlayerHero, AIHero, maxMana, PlayerMana, AIMana, turn, this);
			for (int j = 0; j < temp[i].Count; j++)
			{
				if (temp[i][j].canPlay)
					State.CardAttackHero(temp[i][j], PlayerHero);

			}
			State.Calculate_State_Score();
			ChildsStatus.Add(State);
		}
	}
	private void CardMagicCard(Card _attacker, Card _target)
	{
		Card attacker = AITableCards.Find(item => item.name == _attacker.name);
		Card target = PlayerTableCards.Find(item => item.name == _target.name);
		{
			Action a;
			a.Card1 = attacker.name;
			a.Card2 = target.name;
			a.Hero = "";
			a.OpCode = 3;
			Actions.Enqueue(a);
			attacker.AddToMonster(attacker, target, false, delegate
			{
				//attacker.Destroy(attacker);
			});
		}
	}

	public void CardAttackCard(Card _attacker, Card _target)
	{
		Card attacker = AITableCards.Find(item => item.name == _attacker.name);
		Card target = PlayerTableCards.Find(item => item.name == _target.name);
		//if (attacker!=null&&target!=null)
		{
			Action a;
			a.Card1 = attacker.name;
			a.Card2 = target.name;
			a.Hero = "";
			a.OpCode = 1;
			Actions.Enqueue(a);
			attacker.AttackCard(attacker, target, false, delegate
			{
				attacker.canPlay = false;
			});
		}

	}
	public void CardAttackHero(Card _attacker, Player _target)
	{
		Card attacker = AITableCards.Find(item => item.name == _attacker.name);
		Action a;
		a.Card1 = attacker.name;
		a.Card2 = "";
		a.Hero = _target.name;
		a.OpCode = 2;
		Actions.Enqueue(a);
		attacker.AttackHero(attacker, PlayerHero, false, delegate
		{
			attacker.canPlay = false;
		});

	}
	

	/// <summary>
	/// Takes a list of cards and return a list of all the possible combonations
	/// eg (cardA, cardB) -> (cardA, cardB), (cardB), (cardA), (null)
	/// </summary>
	/// <param name="allValues"></param>
	/// <returns></returns>
	public List<List<Card>> ProducePlacing(List<Card> allValues)
	{
		List<List<Card>> collection = new List<List<Card>>();
		//if counter is less than the count of all the lists of lists. (Count * 2)
		for (int counter = 0; counter < (1 << allValues.Count); ++counter)
		{
			List<Card> combination = new List<Card>();
			for (int i = 0; i < allValues.Count; ++i)
			{
				if ((counter & (1 << i)) == 0)
					combination.Add(allValues[i]);
			}

			collection.Add(combination);
		}
		return collection;
	}
	public List<List<Card>> ProduceAllAttackCombinations(List<Card> allValues)
	{
		List<List<Card>> collection = new List<List<Card>>();
		//if counter is less than the count of all the lists of lists. (Count * 2)
		for (int counter = 0; counter < (1 << allValues.Count); ++counter)
		{
			List<Card> combination = new List<Card>();
			for (int i = 0; i < allValues.Count; ++i)
			{
				if ((counter & (1 << i)) == 0)
					combination.Add(allValues[i]);
			}

			// do something with combination
			collection.Add(combination);
		}
		return collection;
	}
	/*Test ProducePlacing()
	public void DoProducePlacing()
	{
		temp = ProducePlacing(PlayerTableCards);
		foreach(List<Card> list in temp)
		{
			ListofCards listofCards = new ListofCards();
			listofCards.temp1 = list;
			inspector.Add(listofCards);
		}
	}
	*/
	public static class CardListCopier
	{
		public static List<Card> DeepCopy(List<Card> objectToCopy)
		{
			List<Card> temp = new List<Card>();
			foreach (Card Card in objectToCopy)
			{
				temp.Add(Card.Clone());
			}
			return temp;
		}
	}
}
