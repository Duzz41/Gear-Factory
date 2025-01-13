using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FianlScene : MonoBehaviour
{
    // Start is called before the first frame update
    // Update is called once per frame
    public void PLayScene()
    {
        SceneManager.LoadScene("FinalScene");
    }
    public void GoCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
