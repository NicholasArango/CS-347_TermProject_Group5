using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [Header("Inscribed")]
    // Time in sec to move between grid positions
    public float moveDuration = 0.1f;
    // Size of the grid
    public float gridSize = 1f;
    // Grid border
    public float gridBorder = 4.5f;

    private bool isMoving = false;

    private TurnCounter turnCounter;

    // Start is called before the first frame update
    void Start()
    {
        //Find TurnCounter
        GameObject turnGO = GameObject.Find("TurnCounter");
        turnCounter = turnGO.GetComponent<TurnCounter>();

    }

    // Update is called once per frame
    void Update()
    {
        // Only process one move at a time
        if (!isMoving)
        {
            Vector2 currentPos = transform.position;

            //if input is active, move in appropriate direction (only registers first press, will not continuosly move with one long press)
            //(also make sure player doesn't go out of bounds here)

            if (Input.GetKeyDown(KeyCode.UpArrow) && currentPos.y < gridBorder)
            {
                StartCoroutine(Move(Vector2.up));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentPos.y > -gridBorder)
            {
                StartCoroutine(Move(Vector2.down));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentPos.x > -gridBorder)
            {
                StartCoroutine(Move(Vector2.left));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && currentPos.x < gridBorder)
            {
                StartCoroutine(Move(Vector2.right));
            }
        }
    }

    // Smooth movement between grid positions
    private IEnumerator Move(Vector2 direction)
    {
        // Record that we're moving, don't accept more inputs
        isMoving = true;

        // If obstacle is in the way, stop coroutine (make sure obstacles that you want to stop player are in the obstacles layer!)
        LayerMask mask = LayerMask.GetMask("obstacles");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, gridSize, mask);
        if (hit)
        {
            isMoving = false;
            yield break;
        }

        //increment turn counter
        turnCounter.turns += 1;

        // Record where we are and where we're going
        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + (direction * gridSize);

        // Smoothly move in the desired direction taking the required amount of time
        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / moveDuration;
            transform.position = Vector2.Lerp(startPos, endPos, percent);
            yield return null;
        }

        // Make sure we end up exactly where we want to
        transform.position = endPos;

        // No longer moving, can accept another move input.
        isMoving = false;
    }
}
