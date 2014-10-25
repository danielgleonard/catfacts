
namespace Google.Voice.Entities
{
    public class ForwardingPhone : ShallowEntity
    {

        /*
         * {"id":7,
         * "name":"EVO 4G",
         * "phoneNumber":"+18048883365",
         * "type":10,
         * "verified":true,
         * "policyBitmask":1,
         * "dEPRECATEDDisabled":false,
         * "telephonyVerified":true,
         * "smsEnabled":true,
         * "incomingAccessNumber":"",
         * "voicemailForwardingVerified":false,
         * "behaviorOnRedirect":1,
         * "carrier":"SPRINT",
         * "customOverrideState":0,
         * "inVerification":false,
         * "lastVerificationDate":"1303424926757",
         * "provisionablePartner":1,
         * "recentlyProvisionedOrDeprovisioned":false,
         * "formattedNumber":"(804) 888-3365",
         * "wd":{"allDay":false,"times":[]},
         * "we":{"allDay":false,"times":[]},
         * "weekdayTimes":[],
         * "weekendTimes":[],
         * "scheduleSet":false,
         * "weekdayAllDay":false,
         * "weekendAllDay":false,
         * "redirectToVoicemail":true}
         */

        public string id { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public int type { get; set; }
        public bool verified { get; set; }
        public int policyBitmask { get; set; }
        public bool dEPRECATEDDisabled { get; set; }
        public bool telephonyVerified { get; set; }
        public bool smsEnabled { get; set; }
        public string incomingAccessNumber { get; set; }
        public int behaviorOnRedirect { get; set; }
        public string carrier { get; set; }
        public int customOverrideState { get; set; }
        public bool inVerification { get; set; }
        public long lastVerificationDate { get; set; }
        public int provisionablePartner { get; set; }
        public bool recentlyProvisionedOrDeprovisioned { get; set; }
        public string formattedNumber { get; set; }
        public bool scheduleSet { get; set; }
        public bool weekdayAllDay { get; set; }
        public bool weekendAllDay { get; set; }
        public bool redirectToVoicemail { get; set; }
        public override string ToString()
        {
            return name + " - " + formattedNumber;
        }
    }
}
