using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Schema
{
    public class Experiences
    {
        private readonly SchemaContext _context;

        public Experiences(SchemaContext context)
        {
            _context = context;
        }

        public async Task ModifyExp(ulong uid, long expChange)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
                _context.Add(new Experience { UserId = uid, Exp = expChange, Level = 1 });
            else
                user.Exp += expChange;

            await _context.SaveChangesAsync();

            // Check if the user can level up
            //TODO await ModifyLevel(uid);
        }

        public async Task<long> GetExp(ulong uid)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
            {
                _context.Add(new Experience { UserId = uid, Exp = 0, Level = 1 });
                await _context.SaveChangesAsync();
            }

            var experience = await _context.Experiences
                .Where(x => x.UserId == uid)
                .Select(x => x.Exp)
                .FirstOrDefaultAsync();

            return await Task.FromResult(experience);
        }

        public async Task<long> GetRemainingExpToNextLevel(ulong uid)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
            {
                _context.Add(new Experience { UserId = uid, Exp = 0, Level = 1 });
                await _context.SaveChangesAsync();
            }

            var currentExp = await GetExp(uid);
            var nextLevel = await GetLevel(uid) + 1;
            var nextLevelExp = (5 * (nextLevel ^ 2) + (25 * nextLevel) + 100);

            return await Task.FromResult(nextLevelExp - currentExp);
        }

        public async Task<long> GetExpToNextLevel(ulong uid)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
            {
                _context.Add(new Experience { UserId = uid, Exp = 0, Level = 1 });
                await _context.SaveChangesAsync();
            }

            var nextLevel = await GetLevel(uid) + 1;
            var nextLevelExp = (5 * (nextLevel ^ 2) + (25 * nextLevel) + 100);

            return await Task.FromResult(nextLevelExp);
        }

        public async Task CheckThreshold(ulong uid)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
            {
                _context.Add(new Experience { UserId = uid, Exp = 0, Level = 1 });
                await _context.SaveChangesAsync();
                return;
            }

            var currentExp = user.Exp;
            var currentLevel = user.Level;

            while (currentExp / (5 * (currentLevel ^ 2) + (25 * currentLevel) + 100) > 0)
            {
                // Modify local variables
                currentExp -= (5 * (currentLevel ^ 2) + (25 * currentLevel) + 100);
                currentLevel++;

                // Modify in database
                user.Exp = currentExp;
                user.Level = currentLevel;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ModifyLevel(ulong uid, int levelChange)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
                _context.Add(new Experience { UserId = uid, Level = levelChange + 1, Exp = 0, LastMessage = null });
            else
                user.Level += levelChange;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetLevel(ulong uid)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
            {
                _context.Add(new Experience { UserId = uid, Exp = 0, Level = 1 });
                await _context.SaveChangesAsync();
            }

            var level = await _context.Experiences
                .Where(x => x.UserId == uid)
                .Select(x => x.Level)
                .FirstOrDefaultAsync();

            return await Task.FromResult(level);
        }

        public async Task ModifyLastMessage(ulong uid, DateTime lastMessageTime)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
                _context.Add(new Experience { UserId = uid, Level = 1, Exp = 0, LastMessage = lastMessageTime });
            else
                user.LastMessage = lastMessageTime;

            await _context.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastMessage(ulong uid)
        {
            var user = await _context.Experiences
                .FindAsync(uid);

            if (user == null)
            {
                _context.Add(new Experience { UserId = uid, Level = 1, Exp = 0, LastMessage = null });
                await _context.SaveChangesAsync();
            }

            var lastMessage = await _context.Experiences
                .Where(x => x.UserId == uid)
                .Select(x => x.LastMessage)
                .FirstOrDefaultAsync();

            return await Task.FromResult(lastMessage);
        }
    }
}
