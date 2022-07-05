using System.ComponentModel.DataAnnotations;

namespace VitalenCase.CustomModels
{
    public class Credential
    {
        [Required(ErrorMessage = "Required.")]
        public string username { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string password { get; set; }
    }
    public class UserCRUD
    {
        public string username { get; set; }
        public string password { get; set; }
        public int id { get; set; }
    }
}
