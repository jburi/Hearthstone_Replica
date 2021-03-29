using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : CardSO
{
	public void SetCardStatus(CardStatus status)
	{
		cardStatus = status;
	}
	public void AttackCard(Card attacker, Card target, bool addhistory, CustomAction action)
	{
		if (attacker.canPlay)
		{
			target.health -= attacker.attack;
			attacker.health -= target.attack;

			if (target.health <= 0)
			{
				Destroy(target);
			}

			if (attacker.health <= 0)
			{
				//attacker.Destroy(attacker);
			}

			action();
			//if (addhistory)
				//BoardBehaviourScript.instance.AddHistory(attacker, target);
		}
	}//Attack
	public void AttackHero(Card attacker, Player target, bool addhistory, CustomAction action)
	{
		if (attacker.canPlay)
		{
			target.health -= attacker.attack;
			attacker.health -= target.attack;

			action();
			//if (addhistory)
				//BoardBehaviourScript.instance.AddHistory(attacker, target);
		}
	}//Attack
	public void AddToHero(Card magic, Player target, CustomAction action)
	{
		if (magic.canPlay)
		{
			target.attack += magic.AddedAttack;
			if (target.health + magic.AddedHealth <= 30)
				target.health += magic.AddedHealth;
			else
				target.health = 30;
			action();
			//Complete.GameManager.gm.AddHistory(magic, target);
		}
	}//Magic
	public void AddToMonster(Card magic, Card target, bool addhistory, CustomAction action)
	{
		if (magic.canPlay)
		{
			target.attack += magic.AddedAttack;
			target.health += magic.AddedHealth;
			action();
			//if (addhistory)
				//Complete.GameManager.gm.AddHistory(magic, target);
		}
	}//Magic
	public void AddToAll(Card magic, bool addhistory, CustomAction action)
	{
		if (magic.canPlay)
		{
			foreach (var target in Complete.GameManager.gm.AITableCards)
			{
				AddToMonster(magic, target.GetComponent<Card>(), addhistory, delegate { });
			}
			foreach (var target in Complete.GameManager.gm.MyTableCards)
			{
				AddToMonster(magic, target.GetComponent<Card>(), addhistory, delegate { });
			}
			action();
		}
	}//Magic
	public void AddToEnemies(Card magic, List<GameObject> targets, bool addhistory, CustomAction action)
	{
		if (magic.canPlay)
		{
			foreach (var target in targets)
			{
				AddToMonster(magic, target.GetComponent<Card>(), addhistory, delegate { });
			}
			action();
		}
	}//Magic
	public void AddToEnemies(Card magic, List<Card> targets, bool addhistory, CustomAction action)
	{
		if (magic.canPlay)
		{
			foreach (var target in targets)
			{
				AddToMonster(magic, target, addhistory, delegate { });
			}
			action();
		}
	}//Magic
	public Card Clone()
	{
		Card temp = ScriptableObject.CreateInstance<Card>();
		temp.name = this.name;
		temp.description = this.description;
		temp.artwork = this.artwork;
		temp.defaultAttack = this.defaultAttack;
		temp.attack = this.attack;
		temp.defaultHealth = this.defaultHealth;
		temp.health = this.health;
		temp.manaCost = this.manaCost;
		temp.canPlay = this.canPlay;
		temp.cardStatus = this.cardStatus;
		temp.cardStatus = this.cardStatus;
		temp.cardeffect = this.cardeffect;
		temp.AddedHealth = this.AddedHealth;
		temp.AddedAttack = this.AddedAttack;
		temp.owner = this.owner;
		temp.newPos = this.newPos;

		return temp as Card;
	}
}
