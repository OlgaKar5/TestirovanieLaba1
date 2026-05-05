using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ManagerNotes
{
    public class NoteManager
    {
        public List<Note> Notes { get; private set; }
        public List<string> AllTags { get; private set; }

        public NoteManager()
        {
            Notes = new List<Note>();
            AllTags = new List<string>();
            LoadNotes();
        }

        public void AddNote(Note note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));
            
            Notes.Add(note);
            UpdateAllTags();
            SaveNotes();
        }

        public void RemoveNote(Note note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));
            
            Notes.Remove(note);
            UpdateAllTags();
            SaveNotes();
        }

        public void AddTagToNote(Note note, string tag)
        {
            if (note == null || string.IsNullOrWhiteSpace(tag))
                return;
            
            note.AddTag(tag);
            UpdateAllTags();
            SaveNotes();
        }

        public void RemoveTagFromNote(Note note, string tag)
        {
            if (note == null || string.IsNullOrWhiteSpace(tag))
                return;
            
            note.RemoveTag(tag);
            UpdateAllTags();
            SaveNotes();
        }
        public List<Note> GetNotesByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return Notes;
            
            return Notes.Where(n => n.Tags.Contains(tag)).ToList();
        }
        public void CreateNewTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag) && !AllTags.Contains(tag.Trim()))
            {
                AllTags.Add(tag.Trim());
                SaveNotes();
            }
        }
        public void DeleteTag(string tag)
        {
            if (AllTags.Contains(tag))
            {
                AllTags.Remove(tag);
                foreach (var note in Notes)
                {
                    note.RemoveTag(tag);
                }
                SaveNotes();
            }
        }

        private void UpdateAllTags()
        {
            AllTags = Notes.SelectMany(n => n.Tags).Distinct().OrderBy(t => t).ToList();
        }

        private void SaveNotes()
        {
            var lines = new List<string>();
            lines.Add("TAGS:" + string.Join(",", AllTags));
            
            foreach (var note in Notes)
            {
                string tagsStr = string.Join(";", note.Tags);
                string contentEscaped = note.Content.Replace("|", "\\|").Replace("\n", "\\n");
                // Формат: Заголовок|Содержание|Дата|Теги
                lines.Add($"{note.Title}|{contentEscaped}|{note.Date:yyyy-MM-dd HH:mm:ss}|{tagsStr}");
            }
            
            File.WriteAllLines("notes.txt", lines);
        }

        private void LoadNotes()
        {
            if (!File.Exists("notes.txt"))
                return;
                
            var lines = File.ReadAllLines("notes.txt");
            if (lines.Length == 0)
                return;
            if (lines[0].StartsWith("TAGS:"))
            {
                var tags = lines[0].Substring(5).Split(',');
                AllTags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
            }
            int startIndex = lines[0].StartsWith("TAGS:") ? 1 : 0;
            
            for (int i = startIndex; i < lines.Length; i++)
            {
                var parts = lines[i].Split('|');
                if (parts.Length >= 3)
                {
                    DateTime date;
                    if (DateTime.TryParse(parts[2], out date))
                    {
                        string content = parts[1].Replace("\\|", "|").Replace("\\n", "\n");
                        var note = new Note(parts[0], content, date);
                        if (parts.Length == 4 && !string.IsNullOrWhiteSpace(parts[3]))
                        {
                            var tags = parts[3].Split(';');
                            foreach (var tag in tags)
                            {
                                if (!string.IsNullOrWhiteSpace(tag))
                                    note.AddTag(tag);
                            }
                        }
                        
                        Notes.Add(note);
                    }
                }
            }
        }
    }
}