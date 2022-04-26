using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.RapidApi.RapidApiV3LeagueClassHelper
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Paging
    {
        public int current { get; set; }
        public int total { get; set; }
    }

    public class League
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string logo { get; set; }
    }

    public class Country
    {
        public string name { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
    }

    public class Fixtures
    {
        public bool events { get; set; }
        public bool lineups { get; set; }
        public bool statistics_fixtures { get; set; }
        public bool statistics_players { get; set; }
    }

    public class Coverage
    {
        public Fixtures fixtures { get; set; }
        public bool standings { get; set; }
        public bool players { get; set; }
        public bool top_scorers { get; set; }
        public bool top_assists { get; set; }
        public bool top_cards { get; set; }
        public bool injuries { get; set; }
        public bool predictions { get; set; }
        public bool odds { get; set; }
    }

    public class Season
    {
        public int year { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public bool current { get; set; }
        public Coverage coverage { get; set; }
    }

    public class Response
    {
        public League league { get; set; }
        public Country country { get; set; }
        public List<Season> seasons { get; set; }
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