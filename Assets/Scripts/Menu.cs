using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button[] lvls;
    public Text coinText;
    public Slider musicSlider, soundSlider;
    public Text musicText, soundText;

    void Start()
    {
        if (PlayerPrefs.HasKey("Lvl"))
            for (int i = 0; i < lvls.Length; i++)
            {
                if (i <= PlayerPrefs.GetInt("Lvl"))
                    lvls[i].interactable = true;
                else
                    lvls[i].interactable = false;
            }

        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetInt("MusicVolume", 3);
        }
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetInt("SoundVolume", 8);
        }

        musicSlider.value = PlayerPrefs.GetInt("MusicVolume");
        soundSlider.value = PlayerPrefs.GetInt("SoundVolume");
    }

    void Update()
    {
        PlayerPrefs.SetInt("MusicVolume", (int)musicSlider.value);
        PlayerPrefs.SetInt("SoundVolume", (int)soundSlider.value);
        musicText.text = musicSlider.value.ToString();
        soundText.text = soundSlider.value.ToString();
        
        coinText.text = PlayerPrefs.GetInt("coins", 0).ToString();

    }

    public void ExitGame()
    {
        Application.Quit(); 
    }

    public void OpenScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void DelKeys()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Buy_hp(int cost)
    {
        if (PlayerPrefs.GetInt("coins", 0) >= cost)
        {
            PlayerPrefs.SetInt("hp", PlayerPrefs.GetInt("hp") + 1);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - cost);
        }
    }
    public void Buy_bg(int cost)
    {
        if (PlayerPrefs.GetInt("coins") >= cost)
        {
            PlayerPrefs.SetInt("bg", PlayerPrefs.GetInt("bg") + 1);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - cost);
        }
    }
    public void Buy_rg(int cost)
    {
        if (PlayerPrefs.GetInt("coins") >= cost)
        {
            PlayerPrefs.SetInt("rg", PlayerPrefs.GetInt("rg") + 1);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - cost);
        }
    }

}
