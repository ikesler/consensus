namespace Consensus.ApiContracts
{
    public class Pipe
    {
        public Guid PublicId { get; set; }
        public string DataSourceCode { get; set; }
        public string PropsJson { get; set; }
        public string? StateJson { get; set; }
    }
}
