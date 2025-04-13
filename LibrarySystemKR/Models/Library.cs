using System.ComponentModel.DataAnnotations;

namespace LibrarySystemKR.Models
{
    public class Library
    {
        public int LibraryId { get; set; }

        private string _name = "Unnamed Library";
        private string _address = "Unknown Address";

        [Required(ErrorMessage = "Library name is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Name
        {
            get => _name;
            set => _name = value ?? _name;
        }

        [Required(ErrorMessage = "Library address is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Address
        {
            get => _address;
            set => _address = value ?? _address;
        }
        public DateTime LastUpdated { get; set; }  // Добавляем поле LastUpdated

        // Навигационные свойства
        public ICollection<Book> Books { get; set; }
    }
}
