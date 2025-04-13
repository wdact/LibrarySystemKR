using System.ComponentModel.DataAnnotations;

namespace LibrarySystemKR.Models
{
    public class Subscription
    {
        public int LibraryId { get; set; }
        public int BookId { get; set; }
        public int ReaderId { get; set; }

        [Required(ErrorMessage = "Advance payment is required.")]
        public decimal Advance { get; set; } = 0;

        [Required(ErrorMessage = "Issue date is required.")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Return date is required.")]
        public DateTime ReturnDate { get; set; } = DateTime.Now.AddMonths(1);

        // Навигационные свойства
        public Library Library { get; set; }
        public Book Book { get; set; }
        public Reader Reader { get; set; }
    }

}
