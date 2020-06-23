using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Predict.Models;

namespace Predict.Controllers.Api
{
    public class TeamsController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public TeamsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET /api/teams
        public IEnumerable<Team> GetTeams()
        {
            return _context.Teams.ToList(); // NEED TO DO THIS FOR AUTOMAPPER //
        }
    }
}