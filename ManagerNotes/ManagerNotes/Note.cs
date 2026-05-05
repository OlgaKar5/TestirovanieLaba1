using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerNotes
{
    public class Note
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public List<string> Tags { get; set; }
        public Note(string title, string content, DateTime? date = null)
        {
            Title = title;
            Content = content;
            Date = date ?? DateTime.Now;
            Tags = new List<string>();
        }
        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return;
            string trimmedTag = tag.Trim();
            if (!Tags.Any(t => t.Trim().Equals(trimmedTag, StringComparison.OrdinalIgnoreCase)))
            {
                Tags.Add(trimmedTag);
            }
        }
        public void RemoveTag(string tag)
        {
            Tags.Remove(tag);
        }
    }
}