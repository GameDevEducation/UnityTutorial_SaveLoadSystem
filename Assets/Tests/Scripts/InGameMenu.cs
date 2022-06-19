using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
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

    public void LoadMainLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SaveLoadTest_MainScene");
    }
}
