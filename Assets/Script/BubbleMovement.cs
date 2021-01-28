using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMovement : MonoBehaviour
{

    public float moveSpeed;

    void Start() {
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
      transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}
