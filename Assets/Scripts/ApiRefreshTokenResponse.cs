using System;

[Serializable]
public class ApiRefreshTokenResponse
{
    public string access_token;
    public string refresh_token;
    public string[] scope;
    public string token_type;
}