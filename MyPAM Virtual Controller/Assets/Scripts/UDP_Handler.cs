using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

public class UDP_Handler : MonoBehaviour
{
    static bool devMode = false;
    static int timeout;
    
    Vector3 currentEndEffectorPosition;
    Vector3 mappedPosition;

    string dataToSend;

    public Text textX;
    public Text textY;
    public Text sendingData;
    public Text receivedData;
    static UdpClient listener;
    static IPEndPoint groupEP;
    static IPEndPoint endPoint;
    static string sendJson;

    static controllerData cData;
    static gameData gData;

    public static double X0pos { get; set; }
    public static double Y0pos { get; set; }
    public static double Z0pos { get; set; }
    public static double Fx { get; set; }
    public static double Fy { get; set; }
    public static double Fz { get; set; }
    public static int Encoder1 { get; set; }
    public static int Encoder2 { get; set; }
    public static double Pot1 { get; set; }
    public static double Pot2 { get; set; }
    public static double Theta0 { get; set; }
    public static double Theta1 { get; set; }
    public static double X1pos { get; set; }
    public static double Y1pos { get; set; }
    public static double Z1pos { get; set; }
    public static double X2pos { get; set; }
    public static double Y2pos { get; set; }
    public static double Z2pos { get; set; }
    public static bool ErrorOccurred { get; set; }

    ////////////////////////////////////////////////////////////////////////
    public static double Xtarget { get; set; } = 0;
    public static double Ytarget { get; set; } = 0;
    public static double Ztarget { get; set; } = 0;
    public static double X_Attractor1 { get; set; }
    public static double Y_Attractor1 { get; set; }
    public static double Z_Attractor1 { get; set; }
    public static double X_Attractor2 { get; set; }
    public static double Y_Attractor2 { get; set; }
    public static double Z_Attractor2 { get; set; }
    public static bool Traj_Flag { get; set; } = true;
    public static double DeadZone { get; set; } = 0;
    public static bool Assistance { get; set; } = true;
    public static bool Shutdown { get; set; } = false;
    public static double AssistanceLevel { get; set; } = 50;

    //////////////////////////////////////////////////////////////////////////
    void Start()
    {

        if (devMode == false)
        {
            listener = new UdpClient(3500);
            groupEP = new IPEndPoint(IPAddress.Any, 3500);
        }
        else
        {
            listener = new UdpClient(2300);
            groupEP = new IPEndPoint(IPAddress.Any, 2300);
        }

        listener.Client.ReceiveBufferSize = 0;

        listener.Client.ReceiveTimeout = 15;

        textX.text = "X: 0";
        textY.text = "Y: 0";

        UDPsendFirst("VC|controllerStartUpAcknowledgment");

        cData = new controllerData();
        gData = new gameData();
    }

    // Update is called once per frame
    void Update()
    {
        cData.X0pos = X0pos;
        cData.Y0pos = Y0pos;
        cData.Z0pos = Z0pos;
        cData.Fx = Fx;
        cData.Fy = Fy;
        cData.Fz = Fz;
        cData.Encoder1 = Encoder1;
        cData.Encoder2 = Encoder2;
        cData.Pot1 = Pot1;
        cData.Pot2 = Pot2;
        cData.Theta0 = Theta0;
        cData.Theta1 = Theta1;
        cData.X1pos = X1pos;
        cData.Y1pos = Y1pos;
        cData.Z1pos = Z1pos;
        cData.X2pos = X2pos;
        cData.Y2pos = Y2pos;
        cData.Z2pos = Z2pos;
        cData.ErrorOccurred = ErrorOccurred;

        sendJson = JsonConvert.SerializeObject(cData);
        dataToSend = sendJson;
        if (devMode == true)
        {
            UDPsend("dataFromVC|"+dataToSend);
        }
        else
        {
            UDPsend("VC|dataFromVC|" + dataToSend);
        }
        
        sendingData.text = "Sending: " + dataToSend.ToString();

        try
        {
            receiveData();
        }

        catch
        {
        }
        
    }

    private string receiveData()
    {  
        try
        {
            Console.WriteLine("Waiting for broadcast");
            byte[] bytes = listener.Receive(ref groupEP);
            string message = $"{Encoding.ASCII.GetString(bytes, 0, bytes.Length)}";
            receivedData.text = "Received: " + message.Split('|')[2];
            gData = JsonConvert.DeserializeObject<gameData>(message.Split('|')[2]);

            Xtarget = gData.Xtarget;
            Ytarget = gData.Ytarget; 
            Ztarget = gData.Ztarget; 
            X_Attractor1 = gData.X_Attractor1; 
            Y_Attractor1 = gData.Y_Attractor1; 
            Z_Attractor1 = gData.Z_Attractor1; 
            X_Attractor2 = gData.X_Attractor2; 
            Y_Attractor2 = gData.Y_Attractor2; 
            Z_Attractor2 = gData.Z_Attractor2; 
            Traj_Flag = gData.Traj_Flag; 
            DeadZone = gData.DeadZone;
            Assistance = gData.Assistance;
            Shutdown = gData.Shutdown;
            AssistanceLevel = gData.AssistanceLevel;

            return (message);
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            return ("timeout");
        }
    }


    public void UDPsendFirst(string datagram)
    {
        byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(datagram);

        string IP = "127.0.0.1";

        endPoint = new IPEndPoint(IPAddress.Parse(IP), 2500);

        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        client.SendTo(data, endPoint);
    }

    public void UDPsend(string datagram)
    {
        byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(datagram);

        string IP = "127.0.0.1";
        if (devMode == false)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(IP), 2600);
        }
        else
        {
            endPoint = new IPEndPoint(IPAddress.Parse(IP), 3000);
        }

        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        client.SendTo(data, endPoint);
    }


    private class gameData
    {
        public double Xtarget { get; set; }
        public double Ytarget { get; set; }
        public double Ztarget { get; set; }
        public double X_Attractor1 { get; set; }
        public double Y_Attractor1 { get; set; }
        public double Z_Attractor1 { get; set; }
        public double X_Attractor2 { get; set; }
        public double Y_Attractor2 { get; set; }
        public double Z_Attractor2 { get; set; }
        public bool Traj_Flag { get; set; }
        public double DeadZone { get; set; }
        public bool Assistance { get; set; }
        public bool Shutdown { get; set; }
        public double AssistanceLevel { get; set; }
    }

    private class controllerData
    {
        public double X0pos { get; set; }
        public double Y0pos { get; set; }
        public double Z0pos { get; set; }
        public double Fx { get; set; }
        public double Fy { get; set; }
        public double Fz { get; set; }
        public Int32 Encoder1 { get; set; }
        public Int32 Encoder2 { get; set; }
        public double Pot1 { get; set; }
        public double Pot2 { get; set; }
        public double Theta0 { get; set; }
        public double Theta1 { get; set; }
        public double X1pos { get; set; }
        public double Y1pos { get; set; }
        public double Z1pos { get; set; }
        public double X2pos { get; set; }
        public double Y2pos { get; set; }
        public double Z2pos { get; set; }
        public bool ErrorOccurred { get; set; }
    }
}
