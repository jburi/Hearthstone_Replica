using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public GameObject Canvas;
    private bool isDragging = false;
    private bool isOverDropZone = false;
    private GameObject playerZone;
    public GameObject startParent;
    public Vector2 startPosition;

    Complete.GameManager gm;

    private void Start()
    {
        gm = Complete.GameManager.gm;
        Canvas = GameObject.Find("Main Canvas");
        playerZone = gm.playerTable;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == playerZone)
        {
            isOverDropZone = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == playerZone)
        {
            isOverDropZone = false;
        }
    }

    public void StartDrag()
    {
        startParent = transform.parent.gameObject;
        startPosition = transform.position;

        if (!isOverDropZone)
        {
            isDragging = true;
        }
    }

    public void EndDrag()
    {
        isDragging = false;

        if (isOverDropZone)
        {
            gm.PlaceCard(gameObject);
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    //public void 

    public bool getDragging()
    {
        return isDragging;
    }
}
