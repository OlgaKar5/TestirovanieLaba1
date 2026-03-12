using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManagerNotes
{
    public partial class NoteForm : Form
    {
        private NoteManager noteManager;
        private TextBox titleTextBox;
        private TextBox contentTextBox;
        private Button addNoteButton;
        private ListBox notesListBox;
        private Button removeNoteButton;

        public NoteForm()
        {
            this.Text = "Управление заметками";
            this.Width = 500;
            this.Height = 400;

            titleTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 10),
                Width = 200,

            };

            contentTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 40),
                Width = 200,
                Height = 100,
                Multiline = true,
                ScrollBars = ScrollBars.Both,

            };

            addNoteButton = new Button
            {
                Location = new System.Drawing.Point(10, 150),
                Text = "Добавить",
                Width = 100
            };
            addNoteButton.Click += AddNoteButton_Click;

            notesListBox = new ListBox
            {
                Location = new System.Drawing.Point(220, 10),
                Width = 250,
                Height = 200
            };

            removeNoteButton = new Button
            {
                Location = new System.Drawing.Point(220, 220),
                Text = "Удалить",
                Width = 100
            };
            removeNoteButton.Click += RemoveNoteButton_Click;

            this.Controls.Add(titleTextBox);
            this.Controls.Add(contentTextBox);
            this.Controls.Add(addNoteButton);
            this.Controls.Add(notesListBox);
            this.Controls.Add(removeNoteButton);

            noteManager = new NoteManager();
            UpdateNotesList();
        }

        private void UpdateNotesList()
        {
            notesListBox.Items.Clear();
            foreach (var note in noteManager.Notes)
            {
                notesListBox.Items.Add($"{note.Title} ({note.Date.ToString("yyyy-MM-dd")})");
            }
        }

        private void AddNoteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(titleTextBox.Text) || string.IsNullOrEmpty(contentTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            Note newNote = new Note(titleTextBox.Text, contentTextBox.Text);
            try
            {
                noteManager.AddNote(newNote);
                titleTextBox.Clear();
                contentTextBox.Clear();
                UpdateNotesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveNoteButton_Click(object sender, EventArgs e)
        {
            if (notesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите заметку для удаления!");
                return;
            }
            string selectedItem = notesListBox.SelectedItem.ToString();
            string[] parts = selectedItem.Split(new[] { '(' }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string title = parts[0];
                DateTime date;
                if (DateTime.TryParse(parts[1].Split(')')[0], out date))
                {
                    var noteToRemove = noteManager.Notes.Find(n => n.Title == title && n.Date.Date
    == date.Date);
                    if (noteToRemove != null)
                    {
                        try
                        {
                            noteManager.RemoveNote(noteToRemove);
                            UpdateNotesList();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
