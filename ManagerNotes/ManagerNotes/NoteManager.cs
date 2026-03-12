using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ManagerNotes
{
    public class NoteManager
    {
        public List<Note> Notes { get; private set; }

        public NoteManager()
        {
            Notes = new List<Note>();
            LoadNotes();
        }

        public void AddNote(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }
            Notes.Add(note);
            SaveNotes();
        }

        public void RemoveNote(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }
            Notes.Remove(note);
            SaveNotes();
        }

        private void SaveNotes()
        {
            File.WriteAllLines("notes.txt", Notes.Select(n =>
    $"{n.Title}|{n.Content}|{n.Date.ToString("yyyy-MM-dd HH:mm:ss")}"));
        }

        private void LoadNotes()
        {
            if (File.Exists("notes.txt"))
            {
                var lines = File.ReadAllLines("notes.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        DateTime date;
                        if (DateTime.TryParse(parts[2], out date))
                        {
                            Notes.Add(new Note(parts[0], parts[1], date));
                        }
                    }
                }
            }
        }
    }
}
