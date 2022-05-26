using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.RapidApi.RapidApiV3OddsClassHelper
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Bet
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Value> values { get; set; }
    }

    public class Bookmaker
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Bet> bets { get; set; }
    }

    public class Fixture
    {
        public int id { get; set; }
        public string timezone { get; set; }
        public DateTime date { get; set; }
        public int timestamp { get; set; }
    }

    public class League
    {
        public int id { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string logo { get; set; }
        public string flag { get; set; }
        public int season { get; set; }
    }

    public class Paging
    {
        public int current { get; set; }
        public int total { get; set; }
    }

    public class Parameters
    {
        public string league { get; set; }
        public string season { get; set; }
    }

    public class Response
    {
        public League league { get; set; }
        public Fixture fixture { get; set; }
        public DateTime update { get; set; }
        public List<Bookmaker> bookmakers { get; set; }
    }

    public class Root
    {
        public string get { get; set; }
        public Parameters parameters { get; set; }
        public List<object> errors { get; set; }
        public int results { get; set; }
        public Paging paging { get; set; }
        public List<Response> response { get; set; }
    }

    public class Value
    {
        public object value { get; set; }
        public string odd { get; set; }
    }

}