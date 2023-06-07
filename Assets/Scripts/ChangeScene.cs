using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneType
{
    EnterScene = 1, 
    HallScene = 2

}
public class ChangeScene : MonoBehaviour
{
    [SerializeField] private SceneType _targetScene;
    [SerializeField] private Button _button;
    
    void Start()
    {
        _button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        if (_targetScene == SceneType.EnterScene)
        {
            SceneManager.LoadScene("Start scene");
        }
        else
        {
            SceneManager.LoadScene("Hall scene");
        }
    }
}
