using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class TagDAO
    {
        private readonly FunewsManagementContext _context;

        public TagDAO(FunewsManagementContext context)
        {
            _context = context;
        }

        public List<Tag> GetAllTags()
        {
            return _context.Tags.Include(t => t.NewsArticles).ToList();
        }

        public Tag? GetTagById(int tagId)
        {
            return _context.Tags.Include(t => t.NewsArticles)
                                .FirstOrDefault(t => t.TagId == tagId);
        }

        public void AddTag(Tag tag)
        {
            _context.Tags.Add(tag);
            _context.SaveChanges();
        }

        public void UpdateTag(Tag tag)
        {
            var existingTag = _context.Tags.Find(tag.TagId);
            if (existingTag != null)
            {
                existingTag.TagName = tag.TagName;
                existingTag.Note = tag.Note;
                _context.SaveChanges();
            }
        }

        public void DeleteTag(int tagId)
        {
            var tag = _context.Tags.Find(tagId);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                _context.SaveChanges();
            }
        }
    }
}
