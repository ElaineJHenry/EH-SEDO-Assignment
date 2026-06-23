using System.ComponentModel.DataAnnotations;

namespace EH_SEDO_Assignment.ViewModels.Assets
{
    public class AddAssetViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        public string Type { get; set; }

        public string Description { get; set; }


        [Required(ErrorMessage = "Value is Required")]
        public double Value {  get; set; }
        public DateTime AcquisitionDate { get; set; }
    }
}
