using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject cross;
    public GameObject circle;
    public bool spaceAvailable;
    public string space = "empty";
    int r;
    // Start is called before the first frame update
    void Start()
    {
        spaceAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDown()
    {
        if(spaceAvailable && CheckPlayerTurn())
        {
    
                 Instantiate(cross, transform.position, Quaternion.identity);
                 
                 space = "cross";

                spaceAvailable = false;
                GameController.Instance.ChangeTurn();
        }

    }


    public bool CheckPlayerTurn()
    {
        return GameController.Instance.PlayerTurn;
    }
    

    public void PlaceCpuTile()
    {
        Instantiate(circle, transform.position, Quaternion.identity);
        space = "circle";
        spaceAvailable = false;
        GameController.Instance.ChangeTurn();

    }

    


}

