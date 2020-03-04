using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Predict.Dtos;
using Predict.Models;

namespace Predict.Controllers.Api
{
    public class PlayersController : ApiController
    {
        private ApplicationDbContext _context;

        public PlayersController()
        {
            _context = new ApplicationDbContext();
        }

        //GET /api/players
        public IEnumerable<PlayerDto> GetPlayers()
        {
            return _context.Players.ToList().Select(Mapper.Map<Player, PlayerDto>);
        }

        //GET /api/players/1
        public IHttpActionResult GetPlayer(string id)
        {
            var player = _context.Players.SingleOrDefault(c => c.Id == id);

            if (player == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return Ok(Mapper.Map<Player, PlayerDto>(player));
        }

        // NOT NEEDED AS ONLY EVER CALLED FROM ACCOUNT CONTROLLER
        ////POST/api/players
        //[HttpPost]
        //public IHttpActionResult CreatePlayer(PlayerDto playerDto)
        //{
        //    if (!ModelState.IsValid)
        //        throw new HttpResponseException(HttpStatusCode.BadRequest);

        //    var player = Mapper.Map<PlayerDto, Player>(playerDto);
        //    _context.Players.Add(player);
        //    _context.SaveChanges();

        //    playerDto.Id = player.Id;
        //    return Created(new Uri(Request.RequestUri + "/" + player.Id), playerDto);
            
        //}

        //PUT /api/players/1
        [HttpPut]
        public IHttpActionResult UpdatePlayer(string id, PlayerDto playerDto)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var playerInDb = _context.Players.SingleOrDefault(c => c.Id == id);

            if (playerInDb== null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(playerDto, playerInDb);
            _context.SaveChanges();

            return Ok();
        }

        //DELETE /api/players/1
        [HttpDelete]
        public IHttpActionResult DeletePlayer(string id)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var playerInDb = _context.Players.SingleOrDefault(c => c.Id == id);

            if (playerInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Players.Remove(playerInDb);
            _context.SaveChanges();
            return Ok();
        }
    }
}
