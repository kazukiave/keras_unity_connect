using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using GridExtensions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

public class MnistAgent : Agent
{
    public bool drawingMode = false;

    //camera
    [Header("[Camera]")]
    public Camera agentCamera;
    public Camera persCamera;
    public string classification_path ;
    private Texture2D agentTexture;

    //grid
    [Header("[Grid]")]
    public float gridSize;
    public int xExtend;
    public int yExtend;
    private GridWorld gridWorld;
    private int agentPosIdx;

    //action 
    const int right = (int)Axis.Right;
    const int left = (int)Axis.Left;
    const int up = (int)Axis.Up;
    const int down = (int)Axis.Down;
    const int rightUp = (int)Axis.RightUp;
    const int leftUp = (int)Axis.LeftUp;
    const int rightDown = (int)Axis.RightDown;
    const int leftDown = (int)Axis.LeftDown;
    const int none = (int)Axis.None;
   
    int decisionPeriod = 0;

    //socket
    [Header("[Socket]")]
    public int port;
    public string ipAddress = "127.0.0.1";
    private Socket socket;
    byte[] sendBuffer = new byte[1569];

    //
    List<int> deadGrid = new List<int>();

  
    public override void InitializeAgent()
    {
        if (Academy.Instance.IsCommunicatorOn == true)
        {
            Time.timeScale = 100;
        }
        Debug.Log("time scale =" + Time.timeScale);
        gridWorld = gameObject.AddComponent<GridWorld>();
        gridWorld = new GridWorld(gridSize, xExtend, yExtend, new Vector2(transform.position.x, transform.position.y));
        gridWorld.SetCameraOnCenter(agentCamera, - 5.0f);

        /*
        decisionPeriod = GetComponent<DecisionRequester>().DecisionPeriod;
        Debug.Log("DecisionPeriod =" + decisionPeriod);
        */
        agentPosIdx = Mathf.FloorToInt(xExtend / 2);
        gridWorld.States[agentPosIdx] = State.Dead;

        //agentTexture = new Texture2D(agentCamera.targetTexture.width, agentCamera.targetTexture.height, TextureFormat.RGB24, false);
        SocketConnection();
        SendBufferClear();
      
    }

    private void SendBufferClear()
    {
        for(int i = 0; i < 28 * 28; i++)
        {
            sendBuffer[i*2] = 48; //48 = 0
            sendBuffer[i * 2 + 1] = 44; //44 = ,
        }
    }

    public override void CollectObservations()
    {
       
        SetMask();
    }

    List<int> actionList = new List<int>();
    private List<int> SetMask()
    {
        actionList.Clear(); 

        if (gridWorld.GetRight(agentPosIdx) == -1)
        {
            SetActionMask(right);
        }
        else
        {
            actionList.Add(right);
        }

        if (gridWorld.GetLeft(agentPosIdx) == -1)
        {
            SetActionMask(left);
        }
        else
        {
            actionList.Add(left);
        }

        if (gridWorld.GetUp(agentPosIdx) == -1)
        {
            SetActionMask(up);
        }
        else
        {
            actionList.Add(up);
        }

        if (gridWorld.GetDown(agentPosIdx) == -1)
        {
            SetActionMask(down);
        }
        else
        {
            actionList.Add(down);
        }

        if(gridWorld.GetRightUp(agentPosIdx) == -1)
        {
            SetActionMask(rightUp);
        }
        else
        {
            actionList.Add(rightUp);
        }

        if (gridWorld.GetLeftUp(agentPosIdx) == -1)
        {
            SetActionMask(leftUp);
        }
        else
        {
            actionList.Add(leftUp);
        }

        if (gridWorld.GetRightDown(agentPosIdx) == -1)
        {
            SetActionMask(rightDown);
        }
        else
        {
            actionList.Add(rightDown);
        }

        if (gridWorld.GetLeftDown(agentPosIdx) == -1)
        {
            SetActionMask(leftDown);
        }
        else
        {
            actionList.Add(leftDown);
        }

        //None
        if (actionList.Count == 0)
        {
            actionList.Add(none);
        }
        else
        {
            SetActionMask(none);
        }

        return actionList;
    }

    int rewardCount = 0;
    public override void AgentAction(float[] vectorAction)
    {
        if (drawingMode)
        {
            if (Input.GetMouseButton(0))
            {
                Drawing();
            }
            return;
        }

        float action = vectorAction[0];
      
        int prePosIdx = agentPosIdx;
        switch (action)
        {
            case right:
                agentPosIdx = gridWorld.GetRight(agentPosIdx);
                break;

            case left:
                agentPosIdx = gridWorld.GetLeft(agentPosIdx);
                break;

            case up:
                agentPosIdx = gridWorld.GetUp(agentPosIdx);
                break;

            case down:
                agentPosIdx = gridWorld.GetDown(agentPosIdx);
                break;

            case rightUp:
                agentPosIdx = gridWorld.GetRightUp(agentPosIdx);
                break;

            case leftUp:
                agentPosIdx = gridWorld.GetLeftUp(agentPosIdx);
                break;

            case rightDown:
                agentPosIdx = gridWorld.GetRightDown(agentPosIdx);
                break;

            case leftDown:
                agentPosIdx = gridWorld.GetLeftDown(agentPosIdx);
                break;

            case none:
                return;
        }
        if(agentPosIdx == -1)
        {
            agentPosIdx = prePosIdx;
            return;
        }
        //if the agent moves, It will update its status
        if (action != none)
        {
            gridWorld.States[agentPosIdx] = State.Dead;
            gridWorld.MeshRenders[prePosIdx].material.color = Color.white;
            //gridWorld.RefreshDisplay();
            gridWorld.MeshRenders[agentPosIdx].material.color = Color.red;
            sendBuffer[agentPosIdx * 2] = 49;//1
        }
        /*
        rewardCount++;
        if (rewardCount >= 180)
        {
            SetReward(Predict());
        }
        */
        SetReward(Predict());
    }

    public override float[] Heuristic()
    {
        var actionList = SetMask();
        actionList.Jitter();
        return new float[1] { actionList[0] };
    }

    public override void AgentReset()
    {
        gridWorld.Reset();
        agentPosIdx = Random.Range(0, xExtend * yExtend / 2);
        gridWorld.States[agentPosIdx] = State.Dead;
        SendBufferClear();
        rewardCount = 0;
    }


    //http://okuya-kazan.hatenablog.com/entry/2017/08/30/180732
    private void CaptureFromCamera()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = agentCamera.targetTexture;

        agentTexture.ReadPixels(new Rect(0, 0, agentCamera.targetTexture.width, agentCamera.targetTexture.height),0 ,0);
        agentTexture.Apply();
        RenderTexture.active = currentRT;

        byte[] bytes = agentTexture.EncodeToPNG();
        File.WriteAllBytes(classification_path + "/agentView.png", bytes);
    }

    private void SocketConnection()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.NoDelay = true;
        socket.Connect(ipAddress, port);
        Debug.Log("Ip is " + ipAddress + ", Port is " + port);
    }

   
    private float Predict()
    {
        //Tensor作ってそれを送るバージョン画像は保存しない。
        //Serverに送るTensorを作成。
        //StringBuilder tensorBuilder = new StringBuilder();
        /*
        string tensor = "";
        
        for(int i = 0; i < gridWorld.MeshRenders.Length; i++)
        {
            Color color = gridWorld.MeshRenders[i].material.color;
            if(color == Color.black)
            {
                //tensorBuilder.Append("0,");
                //tensor += "0,";
                sendBuffer.Add(48);
                sendBuffer.Add(44);
            }
            else
            {
                //tensorBuilder.Append("1,");
                //tensor += "1,";
                sendBuffer.Add(49);
                sendBuffer.Add(44);
            }
        }
        */

        socket.Send(sendBuffer);

        //socket.Send(Encoding.UTF8.GetBytes(tensor));

        //socket.Send(Encoding.UTF8.GetBytes(tensorBuilder.ToString()));


        byte[] receiveBuffer = new byte[100];
        while (true)
        {
            double receiveSize = socket.Receive(receiveBuffer, receiveBuffer.Length, SocketFlags.None);
            if (receiveSize != 0)
            {
                break;
            }
        }

        float reward = float.Parse(Encoding.UTF8.GetString(receiveBuffer));
        if(!Academy.Instance.IsCommunicatorOn)
        {
            Debug.Log("reward" + reward);
        }
        if (reward < 0.3)
            reward = 0;
        return reward;
        
        /* 画像から読み取るパターン
        byte[] sendBuffer = Encoding.UTF8.GetBytes("s");
        socket.Send(sendBuffer);

        byte[] receiveBuffer = new byte[20];
        while(true)
        {
            double receiveSize = socket.Receive(receiveBuffer, receiveBuffer.Length, SocketFlags.None);
            if (receiveSize != 0)
                break;
        }

        string receiveMessage = Encoding.UTF8.GetString(receiveBuffer);
        return float.Parse(receiveMessage);
        */
    }

    private void SocketShutdow()
    {
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        
    }

    private void OnDestroy()
    {
        SocketShutdow();
    }

    private void FixedUpdate()
    {
        agentCamera.Render();
        if (Academy.Instance.IsCommunicatorOn)
        {
            RequestDecision();
        }
        else
        {
            RequestDecision();
        }
    }

    //Drawing

    private void Drawing()
    {
        Vector3 mousePos3D = agentCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector3(mousePos3D.x, mousePos3D.y);
        /*
        var cube = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
        cube.transform.position = mousePos;
        */
        int nearMesh = 0;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < gridWorld.Centers.Length; i++)
        {
            float distance = Vector3.Magnitude(gridWorld.Centers[i] - mousePos);
            if (minDist > distance)
            {
                minDist = distance;
                nearMesh = i;
            }
        }
     
        gridWorld.States[nearMesh] = State.Dead;
        gridWorld.MeshRenders[nearMesh].material.color = Color.white;
        sendBuffer[nearMesh * 2] = 49;//1
        agentCamera.Render();
        Predict();
    }
}
