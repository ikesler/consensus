namespace Consensus.DataSourceHandlers.Vk
{
    public class AccessTokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int user_id { get; set; }
    }
}
