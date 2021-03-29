using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardZoom : MonoBehaviour
{
    public Canvas Canvas;
    public GameObject zoomCard;

    // Start is called before the first frame update
    void Awake()
    {
        Canvas = GameObject.FindObjectOfType<Canvas>();
    }


    public void OnHoverEnter()
    {
        CardSO.Owner cardOwner;
        cardOwner = 0;

        if (gameObject.GetComponent<CardDisplay>() == true)
        {
            cardOwner = gameObject.GetComponent<CardDisplay>().card.owner;
        }

        if (cardOwner == 0)
        {
            zoomCard = Instantiate(gameObject, new Vector2((int) gameObject.transform.position.x * 2,(int) gameObject.transform.position.y * 2 + 300), Quaternion.identity);
            //Debug.Log("Card owner is: " + cardOwner);
        }
        else
        {
            zoomCard = Instantiate(gameObject, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 350), Quaternion.identity);
            //Debug.Log("Card owner is: " + cardOwner);
        }

        zoomCard.GetComponent<CardZoom>().enabled = false;
        zoomCard.transform.SetParent(Canvas.transform, false);
        zoomCard.layer = LayerMask.NameToLayer("Zoom");

        RectTransform rect = zoomCard.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnHoverExit()
    {
        Destroy(zoomCard);
    }
}
