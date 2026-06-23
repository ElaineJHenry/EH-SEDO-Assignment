namespace EH_SEDO_Assignment.Models
{
    public class AssetListInfo
    {
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetType { get; set; }
        public string AssetStatus { get; set; }
        public string AssetAssignment {  get; set; }
        public bool InUse { get; set; }
        public string InUseText { get; set; }
    }
}
