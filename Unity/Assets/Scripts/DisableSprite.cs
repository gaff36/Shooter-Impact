using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSprite : MonoBehaviour
{
    private SpriteRenderer renderer;

    void Start()
    {

        renderer = GetComponent<SpriteRenderer>();
        renderer.gameObject.SetActive(false);   
    }

   
}
