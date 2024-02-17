using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int NumberOfPlayers;

    private int sheriffNumber = 1;
    private int renegadeNumber = 1;
    private int outlawNumber = 2;
    private int deputyNumber = 0;

    private void Start()
    {
        switch (NumberOfPlayers)
        {
            case 4: // might delete
                break;
            case 5:
                deputyNumber = 1;
                break;
            case 6:
                outlawNumber = 3;
                deputyNumber = 1;
                break;
            case 7:
                outlawNumber = 3;
                deputyNumber = 2;
                break;
        }
    }
}
