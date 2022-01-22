using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _ammoText;
    //Take ammo count from the game handler
    public void updateAmmo(int count){
        count = 11;
        _ammoText.text = "Ammo: " + count;
    }
}
