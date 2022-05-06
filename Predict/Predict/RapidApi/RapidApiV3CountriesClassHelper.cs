using System.Collections.Generic;

namespace Predict.RapidApi.RapidApiV3CountriesClassHelper
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Paging
    {
        public int current { get; set; }
        public int total { get; set; }
    }

    public class Response
    {
        public string name { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
    }

    public class Root
    {
        public string get { get; set; }
        public List<object> parameters { get; set; }
        public List<object> errors { get; set; }
        public int results { get; set; }
        public Paging paging { get; set; }
        public List<Response> response { get; set; }
    }


}