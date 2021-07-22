using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Schema
{
    public class ServerUsers
    {
        private readonly SchemaContext _context;

        public ServerUsers(SchemaContext context)
        {
            _context = context;
        }

        public async Task AddServerUser(ulong uid, ulong serverId)
        {
            var entry = await _context.ServerUsers
                .Where(x => x.ServerId == serverId && x.UserId == uid)
                .FirstOrDefaultAsync();

            if (entry == null)
                _context.Add(new ServerUser { ServerId = serverId, UserId = uid });

            await _context.SaveChangesAsync();
        }

        public async Task<ServerUser> FindServerUser(ulong uid, ulong serverId)
        {
            var entry = await _context.ServerUsers
                .Where(x => x.ServerId == serverId && x.UserId == uid)
                .FirstOrDefaultAsync();

            if (entry == null)
            {
                _context.Add(new ServerUser { ServerId = serverId, UserId = uid });
                await _context.SaveChangesAsync();
            }

            return await Task.FromResult(entry);
        }
    }
}
