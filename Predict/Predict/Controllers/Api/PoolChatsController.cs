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
        public JsonResult<System.Collections.Generic.List<PoolChat>> GetChats(int poolId, int lastId)
        {
            if (!User.Identity.IsAuthenticated)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var playerId = User.Identity.GetUserId();

            var poolPlayer = _context.PoolPlayers.FirstOrDefault(p => p.PoolId == poolId & p.PlayerId == playerId & p.Enabled == true);
            if (poolPlayer==null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var poolChats = _context.PoolChats
                .Include(p => p.Player)
                .Where(a => a.PoolId == poolId & a.Id >= lastId)
                .OrderBy(a => a.CreatedDateTime).ToList();

            if(poolChats != null && poolChats.Count>0)
            {
                if (poolPlayer.LastViewedPoolChatId != poolChats.Last().Id)
                {
                    poolPlayer.LastViewedPoolChatId = poolChats.Last().Id;
                    poolPlayer.ModifiedDateTime = DateTime.UtcNow;
                    _context.PoolPlayers.AddOrUpdate(poolPlayer);
                    _context.SaveChanges();
                }
            }
            return Json(poolChats);

        }


        [HttpGet]
        [Route("api/PoolChats/GetMostRecentChatId/{PoolId}")]
        public int GetMostRecentChatId(int poolId)
        {
            int latestChat = 0;
            var myObect = Helper.Cache.GetCachedItem("Chat*" + poolId);
            if(myObect!=null)
            {
                latestChat = (int)myObect;
            }

            return latestChat;
    
        }


        [HttpPost]
        public int AddPoolChat(newChat obj)
        {

            if (!User.Identity.IsAuthenticated)
                throw new Exception("User Is Not authenticated");

            var playerId = User.Identity.GetUserId();

            var poolPlayer = _context.PoolPlayers.FirstOrDefault(p => p.PoolId == obj.poolId & p.PlayerId==playerId & p.Enabled==true);
            if (poolPlayer==null)
                throw new Exception("Not a member of the pool");

            var dteNow = DateTime.UtcNow;

            var poolChat = new PoolChat
            {
                PlayerId = playerId,
                PoolId = obj.poolId,
                CreatedDateTime = dteNow,
                ModifiedDateTime = dteNow,
                Message = obj.message
            };

            _context.PoolChats.Add(poolChat);
            _context.SaveChanges();

            poolPlayer.LastViewedPoolChatId = poolChat.Id;
            poolPlayer.ModifiedDateTime = dteNow;
            _context.PoolPlayers.AddOrUpdate(poolPlayer);
            _context.SaveChanges();


            Helper.Cache.SetCachedItem("Chat*" + obj.poolId, poolChat.Id);

            return poolChat.Id;
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
