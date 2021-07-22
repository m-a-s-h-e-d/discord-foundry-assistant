using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Schema
{
    public class Balances
    {
        private readonly SchemaContext _context;

        public Balances(SchemaContext context)
        {
            _context = context;
        }

        public async Task ModifyBalance(ulong uid, long balanceChange)
        {
            var user = await _context.Balances
                .FindAsync(uid);

            if (user == null)
                _context.Add(new Balance { UserId = uid, Bal = balanceChange });
            else
                user.Bal += balanceChange;

            await _context.SaveChangesAsync();
        }

        public async Task<long> GetBalance(ulong uid)
        {
            if (await _context.Balances.FindAsync(uid) == null)
            {
                _context.Add(new Balance { UserId = uid, Bal = 0 });
                await _context.SaveChangesAsync();
            }

            var balance = await _context.Balances
                .Where(x => x.UserId == uid)
                .Select(x => x.Bal)
                .FirstOrDefaultAsync();

            return await Task.FromResult(balance);
        }

        public async Task ModifyLastClaim(ulong uid, DateTime lastClaimTime)
        {
            var user = await _context.Balances
                .FindAsync(uid);

            if (user == null)
                _context.Add(new Balance { UserId = uid, Bal = 0, LastClaim = lastClaimTime });
            else
                user.LastClaim = lastClaimTime;

            await _context.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastClaim(ulong uid)
        {
            var user = await _context.Balances
                .FindAsync(uid);

            if (user == null)
            {
                _context.Add(new Balance { UserId = uid, Bal = 0, LastClaim = null });
                await _context.SaveChangesAsync();
            }

            var lastClaim = await _context.Balances
                .Where(x => x.UserId == uid)
                .Select(x => x.LastClaim)
                .FirstOrDefaultAsync();

            return await Task.FromResult(lastClaim);
        }
    }
}
