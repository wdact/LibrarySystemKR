using System.ComponentModel.DataAnnotations;

namespace LibrarySystemKR.Models;

public class Subject
{
    public int SubjectId { get; set; }

    private string _name = "Unknown Subject";

    [Required(ErrorMessage = "Subject name is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string Name
    {
        get => _name;
        set => _name = value ?? _name;
    }
}