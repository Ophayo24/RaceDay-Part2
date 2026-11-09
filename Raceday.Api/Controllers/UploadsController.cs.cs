using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raceday.Api.Services;

namespace Raceday.Api.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize]
public class UploadsController : ControllerBase
{
    private readonly IBlobStorageService _blob;
    public UploadsController(IBlobStorageService blob) => _blob = blob;

    private static readonly string[] AllowedExts = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxBytes = 5 * 1024 * 1024;

    /// <summary>Uploads an event banner image. Organisers only.</summary>
    [HttpPost("event-banner")]
    [Authorize(Roles = "Organiser")]
    [RequestSizeLimit(MaxBytes)]
    public async Task<IActionResult> UploadEventBanner(IFormFile file)
    {
        var (ok, err) = Validate(file);
        if (!ok) return BadRequest(new { error = err });

        await using var stream = file.OpenReadStream();
        var url = await _blob.UploadAsync(stream, file.FileName, file.ContentType);
        return Ok(new { url });
    }

    /// <summary>Uploads a profile picture. Participants and Organisers.</summary>
    [HttpPost("profile-picture")]
    [RequestSizeLimit(MaxBytes)]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        var (ok, err) = Validate(file);
        if (!ok) return BadRequest(new { error = err });

        await using var stream = file.OpenReadStream();
        var url = await _blob.UploadAsync(stream, file.FileName, file.ContentType);
        return Ok(new { url });
    }
    private static (bool ok, string? err) Validate(IFormFile? file)
    {
        if (file == null || file.Length == 0) return (false, "No file supplied.");
        if (file.Length > MaxBytes) return (false, "File exceeds 5 MB.");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExts.Contains(ext)) return (false, "Only .jpg, .jpeg, .png, .webp allowed.");
        return (true, null);
    }
}
