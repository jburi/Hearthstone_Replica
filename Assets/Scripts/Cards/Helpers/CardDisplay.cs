using UnityEngine;
using UnityEngine.UI;


public class CardDisplay : MonoBehaviour {

	public enum State {	hand, board, graveyard }
	public State currentState;

	public Card card;

	public Text nameText;
	public Text descriptionText;

	public UnityEngine.UI.Image artworkImage;

	public Text manaText;
	public Text attackText;
	public Text healthText;

	public bool canPlay = false;
	public delegate void CustomAction();

	// Use this for initialization
	void Start () {
		
		nameText.text = card.name;
		descriptionText.text = card.description;

		artworkImage.sprite = card.artwork;

		manaText.text = card.manaCost.ToString();
		attackText.text = card.attack.ToString();
		healthText.text = card.health.ToString();
	}

	private void Update()
	{
		if (currentState == State.board)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Destroy(card);
			}
		}
	}
	public void Destroy(Card card)
	{
		if (card.owner == Card.Owner.My)
			Complete.GameManager.gm.MyTableCards.Remove(gameObject);
		else if (card.owner == Card.Owner.AI)
			Complete.GameManager.gm.AITableCards.Remove(gameObject);

		//Complete.GameManager.gm.PlaySound(BoardBehaviourScript.instance.cardDestroy);
		Destroy(gameObject);

		//Complete.GameManager.gm.TablePositionUpdate();
	}
}
