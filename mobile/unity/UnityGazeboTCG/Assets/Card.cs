using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private SpriteRenderer rend;

    [SerializeField]
    private Sprite faceSprite, backSprite;

    private bool coroutineAllowed, facedUp;
    [SerializeField]
    private float spinTime = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = backSprite;
        coroutineAllowed = true;
        facedUp = false;
    }

    private void OnMouseDown()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(RotateCard());
        }
    }

    private IEnumerator RotateCard()
    {
        coroutineAllowed = false;

        if (!facedUp)
        {
            for (float i = 0f; i <= 180f; i += 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    rend.sprite = faceSprite;
                }
                yield return new WaitForSeconds(spinTime);
            }
        }

        else if (facedUp)
        {
            for (float i = 180f; i >= 0f; i -= 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    rend.sprite = backSprite;
                }
                yield return new WaitForSeconds(spinTime);
            }
        }

        coroutineAllowed = true;

        facedUp = !facedUp;
    }
}
