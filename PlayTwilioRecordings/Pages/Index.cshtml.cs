using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Twilio.Rest.Api.V2010.Account;

public class IndexModel : PageModel
{
    private readonly TwilioService _twilioService;
    private readonly RecordingManager _recordingManager;

    public List<RecordingResource> Recordings { get; set; }

    public IndexModel(TwilioService twilioService, RecordingManager recordingManager)
    {
        _twilioService = twilioService;
        _recordingManager = recordingManager;
    }

    public async Task OnGetAsync()
    {
        Recordings = _twilioService.FetchRecordings();
    }

    public async Task<IActionResult> OnGetAudioAsync(string id)
    {
        var recording = _twilioService.GetRecordingById(id);
        var encryptedData = await _recordingManager.GetRecordingAsync(recording.Uri);
        //var decryptedData = _recordingManager.DecryptData(encryptedData, recording.EncryptedKey, recording.IV);
        var decryptedData = _recordingManager.DecryptData(encryptedData, "recording.EncryptedKey", "recording.IV");
        return File(decryptedData, "audio/mp4");
    }
}