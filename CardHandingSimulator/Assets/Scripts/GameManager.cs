using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private InputField countInputField;
    private GameObject warning;
    // Start is called before the first frame update
    void Start()
    {
        countInputField = GameObject.Find("Canvas").transform.Find("InputField").GetComponent<InputField>();        
        warning = GameObject.Find("Canvas").transform.Find("Warning").gameObject;
        warning.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && warning.activeSelf)
            warning.SetActive(false);

    }

    public void BtnClickEvent()
    {
        if (countInputField.text != string.Empty && int.Parse(countInputField.text) > 0 && int.Parse(countInputField.text) <= 10)
        {
            HandingManager.Instance.drawableCount = int.Parse(countInputField.text);
            HandingManager.Instance.ReDraw();
        }
        else
            warning.SetActive(true);
    }
}
