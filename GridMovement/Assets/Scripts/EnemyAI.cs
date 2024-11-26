using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Inscribed")]
    //Number of moves per turn
    public int Moves = 1;
    //Amount of HP
    public int HP = 1;
    //Attack range
    public int Range = 1;
    // Time in sec to move between grid positions
    public float moveDuration = 0.1f;
    // Size of the grid
    public float gridSize = 1f;
    // Grid border
    public float gridBorder = 4.5f;

    private bool isMoving = false;

    private TurnCounter turnCounter;

    private float xrange = 0;

    private float yrange = 0;

    private float totRange = 0;

    private bool xNeg = false;

    private bool yNeg = false;

    // Start is called before the first frame update
    void Start()
    {
        //Find TurnCounter
        GameObject turnGO = GameObject.Find("TurnCounter");
        turnCounter = turnGO.GetComponent<TurnCounter>();

        Invoke("enemyMove", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void enemyMove()
    {
        // Only process one move at a time
        if (!isMoving)
        {
            //compute the distance from the player
            findRange();
            //get the current position
            Vector2 currentPos = transform.position;

            // if the total range is 1, enemy is next to player and can attack (needs to be implemented)
            if (totRange == 1)
            {
                //for now, print a message in the log and break
                Debug.LogWarning("Enemy can attack player");
            }
            else if (xrange >= yrange) // defaults to moving left/right if the xrange and yrange are the same
            {
                if (xNeg == false && currentPos.x > -gridBorder)//xNeg is false when the enemy is to the right of the player
                {
                    StartCoroutine(Move(Vector2.left));
                    Debug.LogWarning("Enemy moving left");
                }
                else if (xNeg == true && currentPos.x < gridBorder)//xNeg is true when the enemy is to the left of the player
                {
                    StartCoroutine(Move(Vector2.right));
                    Debug.LogWarning("Enemy moving right");
                }
            }
            else if (yrange > xrange)
            {
                if (yNeg == false && currentPos.y > -gridBorder)//yNeg is false when the enemy is above the player
                {
                    StartCoroutine(Move(Vector2.down));
                    Debug.LogWarning("Enemy moving down");
                }
                else if (yNeg == true && currentPos.y < gridBorder)//yNeg is true when the enemy is below the player
                {
                    StartCoroutine(Move(Vector2.up));
                    Debug.LogWarning("Enemy moving up");
                }
            }
        }
        Invoke("enemyMove", 3f);
    }

    void findRange()
    {
        //reset booleans for the next movements
        if (xNeg == true)
        {
            xNeg = false;
        }
        if (yNeg == true)
        {
            yNeg = false;
        }
        //get the positions of enemy and player
        Vector2 myPos = transform.position;
        Vector2 playPos = GameObject.Find("Player").transform.position;
        //compute the difference in x values
        xrange = myPos.x - playPos.x;
        //if it is negative, make x boolean true and switch signs
        if (xrange < 0)
        {
            xNeg = true;
            xrange = -xrange;
        }
        //compute the difference in y values
        yrange = myPos.y - playPos.y;
        //if it is negative, make y boolean true and switch signs
        if (yrange < 0)
        {
            yNeg = true;
            yrange = -yrange;
        }
        //compute the total range
        totRange = xrange + yrange;
        //print a success message in the log
        Debug.LogWarning("Range found");
    }

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
