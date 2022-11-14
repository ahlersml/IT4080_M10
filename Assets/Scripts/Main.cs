using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using AddressFamily = System.Net.Sockets.AddressFamily;
using System.IO;
using System.Text;
using System.Net;
using Unity.Multiplayer.Tools;
using Unity.Netcode.Transports.UTP;


public class Main : NetworkBehaviour {

    public Button btnHost;
    public Button btnClient;
    public TMPro.TMP_Text txtStatus;
    public TMPro.TMP_InputField inputIp;
    public TMPro.TMP_InputField inputPort;
    public IpAddresses ips;

    public void Start() {
        btnHost.onClick.AddListener(OnHostClicked);
        btnClient.onClick.AddListener(OnClientClicked);
        Application.targetFrameRate = 30;
    }

    private bool ValidateSettings()
    {
        IPAddress ip;
        bool isValidIp = IPAddress.TryParse(inputIp.text, out ip);
        if (!isValidIp)
        {
            txtStatus.text = "Invalid IP";
            return false;
        }
        bool isValidPort = ushort.TryParse(inputPort.text, out ushort port);
        if (!isValidPort)
        {
            txtStatus.text = "Invalid Port";
            return false;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip.ToString(), port);

        btnHost.gameObject.SetActive(false);
        btnClient.gameObject.SetActive(false);
        inputIp.enabled = false;
        inputPort.enabled = false;

        return true;

    }
    private void StartHost(string sceneName = "Lobby", string startMessage = "Starting Host") {
        bool validSettings = ValidateSettings();
        if (!validSettings)
        {
            return;
        }
        txtStatus.text = startMessage;

        NetworkManager.Singleton.StartHost();
        NetworkManager.SceneManager.LoadScene(
            "Lobby",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    private void StartClient(string startMessage = "StartingClient")
    {
        bool validSettings = ValidateSettings();
        if (!validSettings)
        {
            return;
        }

        txtStatus.text = startMessage;

        NetworkManager.Singleton.StartClient();
        txtStatus.text = "Waiting on Host";
    }

    private void ShowConnectionData()
    {
        var curSettings = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData;
        inputIp.text = curSettings.Address.ToString();
        inputPort.text = curSettings.Port.ToString();
    }


    private void OnHostClicked() {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        txtStatus.text = "Starting Host";
        StartHost();
    }

    private void OnClientClicked() {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        txtStatus.text = "Waiting on Host";
        NetworkManager.Singleton.StartClient();
    }
}
