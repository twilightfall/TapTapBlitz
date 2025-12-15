using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneManager : MonoBehaviour
{
   public static SceneManager instance;

    InputAction backAct;

    public TMP_Text text;

    float x = 0;

    private IEnumerator coroute;

    public bool isPaused = false;

    public bool gameStarted = false; 
    public float countdown = 20;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        //coroute = UpdateText();
    }

    private void Start()
    {
        backAct = InputSystem.actions.FindAction("Back");
        backAct.performed += PauseGame;
    }

    //IEnumerator UpdateText()
    //{
    //    print("started");
    //    while (true)
    //    {
    //        text.text = countdown.ToString("F0");
    //        countdown--;

    //        if (countdown < 0)
    //        {
    //            print("GO!");
    //            End();
    //        }

    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    //private void Start()
    //{
    //    StartCoroutine(coroute);
    //}

    //void End()
    //{
    //    print("end");
    //    gameStarted = true;
    //    countdown = 2;
    //    StopAllCoroutines();
    //}

    //public void StartCR()
    //{
    //    gameStarted = false;    
    //    StartCoroutine(coroute);
    //}

    //public void EndCR()
    //{
    //           StopCoroutine(coroute);
    //    gameStarted = false;
    //    countdown = 2;
    //    text.text = "";
    //}

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void PauseGame(InputAction.CallbackContext ctx)
    {
        
    }
}
