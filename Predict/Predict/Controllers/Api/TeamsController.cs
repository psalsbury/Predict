using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Predict.Models;
using Predict.Dtos;

namespace Predict.Controllers.Api
{
    public class TeamsController : ApiController
    {
        private ApplicationDbContext _context;

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
