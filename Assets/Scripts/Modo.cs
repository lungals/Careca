using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class Modo : MonoBehaviour
{
    [SerializeField]
    TMP_InputField ip, port;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("ip"))
            ip.text = PlayerPrefs.GetString("ip");
        if(PlayerPrefs.HasKey("port"))
            port.text=PlayerPrefs.GetString("port");
    }

    public void Server()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("0.0.0.0", ushort.Parse(port.text));
        NetworkManager.Singleton.StartServer();
        GravarPreferencias();
       
    }
    public void Host()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", ushort.Parse(port.text), "0.0.0.0");
        NetworkManager.Singleton.StartHost();
        GravarPreferencias();
    }
    public void Client()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip.text, ushort.Parse(port.text));
        NetworkManager.Singleton.StartClient();
        GravarPreferencias();
    }

    void GravarPreferencias()
    {
        PlayerPrefs.SetString("ip",ip.text);
        PlayerPrefs.SetString("port", port.text);
        gameObject.SetActive(false);
    }

}
