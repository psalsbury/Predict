using Microsoft.Ajax.Utilities;
using Predict.Models;
using System;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;

namespace Predict.Controllers.Api
{

    public class newChat
    {
        public int poolId { get; set; }
        public string message { get; set; }
    }
    public class PoolChatsController : ApiController
    {

        private readonly ApplicationDbContext _context;
        public PoolChatsController()
        {
            _context = new ApplicationDbContext();
        }


        [HttpGet]
        [Route("api/PoolChats/GetChats/{PoolId}/{playerId}")]
        public JsonResult<System.Collections.Generic.List<PoolChat>> GetChats(int poolId, string playerId)
        {
            if (!User.Identity.IsAuthenticated)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            if (playerId != User.Identity.GetUserId())
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var exists = _context.PoolPlayers.Any(p => p.PoolId == poolId & p.PlayerId == playerId & p.Enabled == true);
            if (!exists)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var poolChats = _context.PoolChats.Where(a => a.PoolId == poolId).OrderBy(a => a.CreatedDateTime).ToList();

            return Json(poolChats);

        }

        [HttpPost]
        public void AddPoolChat(newChat obj)
        {

            if (!User.Identity.IsAuthenticated)
                return ;

            var playerId = User.Identity.GetUserId();

            var exists = _context.PoolPlayers.Any(p => p.PoolId == obj.poolId & p.PlayerId==playerId & p.Enabled==true);
            if (!exists)
                return;

            var poolChat = new PoolChat
            {
                PlayerId = playerId,
                PoolId = obj.poolId,
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                Message = obj.message
            };

            _context.PoolChats.Add(poolChat);
            _context.SaveChanges();

            return;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
