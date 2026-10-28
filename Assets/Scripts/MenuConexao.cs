using UnityEngine;
using Fusion;
using TMPro;

public class MenuConexao : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private TMP_InputField nomeSalaInput;
    [SerializeField] private GameObject painelMenu;

    public async void ConectarNaSala()
    {
        if (string.IsNullOrEmpty(nomeSalaInput.text)) return;

        painelMenu.SetActive(false);
        var sceneManager = GetComponent<NetworkSceneManagerDefault>();

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = nomeSalaInput.text,
            PlayerCount = 2,
            SceneManager = sceneManager
        });
    }
}

