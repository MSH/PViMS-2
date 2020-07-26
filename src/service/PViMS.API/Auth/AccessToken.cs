namespace PVIMS.API.Auth
{
    public sealed class AccessToken
    {
        public AccessToken()
        {

        }

        public AccessToken(string token, int expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
        }

        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
