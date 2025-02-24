using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class RoomBorder : MonoBehaviour
{
    [SerializeField] LayerMask layer;
    Collider2D roomArea;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnTriggerExit2D(Collider2D other)
    {

    }
}
