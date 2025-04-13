using System.ComponentModel.DataAnnotations;

namespace LibrarySystemKR.Models
{
    public class Reader
    {
        public int ReaderId { get; set; }

        private string _fullName = "Unknown Reader";
        private string _address = "Unknown Address";
        private string _phone = "Unknown Phone";

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]

        public string FullName
        {
            get => _fullName;
            set => _fullName = value ?? _fullName;
        }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Address
        {
            get => _address;
            set => _address = value ?? _address;
        }

        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Phone
        {
            get => _phone;
            set => _phone = value ?? _phone;
        }
    }

}
