using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject LoadGameButton;

    // Start is called before the first frame update
    void Start()
    {
        LoadGameButton.SetActive(SaveLoadManager.Instance.HasSavedGames);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        SaveLoadManager.Instance.ClearSave();

        LoadMainLevel();
    }

    public void LoadMainLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SaveLoadTest_MainScene");
    }
}
