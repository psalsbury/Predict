using Predict.Models;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Predict.Controllers.Api
{
    public class PoolsController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //todo  add api to validate that entry code is correct. Only allow from specific page

        // DELETE: api/Pools/5
        [ResponseType(typeof(Pool))]
        public IHttpActionResult DeletePool(int id)
        {
            var pool = _context.Pools.Find(id);
            if (pool == null) return NotFound();

            var poolPlayers = _context.EventPoolPlayers.Where(b => b.PoolId == id);
            foreach (var poolPlayer in poolPlayers) _context.EventPoolPlayers.Remove(poolPlayer);

            _context.Pools.Remove(pool);
            _context.SaveChanges();

            return Ok(pool);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}