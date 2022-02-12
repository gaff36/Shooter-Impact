using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldRef;

    public void OnClickInputField()
    {
        inputFieldRef.text = "";
    }

    private void OnDisable()
    {
        inputFieldRef.text = "Enter your nickname to login​";
    }
}
