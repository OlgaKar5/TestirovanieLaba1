using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace ManagerNotes
{
    public class Note
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }

        public Note(string title, string content)
        {
            Title = title;
            Content = content;
            Date = DateTime.Now;
        }
    }
}
