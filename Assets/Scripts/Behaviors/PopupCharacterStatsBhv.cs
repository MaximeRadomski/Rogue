using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCharacterStatsBhv : MonoBehaviour
{
    private Character _character;
    private SkinContainerBhv _skinContainerBhv;
    private List<GameObject> _tabs;
    private List<ButtonBhv> _buttonsTabs;
    private Instantiator _instantiator;
    private int _currentTab;
    private Vector3 _resetTabPosition;
    private Vector3 _currentTabPosition;

    public void SetPrivates(Character character)
    {
        _character = character;
        _currentTab = 0;
        _resetTabPosition = new Vector3(-10.0f, 10.0f, 0.0f);
        _currentTabPosition = transform.position;
        _tabs = new List<GameObject>();
        _buttonsTabs = new List<ButtonBhv>();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        for (int i = 0; i < 5; ++i)
        {
            _tabs.Add(transform.Find("Tab" + i).gameObject);
            _buttonsTabs.Add(transform.Find("ButtonTab" + i).GetComponent<ButtonBhv>());
            if (i == 0)
            {
                BringTabFront(_tabs[i]);
            }
            else
            {
                UnpressButton(_buttonsTabs[i].gameObject);
                BringTabBack(_tabs[i], i);
            }
        }
        _skinContainerBhv = _tabs[0].transform.Find("SkinContainer").GetComponent<SkinContainerBhv>();
        SetButtons();
        DisplayStats();
    }

    private void SetButtons()
    {
        foreach (var button in _buttonsTabs)
        {
            button.EndActionDelegate = ChangeTab;
        }
    }

    private void DisplayStats()
    {
        _instantiator.LoadCharacterSkin(_character, _skinContainerBhv.gameObject);
        _tabs[0].transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = _character.Name;
        _tabs[0].transform.Find("Race").GetComponent<TMPro.TextMeshPro>().text = _character.Race.ToString();
        _tabs[0].transform.Find("Gender").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsGender_" + _character.Gender.GetHashCode());
        _tabs[0].transform.Find("Hp").GetComponent<TMPro.TextMeshPro>().text = _character.Hp.ToString();
        _tabs[0].transform.Find("HpMax").GetComponent<TMPro.TextMeshPro>().text = _character.HpMax.ToString();
        _tabs[0].transform.Find("Pa").GetComponent<TMPro.TextMeshPro>().text = _character.PaMax.ToString();
        _tabs[0].transform.Find("PaMax").GetComponent<TMPro.TextMeshPro>().text = _character.PaMax.ToString();
        _tabs[0].transform.Find("Pm").GetComponent<TMPro.TextMeshPro>().text = _character.PmMax.ToString();
        _tabs[0].transform.Find("PmMax").GetComponent<TMPro.TextMeshPro>().text = _character.PmMax.ToString();
        //_tabs[0].transform.Find("StatsList").GetComponent<TMPro.TextMeshPro>().text = GenerateStatsList();
    }

    private string GenerateStatsList()
    {
        string statsList = "";
        return statsList;
    }

    private void ChangeTab()
    {
        var newTab = int.Parse(Constants.LastEndActionClickedName.Substring(Helper.CharacterAfterString(Constants.LastEndActionClickedName, "Tab")));
        if (newTab == _currentTab)
            return;
        _currentTab = newTab;
        for (int i = 0; i < _buttonsTabs.Count; ++i)
        {
            if (i == _currentTab)
            {
                PressButton(_buttonsTabs[i].gameObject);
                BringTabFront(_tabs[i]);
            }
            else
            {
                UnpressButton(_buttonsTabs[i].gameObject);
                BringTabBack(_tabs[i], i);
            }
        }
    }

    private void PressButton(GameObject button)
    {
        var child = button.transform.GetChild(0);
        child.transform.position = button.transform.position;
    }

    private void UnpressButton(GameObject button)
    {
        var child = button.transform.GetChild(0);
        child.transform.position = button.transform.position + new Vector3(0.0f, -Constants.Pixel * 5.0f, 0.0f); 
    }

    private void BringTabFront(GameObject tab)
    {
        tab.transform.position = _currentTabPosition;
    }

    private void BringTabBack(GameObject tab, int id)
    {
        tab.transform.position = _resetTabPosition + new Vector3(0.5f * (float)id, -1.0f * (float)id, 0.0f);
    }

    public void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        Destroy(gameObject);
    }
}
