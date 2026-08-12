namespace gmvTM.Server
{
    public readonly struct Security
    {
        public string AuthUsernameSetting => "Auth:Username";
        public string AuthPasswordHashSetting => "Auth:PasswordHashSha256";
        public string BasicSchemePrefix => "Basic ";
        public string BasicSchemeName => "basic";
        public string WwwAuthenticateValue => "Basic realm=\"gmvTM\"";
        public string ApiPathPrefix => "/api";
        public string ODataPathPrefix => "/odata";
    }
}
