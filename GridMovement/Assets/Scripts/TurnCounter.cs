using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCounter : MonoBehaviour
{
    [Header("Inscribed")]
    //Total number of turns player can take before enemies' turn
    public int allowedTurns = 10;

    [Header("Dynamic")]
    //Numeber of turns player has taken so far
    public int turns = 0;

    private Text uiText;

    // Start is called before the first frame update
    void Start()
    {
        uiText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        uiText.text = "Turn #:\n" + turns.ToString("#,0") + "/" + allowedTurns.ToString("#,0");
    }
}
