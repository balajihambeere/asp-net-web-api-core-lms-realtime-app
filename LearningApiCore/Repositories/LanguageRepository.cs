namespace LearningApiCore.Repositories
{
    using LearningApiCore.DataAccess;
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class LanguageRepository : ILanguageRepository
    {
        private readonly AppDataContext _context;
        public LanguageRepository(AppDataContext context)
        {
            _context = context;
        }

        public async Task<Language> Add(Language language)
        {
            language.CreatedOn = DateTime.Now;
            language.RowId = Guid.NewGuid();
            _context.Language.Add(language);
            await _context.SaveChangesAsync();
            return language;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Language.AnyAsync(e => e.LanguageId == id);
        }

        public async Task<bool> Exists(string name)
        {
            return await _context.Language.AnyAsync(e => e.Name.Equals(name.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Language> Find(int id)
        {
            return await _context.Language.FindAsync(id);
        }

        public IEnumerable<Language> GetAll()
        {
            var sourceCollection = _context.Language.Select(x => new Language
            {
                LanguageId = x.LanguageId,
                Name = x.Name,
                IsActive = x.IsActive
            });
            return sourceCollection;
        }

        public IEnumerable<KeyValue> GetkeyValue()
        {
            var sourceCollection = _context.Language.Select(x => new KeyValue
            {
                Key = x.LanguageId,
                Value = x.Name
            });
            return sourceCollection;
        }

        public async Task<Language> Update(Language language)
        {
            language.ModifiedOn = DateTime.Now;
            _context.Language.Update(language);
            await _context.SaveChangesAsync();
            return language;
        }
    }
}
