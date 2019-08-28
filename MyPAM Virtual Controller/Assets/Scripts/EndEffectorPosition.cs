using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndEffectorPosition : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 currentEndEffectorPosition;
    Vector3 mappedPosition;

    public Text textX;
    public Text textY;

    public GameObject joint1;
    void Start() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        currentEndEffectorPosition = joint1.transform.position - transform.position;
        mappedPosition = currentEndEffectorPosition - new Vector3(0.05f, 0.05f, 0.0f);

        mappedPosition.x = -(mappedPosition.x * (300f / 4.9f) - 150f);
        UDP_Handler.X2pos = mappedPosition.x;
        mappedPosition.y = -(mappedPosition.y * (210f / 3f) - 171.5f);
        UDP_Handler.Y2pos = mappedPosition.y;

        textX.text = "X: " + mappedPosition.x.ToString();
        textY.text = "Y: " + mappedPosition.y.ToString();
    }
}
