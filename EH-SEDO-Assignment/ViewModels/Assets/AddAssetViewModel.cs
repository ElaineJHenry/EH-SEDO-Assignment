using System.ComponentModel.DataAnnotations;

namespace EH_SEDO_Assignment.ViewModels.Assets
{
    public class AddAssetViewModel
    {
        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is Required")]
        [MaxLength(30)]
        public string Type { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Value is Required")]
        [Range(0, 1000000, ErrorMessage = "Value must be a positive value between 0 and 1,000,000")]
        [Display(Name = "Value (£)")]
        public double Value {  get; set; }

        [Required(ErrorMessage = "Acquisition Date is Required")]
        [DataType(DataType.Date)]
        [Display(Name = "Acquisition Date")]
        public DateTime? AcquisitionDate { get; set; }
    }
}
