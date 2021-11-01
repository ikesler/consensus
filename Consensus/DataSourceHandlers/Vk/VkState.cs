namespace Consensus.DataSourceHandlers.Vk
{
    public class VkState
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public ulong Offset { get; set; }
        public bool IsHistoryDone { get; set; }
        public DateTime LastPostDate { get; set; }
    }
}
