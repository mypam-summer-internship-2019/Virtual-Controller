using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotationTest : MonoBehaviour
{

    private static float targetX = 0;
    private static float targetY = 0;

    Vector2 targetPos;

    float gridTLX;
    float gridTLY;

    float gridBRX;
    float gridBRY;

    float armA;
    float armB;
    float armC;

    float r1;
    float A;
    float theta1;
    float theta2;

    float j1Angle;
    float j2Angle;

    private static bool move = true;

    float armLength2;
    float armLength;

    public GameObject joint1;
    public GameObject joint2;
    public GameObject endEffector;

    public Slider speed;

    Vector3 endEffectorPos;
    Vector3 gridPos;
    Vector3 startMousePos;
    Vector3 endEffectorStartPos;
    Vector3 currentMousePos;
    Vector3 moveArm;
    Vector3 joint1Pos;
    Vector3 startPos;

    public Toggle assistanceToggle;

    // Start is called before the first frame update
    void Start()
    {
        joint1Pos = joint1.transform.position;

        //Arm length in pixels

        gridPos = joint2.transform.position;
        armA = gridPos.y;

        gridPos = endEffector.transform.position;
        armB = gridPos.y;

        gridPos = joint1.transform.position;
        armC = gridPos.y;

        armLength = armA - armB;

        armLength2 = armC - armA;

        ///Debugging
        Debug.Log(gridTLX);
        Debug.Log(gridTLY);
        Debug.Log(gridBRX);
        Debug.Log(gridBRY);
        Debug.Log( "X Length: " + (gridTLX - gridBRX) );
        Debug.Log( "Y Length: " + (gridBRY - gridTLY) );
        Debug.Log( "Arm Length: " + (armLength));

        joint1.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0);
        joint2.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90);


        r1 = Mathf.Sqrt(Mathf.Pow(2.51f, 2) + Mathf.Pow(2.51f, 2));

        A = Mathf.Acos((Mathf.Pow(armLength2, 2) + Mathf.Pow(armLength, 2) - Mathf.Pow(r1, 2)) / (2 * armLength2 * armLength));
        theta2 = Mathf.PI - A;
        theta1 = Mathf.Asin(Mathf.Sin(A) / r1 * armLength) + Mathf.Atan(-2.45f / 2.45f);

        j1Angle = theta1 / Mathf.PI * 180;
        j2Angle = (-theta2 + theta1) / Mathf.PI * 180;

        joint1.transform.rotation = Quaternion.Euler(0.0f, 0.0f, j1Angle);
        joint2.transform.rotation = Quaternion.Euler(0.0f, 0.0f, j2Angle);
    }
    // Update is called once per frame
    void Update()
    {
        targetX = joint1.transform.position.x - ((((float)(-UDP_Handler.Xtarget) + 150f) / (300f / 4.9f)) + 0.05f);
        targetY = joint1.transform.position.y - ((((float)(-UDP_Handler.Ytarget) + 171.5f) / (210f / 3f)) + 0.05f);
        targetPos = new Vector3(targetX, targetY,-6);
        Debug.Log(targetPos);

        

        if (Input.GetKeyDown("mouse 0") == true)
        {
            startMousePos = transform.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            endEffectorStartPos = endEffector.transform.position;
        }

        if (Input.GetKey("mouse 0") == true)
        {
            if (Input.GetKey("mouse 0") == true)
            {
                currentMousePos = transform.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            }

            moveArm = startMousePos - currentMousePos;
            endEffectorPos = endEffectorStartPos - joint1Pos - moveArm;
           
            if (endEffectorPos.x <= -4.95f)
            {
                endEffectorPos.x = -4.95f;
            }
            else if (endEffectorPos.x >= -0.05f)
            {
                endEffectorPos.x = -0.05f;
            }

            if (endEffectorPos.y <= -4f)
            {
                endEffectorPos.y = -4f;
            }

            else if (endEffectorPos.y >= -1f)
            {
                endEffectorPos.y = -1f;
            }
            /// Inverse Kiematics//////////////////////////////////////

            r1 = Mathf.Sqrt(Mathf.Pow(endEffectorPos.x, 2) + Mathf.Pow(endEffectorPos.y, 2));

            A = Mathf.Acos((Mathf.Pow(armLength2, 2) + Mathf.Pow(armLength, 2) - Mathf.Pow(r1, 2)) / (2 * armLength2 * armLength));
            theta2 = Mathf.PI - A;
            theta1 = Mathf.Asin(Mathf.Sin(A) / r1 * armLength) + Mathf.Atan(-endEffectorPos.x/ endEffectorPos.y);

            j1Angle = theta1 / Mathf.PI * 180;
            j2Angle = (-theta2 + theta1) / Mathf.PI * 180;

            UDP_Handler.Pot1 = j1Angle;
            UDP_Handler.Pot2 = j2Angle;

            joint1.transform.rotation = Quaternion.Euler(0.0f, 0.0f, j1Angle);
            joint2.transform.rotation = Quaternion.Euler(0.0f, 0.0f, j2Angle);    
        }

        if (assistanceToggle.isOn)
        {

            endEffectorPos = (Vector3.Lerp(endEffector.transform.position, targetPos, (speed.value / Vector3.Distance(targetPos, endEffector.transform.position))) - joint1Pos);


            if (endEffectorPos.x <= -4.95f)
            {
                endEffectorPos.x = -4.95f;
            }
            else if (endEffectorPos.x >= -0.05f)
            {
                endEffectorPos.x = -0.05f;
            }

            if (endEffectorPos.y <= -4f)
            {
                endEffectorPos.y = -4f;
            }

            else if (endEffectorPos.y >= -1f)
            {
                endEffectorPos.y = -1f;
            }
            /// Inverse Kiematics//////////////////////////////////////

            r1 = Mathf.Sqrt(Mathf.Pow(endEffectorPos.x, 2) + Mathf.Pow(endEffectorPos.y, 2));

            A = Mathf.Acos((Mathf.Pow(armLength2, 2) + Mathf.Pow(armLength, 2) - Mathf.Pow(r1, 2)) / (2 * armLength2 * armLength));
            theta2 = Mathf.PI - A;
            theta1 = Mathf.Asin(Mathf.Sin(A) / r1 * armLength) + Mathf.Atan(-endEffectorPos.x / endEffectorPos.y);

            j1Angle = theta1 / Mathf.PI * 180;
            j2Angle = (-theta2 + theta1) / Mathf.PI * 180;

            UDP_Handler.Pot1 = j1Angle;
            UDP_Handler.Pot2 = j2Angle;

            joint1.transform.rotation = Quaternion.Euler(0.0f, 0.0f, j1Angle);
            joint2.transform.rotation = Quaternion.Euler(0.0f, 0.0f, j2Angle);
        }
    }
}