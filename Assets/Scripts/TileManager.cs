using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //The Enum is to manage the "State" of each tile, it can be Empty, an X or an O
    public enum TileState
    {
        Empty,
        X,
        O
    }

    //Manages each tile Object, and whether the specific tile can be used and what it's value is.
public class TileManager : MonoBehaviour
{
    public GameObject cross; //Sprites that are placed on tile.
    public GameObject circle;
    public bool spaceAvailable; //deprecated
    public TileState space; //Enum that gives the tile a value
    // Start is called before the first frame update


    void Start()
    {
        spaceAvailable = true; //deprecated
        space = TileState.Empty;
    }

    void OnMouseDown() //When the mouse is clicked and the tile is available and it is the player's turn. The sprite for the player is created.
    {
        if(spaceAvailable && CheckPlayerTurn())
        {
            Instantiate(cross, transform.position, Quaternion.identity);
            space = TileState.X;
            spaceAvailable = false;
            GameController.Instance.ChangeTurn();
        }
    }

    public bool CheckPlayerTurn() //Determines if it is the player's turn
    {
        return GameController.Instance.PlayerTurn;
    }
    
    public void PlaceAiTile() //Creates the sprite for the AI at the selected position
    {
        Instantiate(circle, transform.position, Quaternion.identity);
        space = TileState.O;
        spaceAvailable = false;
        GameController.Instance.ChangeTurn();
    }

}

