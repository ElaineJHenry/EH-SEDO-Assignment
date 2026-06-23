using EH_SEDO_Assignment.Models;
using System.ComponentModel.DataAnnotations;

namespace EH_SEDO_Assignment.ViewModels.Assets
{
    public class AssetDetailViewModel
    {

        [Required]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is Required")]
        public string Type {  get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Value is Required")]
        public double AssetValue { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AcquisitionDate { get; set; }

        [Required(ErrorMessage = "Status is Required")]
        public string Status { get; set; }

        [Required]
        public bool InUse {  get; set; }

        public bool CanCheckInAsset { get; set; }

        public bool ShowAlert { get; set; }
        public string AlertMessage { get; set; }

        public List<AssetAssignmentHistoryModel> AssignmentHistory { get; set; }
    }
}
