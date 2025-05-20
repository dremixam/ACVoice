/*
 * Simple Twitch OAuth flow example
 * by HELLCAT
 *
 * At first glance, this looks like more than it actually is.
 * It's really no rocket science, promised! ;-)
 * And for any further questions contact me directly or on the Twitch-Developers discord.
 *
 * 🐦 https://twitter.com/therealhellcat
 * 📺 https://www.twitch.tv/therealhellcat
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TwitchOAuth : MonoBehaviour
{
    [SerializeField] private string twitchAuthUrl = "https://id.twitch.tv/oauth2/authorize";
    [SerializeField] private string twitchValidateUrl = "https://id.twitch.tv/oauth2/validate";
    [SerializeField] private string twitchRefreshTokenUrl = "https://id.twitch.tv/oauth2/token";
    [SerializeField] private string twitchClientId = "PUT YOUR CLIENT ID HERE";
    [SerializeField] private string twitchClientSecret = "PUT YOUR CLIENT SECRET HERE";
    [SerializeField] private string twitchRedirectUrl = "http://localhost:8080/";
    [SerializeField] private TwitchApiCallHelper twitchApiCallHelper = null;
    [SerializeField] private Button LoginButton;
    [SerializeField] private PubSubBehaviour PubSubBehaviour;
    private string _twitchAuthStateVerify;
    private string _authToken = "";
    private string _userID = "";
    private string _refreshToken = "";

    private bool _authenticated = false;
    private bool _justAuthenticated = false;

    private string _authCode = "";

    private void Start()
    {
        _authToken = PlayerPrefs.GetString("oauth", "");
        _refreshToken = PlayerPrefs.GetString("oauth:refresh", "");

        SetButtonText(LoginButton, "Twitch Login");
    }

    private void Update()
    {
        if (_authCode != "")
        {
            GetOAuthToken(_authCode);
        }
        if (_justAuthenticated)
        {
            Logger.Log("Authenticated");
            PlayerPrefs.SetString("oauth", _authToken);
            PlayerPrefs.SetString("oauth:refresh", _refreshToken);
            _justAuthenticated = false;
            SetButtonText(LoginButton, "Twitch Logout");
            PubSubBehaviour.Subscribe();
        }
        else if (!_authenticated && _authToken != "")
        {
            if (_userID == "" || _refreshToken == "")
            {
                Logger.Log("Authentication need refresh");
                RefreshToken();
                GetUserIDFromAuth(_authToken);
            }
            else
            {
                SetButtonText(LoginButton, "Twitch Logout");
                _authenticated = true;
            }
        }
    }

    private void SetButtonText(Button button, string text)
    {
        TMP_Text tmp_text = button.GetComponentInChildren<TMP_Text>();
        if (tmp_text != null)
        {
            tmp_text.text = text;
        }
    }

    public void ToggleAuth()
    {
        if (_authenticated)
        {
            _authenticated = false;
            _authToken = "";
            _userID = "";
            _refreshToken = "";
            PlayerPrefs.SetString("oauth", _authToken);
            PlayerPrefs.SetString("oauth:refresh", _refreshToken);
            SetButtonText(LoginButton, "Twitch Login");
            PubSubBehaviour.Revoke();
        }
        else
        {
            InitiateTwitchAuth();
        }
    }

    /// <summary>
    /// Starts the Twitch OAuth flow by constructing the Twitch auth URL based on the scopes you want/need.
    /// </summary>
    public void InitiateTwitchAuth()
    {
        string[] scopes;
        string s;

        Logger.Log("Initiating Auth");

        // list of scopes we want
        scopes = new[]
        {
            "user:read:email",
            "chat:edit",
            "chat:read",
            "channel:read:redemptions",
            "channel_subscriptions",
            "user:read:broadcast",
            "user:edit:broadcast",
            "channel:manage:redemptions"
        };

        // generate something for the "state" parameter.
        // this can be whatever you want it to be, it's gonna be "echoed back" to us as is and should be used to
        // verify the redirect back from Twitch is valid.
        _twitchAuthStateVerify = ((Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();

        // query parameters for the Twitch auth URL
        s = "client_id=" + twitchClientId + "&" +
            "redirect_uri=" + UnityWebRequest.EscapeURL(twitchRedirectUrl) + "&" +
            "state=" + _twitchAuthStateVerify + "&" +
            "response_type=code&" +
            "scope=" + String.Join("+", scopes);

        // start our local webserver to receive the redirect back after Twitch authenticated
        StartLocalWebserver();

        // open the users browser and send them to the Twitch auth URL
        Application.OpenURL(twitchAuthUrl + "?" + s);
    }

    /// <summary>
    /// Opens a simple "webserver" like thing on localhost:8080 for the auth redirect to land on.
    /// Based on the C# HttpListener docs: https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener
    /// </summary>
    private void StartLocalWebserver()
    {
        HttpListener httpListener = new HttpListener();

        httpListener.Prefixes.Add(twitchRedirectUrl);
        httpListener.Start();
        httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest), httpListener);
    }

    /// <summary>
    /// Handles the incoming HTTP request
    /// </summary>
    /// <param name="result"></param>
    private void IncomingHttpRequest(IAsyncResult result)
    {
        string code;
        string state;
        HttpListener httpListener;
        HttpListenerContext httpContext;
        HttpListenerRequest httpRequest;
        HttpListenerResponse httpResponse;
        string responseString;

        // get back the reference to our http listener
        httpListener = (HttpListener)result.AsyncState;

        // fetch the context object
        httpContext = httpListener.EndGetContext(result);

        // if we'd like the HTTP listener to accept more incoming requests, we'd just restart the "get context" here:
        // httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest),httpListener);
        // however, since we only want/expect the one, single auth redirect, we don't need/want this, now.
        // but this is what you would do if you'd want to implement more (simple) "webserver" functionality
        // in your project.

        // the context object has the request object for us, that holds details about the incoming request
        httpRequest = httpContext.Request;

        code = httpRequest.QueryString.Get("code");
        state = httpRequest.QueryString.Get("state");

        // check that we got a code value and the state value matches our remembered one
        if ((code.Length > 0) && (state == _twitchAuthStateVerify))
        {
            // if all checks out, use the code to exchange it for the actual auth token at the API
            //GetTokenFromCode(code);
            //GetUserIDFromAuth(_authToken);
            _authCode = code;
        }

        // build a response to send an "ok" back to the browser for the user to see
        httpResponse = httpContext.Response;
        responseString = "<html><body><b>DONE!</b><br>(You can close this tab/window now)</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

        // send the output to the client browser
        httpResponse.ContentLength64 = buffer.Length;
        System.IO.Stream output = httpResponse.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        // the HTTP listener has served it's purpose, shut it down
        httpListener.Stop();
        // obv. if we had restarted the waiting for more incoming request, above, we'd not Stop() it here.

    }

    /// <summary>
    /// Makes the API call to exchange the received code for the actual auth token
    /// </summary>
    /// <param name="code">The code parameter received in the callback HTTP reuqest</param>
    private void GetTokenFromCode(string code)
    {
        string apiUrl;
        string apiResponseJson;
        ApiCodeTokenResponse apiResponseData;

        // construct full URL for API call
        apiUrl = "https://id.twitch.tv/oauth2/token" +
                 "?client_id=" + twitchClientId +
                 "&client_secret=" + twitchClientSecret +
                 "&code=" + code +
                 "&grant_type=authorization_code" +
                 "&redirect_uri=" + UnityWebRequest.EscapeURL(twitchRedirectUrl);

        // make sure our API helper knows our client ID (it needed for the HTTP headers)
        twitchApiCallHelper.TwitchClientId = twitchClientId;

        // make the call!
        apiResponseJson = twitchApiCallHelper.CallApi(apiUrl, "POST");

        // parse the return JSON into a more usable data object
        apiResponseData = JsonUtility.FromJson<ApiCodeTokenResponse>(apiResponseJson);

        // fetch the token from the response data
        _authToken = apiResponseData.access_token;
        _refreshToken = apiResponseData.refresh_token;
        _authenticated = true;
        _justAuthenticated = true;
        //
    }

    private void GetOAuthToken(string code)
    {
        GetTokenFromCode(code);
        GetUserIDFromAuth(_authToken);
        _authCode = "";
    }

    private void GetUserIDFromAuth(string auth)
    {
        string apiUrl;
        string apiResponseJson;
        ApiValidateResponse apiResponseData;

        // construct full URL for API call
        apiUrl = twitchValidateUrl;

        // make sure our API helper knows our client ID (it needed for the HTTP headers)
        twitchApiCallHelper.TwitchAuthToken = auth;

        // make the call!
        apiResponseJson = twitchApiCallHelper.CallApi(apiUrl, "GET");

        // parse the return JSON into a more usable data object
        apiResponseData = JsonUtility.FromJson<ApiValidateResponse>(apiResponseJson);


        // fetch the token from the response data


        if (apiResponseData.user_id == "")
        {
            _userID = "";
            _authToken = "";
        }
        else
        {
            _userID = apiResponseData.user_id;
        }

        //
    }


    private void RefreshToken()
    {
        /*
        POST https://id.twitch.tv/oauth2/token
    --data-urlencode
    ?grant_type=refresh_token
    &refresh_token=<your refresh token>
    &client_id=<your client ID>
    &client_secret=<your client secret>

            */

        string apiUrl;
        string apiResponseJson;
        ApiRefreshTokenResponse apiResponseData;

        // construct full URL for API call
        apiUrl = twitchRefreshTokenUrl +
                 "?client_id=" + twitchClientId +
                 "&client_secret=" + twitchClientSecret +
                 "&refresh_token=" + _refreshToken +
                 "&grant_type=refresh_token";

        twitchApiCallHelper.TwitchClientId = twitchClientId;

        // make the call!
        apiResponseJson = twitchApiCallHelper.CallApi(apiUrl, "POST");

        // parse the return JSON into a more usable data object
        apiResponseData = JsonUtility.FromJson<ApiRefreshTokenResponse>(apiResponseJson);


        // fetch the token from the response data
        _authToken = apiResponseData.access_token;
        _refreshToken = apiResponseData.refresh_token;
        _authenticated = true;
        _justAuthenticated = true;
    }


    public string GetAuthToken()
    {
        return _authToken;
    }

    public string GetUserID()
    {
        return _userID;
    }
}
