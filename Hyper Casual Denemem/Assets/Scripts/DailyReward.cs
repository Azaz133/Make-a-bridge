using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;
    public long rewardGiivingTÝmeTicks;
    public GameObject rewardMenu;
    public Text remainingTimeText;


    public void InýtializeDailyReward()
    {
        if (PlayerPrefs.HasKey("lastDailyReward"))
        {
            rewardGiivingTÝmeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;
            long currentTime = System.DateTime.Now.Ticks;
            if (currentTime >= rewardGiivingTÝmeTicks)
            {
                GiveReward();   
            }
        }
        else
        {
            GiveReward();    
        }
        initialized = true; 
    }

    public void GiveReward()
    {
        levelController.Current.GiveMoneyToPLayer(100);
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReward", System.DateTime.Now.Ticks.ToString());
        rewardGiivingTÝmeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;
    }

    void Update()
    {
        if (initialized)
        {
            if (levelController.Current.startMenu.activeInHierarchy)
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGiivingTÝmeTicks - currentTime;
                if(remainingTime <= 0)
                {
                    GiveReward();
                }
                else
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);
                    remainingTimeText.text = string.Format("{0}:{1}:{2}",timeSpan.Hours.ToString("D2") , timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));
                    Debug.Log(remainingTimeText.text);
                }
            }
        }
    }
    public void TapToReturnButton()
    {
        rewardMenu.SetActive(false);
    }
}
