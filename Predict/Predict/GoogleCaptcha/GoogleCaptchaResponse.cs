using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Predict.GoogleCaptcha
{
    public class GoogleCaptchaResponse
    {

        // GoogleCaptchaResponse myDeserializedClass = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(myJsonResponse); 

        public bool success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
}