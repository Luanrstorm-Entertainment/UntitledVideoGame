using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.scripts.Monobehaviour.Essence;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingUIController : MonoBehaviour
{
    GameObject player;
    public BuildModeController buildMode;

    public TextMeshProUGUI essencetext;
    //show these things in the menu
    public List<GameObject> BuildableStructures = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (buildMode == null) buildMode = GameObject.Find("BuildMode").GetComponent<BuildModeController>();

    }
    public void OpenWindow()
    {
        //Cursor.lockState = CursorLockMode.None;
        if(player != null)
        {
            player.GetComponent<Player>().SetInputEnabled(false);
            
            //player.GetComponents<MonoBehaviour>().ToList().ForEach(x => x.enabled = false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.SetActive(true);
        essencetext.text = EssenceBank.Instance?.EssenceAmount.ToString()??"0";
    }
    public void CloseWindow()
    {
        if (player != null)
        {
            player.GetComponent<Player>().SetInputEnabled(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    public void BuildObjecct(GameObject gameObject)
    {
        buildMode.BuildStructure(gameObject.GetComponent<BuildableStructure>());
    }

    private void Update()
    { 
        essencetext.text = EssenceBank.Instance?.EssenceAmount.ToString()??"0";
    }
}
