namespace gmvTM.Server
{
    public static class gmvServer
    {
        private static readonly Messages messages = new Messages();
        private static readonly Security security = new Security();

        public static Messages Messages => messages;
        public static Security Security => security;
    }
}
