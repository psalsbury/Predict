using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predict.RapidApiOdds
{
    public class Paging
    {
        public int current { get; set; }
        public int total { get; set; }
    }

    public class Fixture
    {
        public int league_id { get; set; }
        public int fixture_id { get; set; }
        public int updateAt { get; set; }
    }

    public class Value
    {
        public object value { get; set; }
        public string odd { get; set; }
    }

    public class Bet
    {
        public int label_id { get; set; }
        public string label_name { get; set; }
        public List<Value> values { get; set; }
    }

    public class Bookmaker
    {
        public int bookmaker_id { get; set; }
        public string bookmaker_name { get; set; }
        public List<Bet> bets { get; set; }
    }

    public class Odd
    {
        public Fixture fixture { get; set; }
        public List<Bookmaker> bookmakers { get; set; }
    }

    public class Api
    {
        public int results { get; set; }
        public Paging paging { get; set; }
        public List<Odd> odds { get; set; }
    }

    public class Root
    {
        public Api api { get; set; }
    }


}
