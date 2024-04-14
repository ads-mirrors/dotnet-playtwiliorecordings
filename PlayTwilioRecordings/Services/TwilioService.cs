using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Collections.Generic;
using System.Linq;

public class TwilioService
{
    private string _accountSid;
    private string _authToken;

    public TwilioService(string accountSid, string authToken)
    {
        _accountSid = accountSid;
        _authToken = authToken;
        TwilioClient.Init(_accountSid, _authToken);
    }

    public List<RecordingResource> FetchRecordings()
    {
        var recordings = RecordingResource.Read(limit: 20).ToList();
        return recordings;
    }

    public RecordingResource GetRecordingById(string recordingSid)
    {
        try
        {
            var recording = RecordingResource.Fetch(pathSid: recordingSid);
            return recording;
        }
        catch (Twilio.Exceptions.TwilioException ex)
        {
            // Log error or handle exceptions as needed
            throw new ApplicationException("Failed to fetch recording with SID: " + recordingSid, ex);
        }
    }
}