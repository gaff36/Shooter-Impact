using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class StatItem : MonoBehaviourPunCallbacks
{
    public Sprite[] portraits;
    private Image backgroundImage;
    public Image playerAvatar;


    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text deathCountText;
    private VerticalLayoutGroup layoutGroup;
    private Player player;
    public string id;

    public int killCount;
    public int deathCount;
    public int portraitID;

    public ExitGames.Client.Photon.Hashtable stats = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        layoutGroup = FindObjectOfType<VerticalLayoutGroup>();
    }

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdateStatItem(player);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdateStatItem(targetPlayer);
        }
    }
    
    private void UpdateStatItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerStats"))
        {
            Debug.Log("1");
            deathCount = ((int[])player.CustomProperties["playerStats"])[0];
            killCount = ((int[])player.CustomProperties["playerStats"])[1];
            portraitID = ((int[])player.CustomProperties["playerStats"])[2];
            int [] temp; temp = new int[3]; temp[0] = deathCount; temp[1] = killCount; temp[2] = portraitID;
            stats["playerStats"] = temp;
            playerAvatar.sprite = portraits[((int[])player.CustomProperties["playerStats"])[2]];
        }
        else
        {
            Debug.Log("2");
            killCount = 0;
            deathCount = 0;
            int[] temp; temp = new int[3]; temp[0] = deathCount; temp[1] = killCount; temp[2] = 0;
            stats["playerStats"] = temp;
            playerAvatar.sprite = portraits[0];
        }
        
        killCountText.text = "K: " + killCount.ToString();
        deathCountText.text = "D: " + deathCount.ToString();
    }

    

}
