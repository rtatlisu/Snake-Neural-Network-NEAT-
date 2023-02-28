using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    bool coroutineRunning = false;
    public void SaveOnClick()
    {
        GetComponent<Image>().color = Color.green;
        GameManager.instance.Save();
        if(!coroutineRunning)
        {
            StartCoroutine(sleep());
        } 
    }

    IEnumerator sleep()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(2);
        GetComponent<Image>().color = Color.white;
        coroutineRunning = false;
    }
}
