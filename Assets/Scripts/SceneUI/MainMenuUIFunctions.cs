using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIFunctions : MonoBehaviour
{
    public int maxScoreCap, maxTeamSize;
    public Slider[] sliders;
    public Toggle[] toggles;
    public TextMeshProUGUI[] textBoxes;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ToggleBots(toggles[0].gameObject);
        ToggleTeams(toggles[1].gameObject);
        UpdateTeamSize(sliders[1].gameObject);
        UpdateScoreCap(sliders[0].gameObject);
    }
    public void UpdateScoreCap(GameObject sender)
    {
        DeathmatchScript.scoreCap = (int)(sender.GetComponent<Slider>().value);
        textBoxes[0].text = "Score to win (Per Player): " + DeathmatchScript.scoreCap;
    }
    public void ToggleBots(GameObject sender)
    {
        DeathmatchScript.fillRoomWithBots = sender.GetComponent<Toggle>().isOn;
    }
    public void ToggleTeams(GameObject sender)
    {
        DeathmatchScript.teamDeathmatch = sender.GetComponent<Toggle>().isOn;
    }
    public void UpdateTeamSize(GameObject sender)
    {
        DeathmatchScript.teamSize = (int)(sender.GetComponent<Slider>().value);
        textBoxes[1].text = "Team Size: " + DeathmatchScript.teamSize;
    }
    public void LoadNextScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        DeathmatchScript.newSceneTrip = true;
    }
    public void Quit()
    {
        Application.Quit();
    }
}
