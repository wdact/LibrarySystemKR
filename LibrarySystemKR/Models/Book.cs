using System.ComponentModel.DataAnnotations;

namespace LibrarySystemKR.Models;

public class Book
{
    [Key]
    public int BookId { get; init; }

    // Поля обязательные для заполнения
    [Required(ErrorMessage = "Library is required.")]
    public int LibraryId { get; init; }

    [Required(ErrorMessage = "Subject is required.")]
    public int SubjectId { get; init; }

    private string _title = "Unknown Title"; // Значение по умолчанию
    private string _author = "Unknown Author"; // Значение по умолчанию
    private string _publisher = "Unknown Publisher"; // Значение по умолчанию
    private string _placeOfPublication = "Unknown Place"; // Значение по умолчанию
    private int _yearOfPublication = 2025; // Значение по умолчанию
    private int _quantity = 1; // Значение по умолчанию

    // Название книги
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string Title
    {
        get => _title;
        set => _title = value ?? _title;
    }

    // Автор книги
    [Required(ErrorMessage = "Author is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string Author
    {
        get => _author;
        set => _author = value ?? _author;
    }

    // Издатель
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string Publisher
    {
        get => _publisher;
        set => _publisher = value ?? _publisher;
    }

    // Место издания
    [StringLength(200, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string PlaceOfPublication
    {
        get => _placeOfPublication;
        set => _placeOfPublication = value ?? _placeOfPublication;
    }

    // Год издания
    public int YearOfPublication
    {
        get => _yearOfPublication;
        set => _yearOfPublication = value == 0 ? 2025 : value;
    }

    // Количество экземпляров книги
    public int Quantity
    {
        get => _quantity;
        set => _quantity = value < 1 ? 1 : value;
    }

    // Навигационные свойства
    public Library Library { get; set; }
    public Subject Subject { get; set; }
}