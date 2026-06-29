namespace MinuteSheetFFC.Domain.Entities;

public class Attachment
{
    public int Id { get; set; }
    public int MinuteSheetId { get; set; }
    public string FileName { get; set; } = null!;
    public string? FileType { get; set; }
    public long FileSize { get; set; }
    public string FilePath { get; set; } = null!;
    public string? UploadedByPNo { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public MinuteSheetRequest MinuteSheet { get; set; } = null!;
}
