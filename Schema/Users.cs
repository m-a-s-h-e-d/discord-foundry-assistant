using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Schema
{
    public class Users
    {
        private readonly SchemaContext _context;

        public Users(SchemaContext context)
        {
            _context = context;
        }

        public async Task ModifyUsername(ulong uid, string userName)
        {
            var user = await _context.Users
                .FindAsync(uid);

            if (user == null)
            {
                var rnd = new Random();
                var newColor = $"{rnd.Next(0x1000000):X6}";
                _context.Add(new User { UserId = uid, Username = userName, EmbedColor = newColor, RepCount = 0, LastRep = null });
            }
            else
                user.Username = userName;

            await _context.SaveChangesAsync();
        }

        public async Task<string> GetUsername(ulong uid, string userName)
        {
            var user = await _context.Users
                .FindAsync(uid);

            if (user == null)
            {
                var rnd = new Random();
                var newColor = $"{rnd.Next(0x1000000):X6}";
                _context.Add(new User { UserId = uid, Username = userName, EmbedColor = newColor, RepCount = 0, LastRep = null });
                await _context.SaveChangesAsync();
            }

            var name = await _context.Users
                .Where(x => x.UserId == uid)
                .Select(x => x.Username)
                .FirstOrDefaultAsync();

            return await Task.FromResult(name);
        }

        public async Task ModifyEmbedColor(ulong uid, string hexColor, string userName)
        {
            var user = await _context.Users
                .FindAsync(uid);

            if (user == null)
                _context.Add(new User { UserId = uid, Username = userName, EmbedColor = hexColor, RepCount = 0, LastRep = null });
            else
                user.EmbedColor = hexColor;

            await _context.SaveChangesAsync();
        }

        public async Task<uint> GetEmbedColor(ulong uid, string userName)
        {
            var user = await _context.Users
                .FindAsync(uid);

            if (user == null)
            {
                var rnd = new Random();
                var newColor = $"{rnd.Next(0x1000000):X6}";
                _context.Add(new User { UserId = uid, Username = userName, EmbedColor = newColor, RepCount = 0, LastRep = null });
                await _context.SaveChangesAsync();
            }

            var color = await _context.Users
                .Where(x => x.UserId == uid)
                .Select(x => x.EmbedColor)
                .FirstOrDefaultAsync();

            return await Task.FromResult(uint.Parse(color, System.Globalization.NumberStyles.HexNumber));
        }

        public async Task ModifyRep(ulong uid, int repAmount, string userName)
        {
            var user = await _context.Users
                .FindAsync(uid);

            if (user == null)
            {
                var rnd = new Random();
                var newColor = $"{rnd.Next(0x1000000):X6}";
                _context.Add(new User { UserId = uid, Username = userName, EmbedColor = newColor, RepCount = repAmount, LastRep = null });
            }
            else
                user.RepCount += repAmount;

            await _context.SaveChangesAsync();
        }

        public async Task<long> GetRepCount(ulong uid, string userName)
        {
            var user = await _context.Users
                .FindAsync(uid);

            if (user == null)
            {
                var rnd = new Random();
                var newColor = $"{rnd.Next(0x1000000):X6}";
                _context.Add(new User { UserId = uid, Username = userName, EmbedColor = newColor, RepCount = 0, LastRep = null });
                await _context.SaveChangesAsync();
            }

            var repCount = await _context.Users
                .Where(x => x.UserId == uid)
                .Select(x => x.RepCount)
                .FirstOrDefaultAsync();

            return await Task.FromResult(repCount);
        }

        public async Task ModifyLastRep(ulong uid, DateTime lastRepTime, string userName)
        {
            var user = await _context.Users
                .FindAsync(uid);

            if (user == null)
            {
                var rnd = new Random();
                var newColor = $"{rnd.Next(0x1000000):X6}";
                _context.Add(new User { UserId = uid, Username = userName, EmbedColor = newColor, RepCount = 0, LastRep = lastRepTime });
            }
            else
                user.LastRep = lastRepTime;

            await _context.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastRep(ulong uid, string userName)
        {
            var user = await _context.Users
                .FindAsync(uid);

            if (user == null)
            {
                var rnd = new Random();
                var newColor = $"{rnd.Next(0x1000000):X6}";
                _context.Add(new User { UserId = uid, Username = userName, EmbedColor = newColor, RepCount = 0, LastRep = null });
                await _context.SaveChangesAsync();
            }

            var lastRep = await _context.Users
                .Where(x => x.UserId == uid)
                .Select(x => x.LastRep)
                .FirstOrDefaultAsync();

            return await Task.FromResult(lastRep);
        }
    }
}
