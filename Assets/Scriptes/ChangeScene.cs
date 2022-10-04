using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public GameObject exit;
    GameObject gm;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void About()
    {
        SceneManager.LoadScene("About");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scene_1");
    }

    public void MainMenu ()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        gm = Instantiate(exit, GameObject.Find("UICanvas").transform);
    }

    public void Exit_yes()
    {
        Application.Quit();
    }

    public void Exit_No()
    {
        Destroy(gameObject);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Scene_1");
    }

    public void OpenUrl (string url)
    {
        Application.OpenURL (url);
    }

}
