using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Dialogue : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] sentences;
    public GameObject panelDialog;
    public Text dialog;
    public bool dialogStart = false;

    void Start() {
        panelDialog.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player")
        {
            panelDialog.SetActive(true);
            dialog.text = sentences[0];
            dialogStart = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        panelDialog.SetActive(false);
        dialogStart = false;
    }

    private void Update() {
        if (dialogStart == true) 
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                dialog.text = sentences[1];

            }
        }
    }
}
