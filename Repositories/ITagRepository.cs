using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetAllTags();
        Tag? GetTagById(int tagId);
        void AddTag(Tag tag);
        void UpdateTag(Tag tag);
        void DeleteTag(int tagId);
    }
}
