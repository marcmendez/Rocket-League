using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneChanger : MonoBehaviour
{
    public void ChangeScenes (string sceneGame)
    {
        Debug.Log("Hola bixis");
        SceneManager.LoadScene(sceneGame);
    }
}
