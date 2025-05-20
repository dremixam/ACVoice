using System.Collections;
using System.Collections.Generic;
using TwitchLib.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PubSubBehaviour : MonoBehaviour
{
    private PubSub _pubSub;

    public TextBoxEnabler TextBoxEnabler;

    public Button EnableButton;

    public TwitchOAuth TwitchOAuth;
    public string RewardID;

    public void Revoke()
    {
        _pubSub.Disconnect();
    }

    public void Subscribe()
    {
        if (TwitchOAuth.GetAuthToken() == "")
        {
            Logger.Log("Can't listen, you're not authenticated");
            return;
        }

        // Create new instance of PubSub Client
        _pubSub = new PubSub();

        // Subscribe to Events
        //_pubSub.OnChannelPointsRewardRedeemed += OnChannelPointsRewardRedeemed;
        _pubSub.OnRewardRedeemed += OnRewardRedeemed;


        _pubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;

        _pubSub.OnPubSubServiceError += OnPubSubServiceError;

        _pubSub.OnLog += OnLog;

        // Connect
        _pubSub.Connect();
    }

    private void OnPubSubServiceConnected(object sender, System.EventArgs e)
    {
        Logger.Log("Waiting for events…");

        _pubSub.ListenToRewards(TwitchOAuth.GetUserID());

        _pubSub.SendTopics(TwitchOAuth.GetAuthToken());
    }

    private void OnChannelPointsRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnChannelPointsRewardRedeemedArgs e)
    {
        if (e.RewardRedeemed.Redemption.Reward.Title == RewardID)
        {


            Logger.Log("Request from " + e.RewardRedeemed.Redemption.User.DisplayName);

            TextBoxEnabler.PlayText(e.RewardRedeemed.Redemption.User.DisplayName, e.RewardRedeemed.Redemption.Reward.Prompt);
        }
    }

    private void OnRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
    {

        if (e.RewardTitle == RewardID)
        {
            Logger.Log("Request from " + e.DisplayName);

            TextBoxEnabler.PlayText(e.DisplayName, e.Message);
        }
    }

    private void OnLog(object sender, TwitchLib.PubSub.Events.OnLogArgs e)
    {
        //Debug.Log($"LOG : {e.Data}");
    }

    private void OnPubSubServiceError(object sender, TwitchLib.PubSub.Events.OnPubSubServiceErrorArgs e)
    {
        Logger.Log("Error : " + e.Exception.Message);
    }

    public void SetRewardName(string name)
    {
        RewardID = name;
    }

}
