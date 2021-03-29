using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSO : ScriptableObject {

	public new string name;
	public string description;

	public Sprite artwork;

	public int defaultAttack;
	public int attack;
	public int defaultHealth;
	public int health;
	public int manaCost;
	public bool canPlay = false;
	public delegate void CustomAction();

	public enum CardStatus { InDeck, InHand, OnTable, Destroyed };
	public CardStatus cardStatus = CardStatus.InDeck;
	public enum CardType { Monster, Magic };
	public CardType cardType;
	public enum CardEffect { ToAll, ToEnemies, ToSpecific };
	public CardEffect cardeffect;
	public int AddedHealth;
	public int AddedAttack;
	public enum Owner { My, AI };
	public Owner owner = Owner.My;

	public Vector3 newPos;
}