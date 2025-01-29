using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades,
    None
}

public class Card : MonoBehaviour
{
    private SpriteRenderer rend;
    [SerializeField]
    private Sprite faceSprite, backSprite;
    private bool coroutineAllowed, facedUp, isAnimating;

    public int value;
    public CardSuit suit;
    public int rank;

    private bool isDragging = false;
    private float startMouseX;
    private float startRotationY;
    
    private GameController gameController;
    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = backSprite;
        coroutineAllowed = true;
        facedUp = false;
    }

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public void MoveToPosition(Vector2 targetPos, float duration) {
        transform.DOMove(new Vector3(targetPos.x, targetPos.y, 0), duration);
    }

    public void MoveUp(float amount)
    {
        Vector2 currentPos = transform.position;
        MoveToPosition(new Vector2(currentPos.x, currentPos.y + amount), 0.3f);
    }

    public void FlipCard(float duration = 0.5f) {
        float currentRotation = transform.rotation.eulerAngles.y;
        float targetRotation = (currentRotation < 90 || currentRotation > 270) ? 180f : 0f;
        transform.DORotate(new Vector3(0, targetRotation, 0), duration);
    }

    public void SetRenderOrder(int order) {
        rend.sortingOrder = order;
    }

    // private void OnMouseDown()
    // {
    //     if (coroutineAllowed)
    //     {
    //         StartCoroutine(RotateCard());
    //     }
    // }

    void OnMouseDown()
    {
        gameController.SelectCard(this);
        // isDragging = true;
        // startMouseX = Input.mousePosition.x;
        // startRotationY = transform.eulerAngles.y;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            float difference = Input.mousePosition.x - startMouseX;
            float newRotation = startRotationY + difference;
            transform.rotation = Quaternion.Euler(0, newRotation, 0);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
       float yRotation = transform.rotation.eulerAngles.y % 360;

       if (yRotation < 90 || yRotation > 270) {
        rend.sprite = faceSprite;
       } else {
        rend.sprite = backSprite;
       }
    }

    // private IEnumerator RotateCard()
    // {
    //     coroutineAllowed = false;

    //     if (!facedUp)
    //     {
    //         for (float i = 0f; i <= 180f; i += 10f)
    //         {
    //             transform.rotation = Quaternion.Euler(0f, i, 0f);
    //             yield return new WaitForSeconds(0.01f);
    //         }
    //     }

    //     else if (facedUp)
    //     {
    //         for (float i = 180f; i >= 0f; i -= 10f)
    //         {
    //             transform.rotation = Quaternion.Euler(0f, i, 0f);
    //             yield return new WaitForSeconds(0.01f);
    //         }
    //     }

    //     facedUp = !facedUp;
    //     coroutineAllowed = true;
    // }
}
 