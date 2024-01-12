using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveOnMouseClick2D : MonoBehaviour
{
    public float moveSpeed = 2f;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is left mouse, 1 is right mouse
        {
            //Get mouse pos in screen
            Vector3 mousePosition = Input.mousePosition;

            // Trans mouse pos from screen to game world
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // trans GameObject to mouse pos
            StartCoroutine(MoveSmoothly(targetPosition));
        }
    }

    private IEnumerator MoveSmoothly(Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 startingPosition = transform.position;
        //limit targetPos
        if (targetPosition.x > 12f) targetPosition.x = 12f;
        else if (targetPosition.x < -20f) targetPosition.x = -20f;

        if (targetPosition.y > 53f) targetPosition.y = 53f;
        else if (targetPosition.y < 3f) targetPosition.y = 3f;

        while (elapsedTime < 1f)
        {
            // cal new pos
            transform.position = Vector2.Lerp(startingPosition, targetPosition, elapsedTime);

            // increase elapsedTime
            elapsedTime += Time.deltaTime * moveSpeed;

            // wait
            yield return null;
        }

        
        transform.position = targetPosition;
    }
}
