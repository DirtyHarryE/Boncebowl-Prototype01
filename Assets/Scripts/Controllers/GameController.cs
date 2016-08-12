using UnityEngine;
using System.Collections;

public class GameController : Singleton<GameController>
{
    public enum GameMode { SelectMode, GetReady, Looking, Bowling, Returning }
    public GameMode CurrentMode
    {
        get; private set;
    }

    private enum CameraType { Rolling, Matching}
    private CameraType m_CameraType;

    Transform m_Camera;

    Rigidbody m_Sphere;

    //Looking
    private float m_Charge;
    private const float m_MaxCharge = 2f;

    public float Charge
    {
        get
        {
            return m_Charge / m_MaxCharge;
        }
    }



    float m_AngleDistance = 0.02F;

    Vector3 m_PrevForward;


    //Bowling

    private Vector3 m_BallPreviousPosition;
    private Vector3 m_BallInitialPosition;

    private float m_BallIdleTime;
    private const float BallMaxIdleTime = 3f;


    //Return

    private float m_ReturnTimer;
    private const float ReturnTimerMax = 10f;

    private Vector3 m_ReturnFromPos;
    private Quaternion m_ReturnFromRot;





    //Mouse
    float m_MouseSensitivity = 5F;
    float m_RotationY = 0F;
    private float m_MinimumY = -90F;
    private float m_MaximumY = 90F;












    public void Initialize()
    {
        CurrentMode = GameMode.SelectMode;

        m_Camera = GameObject.Find("Sphere/Main Camera").transform;

        m_Sphere = GameObject.Find("Sphere").GetComponent<Rigidbody>();

        m_BallInitialPosition = m_Sphere.transform.position;

        GazeButton.HotspotClickedEvent += ButtonPress;
    }
    // Update is called once per frame
    public void Update()
    {

        if (m_CameraType == CameraType.Matching)
        {
            m_Camera.position = m_Sphere.position;
        }

        switch (CurrentMode)
        {
            case GameMode.SelectMode:
                MoveCamera();
                break;
            case GameMode.GetReady:
                MoveCamera();
                ReadyUpdate();
                break;
            case GameMode.Looking:
                MoveCamera();
                ThrowUpdate();
                break;
            case GameMode.Bowling:
                MoveCamera();
                BowlUpdate();
                break;
            case GameMode.Returning:
                MoveCamera();
                ReturnUpdate();
                break;

        }
    }




    void ButtonPress(GazeButton button)
    {
        switch (button.Type)
        {
            case GazeButton.HotspotType.Begin:
                break;

            case GazeButton.HotspotType.SelectRoll:
                ChangeCameraType(CameraType.Rolling);
                break;
            case GazeButton.HotspotType.SelectStill:
                ChangeCameraType(CameraType.Matching);
                break;
        }
    }

    public void GetReady()
    {
        CurrentMode = GameMode.GetReady;
    }

    private void ReadyUpdate()
    {
        Vector3 cameraRot = m_Camera.transform.eulerAngles;
        Debug.Log("RTeady : " + cameraRot);

        if (!(cameraRot.y >= 270 && cameraRot.y <= 360))
        {
            CurrentMode = GameMode.Looking;
        }
    }
    
    private void ChangeCameraType(CameraType type)
    {
        m_CameraType = type;

        switch (m_CameraType)
        {
            case CameraType.Matching:
                m_Camera.SetParent(null);
                break;
            case CameraType.Rolling:
                m_Camera.SetParent(m_Sphere.transform);
                break;
        }

    }



    void PrepareUpdate()
    {

    }

    void ThrowUpdate()
    {
        Debug.DrawRay(m_Camera.position, m_Camera.forward.normalized * 10f, Color.red);
        Debug.DrawRay(m_Camera.position, m_PrevForward.normalized * 10f, Color.yellow);

        if ((m_Camera.forward - m_PrevForward).magnitude > m_AngleDistance)
        {
            m_PrevForward = m_Camera.forward;
            m_Charge = 0;
        }
        else
        {
            m_Charge += Time.deltaTime;
        }
        if (m_Charge >= m_MaxCharge)
        {
            m_Charge = 0f;
            Throw();
        }
    }
    void Throw()
    {
        m_Charge = 0;
        CurrentMode = GameMode.Bowling;

        m_Sphere.isKinematic = false;
        m_Sphere.AddForce(m_Camera.forward * 1000f);
    }

    void BowlUpdate()
    {
        Vector3 currentPos = m_Sphere.transform.position;
        if ((m_BallPreviousPosition - currentPos).magnitude > 0.5f)
        {
            Debug.Log("Updating Ball Pos : " + m_BallPreviousPosition + "; to : " + currentPos);
            m_BallPreviousPosition = currentPos;
            m_BallIdleTime = 0f;
        }
        else
        {
            m_BallIdleTime += Time.deltaTime;
        }
        if (m_BallIdleTime >= BallMaxIdleTime)
        {
            m_BallIdleTime = 0f;
            Return();
        }
        Debug.DrawLine(m_BallPreviousPosition, currentPos, Color.magenta);
        //Debug.Log("Bowling (" + m_BallPreviousPosition + ", " + currentPos + " : " + (m_BallPreviousPosition - currentPos) + "," + (m_BallPreviousPosition - currentPos).magnitude + ")");
    }
    void Return()
    {
        m_ReturnTimer = 0;

        m_Sphere.isKinematic = true;

        m_ReturnFromPos = m_Sphere.transform.position;
        m_ReturnFromRot = m_Sphere.transform.rotation;

        CurrentMode = GameMode.Returning;
    }
    void ReturnUpdate()
    {
        float t = m_ReturnTimer / ReturnTimerMax;
        m_Sphere.transform.position = Vector3.Lerp(m_ReturnFromPos, m_BallInitialPosition, t);
        m_Sphere.transform.rotation = Quaternion.Lerp(m_ReturnFromRot, Quaternion.identity, t);
        m_ReturnTimer += Time.deltaTime;

        if (m_ReturnTimer >= ReturnTimerMax)
        {
            CurrentMode = GameMode.SelectMode;
        }
    }

    void MoveCamera()
    {
        if (Input.GetMouseButton(0) && m_Camera != null)
        {
            float rotationX = m_Camera.localEulerAngles.y + Input.GetAxis("Mouse X") * m_MouseSensitivity;
            m_RotationY += Input.GetAxis("Mouse Y") * m_MouseSensitivity;
            m_RotationY = Mathf.Clamp(m_RotationY, m_MinimumY, m_MaximumY);
            m_Camera.localEulerAngles = new Vector3(-m_RotationY, rotationX, 0);
        }
    }
}