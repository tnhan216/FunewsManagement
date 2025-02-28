using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly TagDAO _dao;

        public TagRepository(FunewsManagementContext context)
        {
            _dao = new TagDAO(context);
        }

        public List<Tag> GetAllTags() => _dao.GetAllTags();

        public Tag? GetTagById(int tagId) => _dao.GetTagById(tagId);

        public void AddTag(Tag tag) => _dao.AddTag(tag);

        public void UpdateTag(Tag tag) => _dao.UpdateTag(tag);

        public void DeleteTag(int tagId) => _dao.DeleteTag(tagId);
    }
}
