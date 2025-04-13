using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystemKR.Models
{
    public class BookHistory
    {
        public int HistoryId { get; set; }
        public int LibraryId { get; set; }
        public int BookId { get; set; }

        private string _actionType = "Checked Out";

        public DateTime ActionDate { get; set; } = DateTime.Now;

        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string ActionType
        {
            get => _actionType;
            set => _actionType = value ?? _actionType;
        }

        // Навигационное свойство
        public Book Book { get; set; }  
    }

}
