using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System .Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManagerNotes
{
    public partial class NoteForm : Form
    {
        private NoteManager noteManager;
		private Label titleLabel;
		private TextBox titleTextBox;
		private Label contentLabel;
		private TextBox contentTextBox;
        private Button addNoteButton;
        private ListBox notesListBox;
        private Button removeNoteButton;

        public NoteForm()
        {
            this.Text = "Управление заметками";
            this.Width = 500;
            this.Height = 400;

			titleLabel = new Label
			{
				Location = new Point ( 10 , 10 ) ,
				Text = "Заголовок:" ,
				AutoSize = true ,
				Font = new Font ( "Microsoft Sans Serif" , 9 , FontStyle .Regular )
			};

			titleTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 25),
                Width = 200,

            };

			contentLabel = new Label
			{
				Location = new Point ( 10 , 50 ) ,
				Text = "Содержание:" ,
				AutoSize = true ,
				Font = new Font ( "Microsoft Sans Serif" , 9 , FontStyle .Regular )
			};

			contentTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 70),
                Width = 200,
                Height = 100,
                Multiline = true,
                ScrollBars = ScrollBars.Both,

            };

            addNoteButton = new Button
            {
                Location = new System.Drawing.Point(10, 170),
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

			this .Controls .Add ( titleLabel );        
			this .Controls .Add ( titleTextBox );
			this .Controls .Add ( contentLabel );     
			this .Controls .Add ( contentTextBox );
			this .Controls .Add ( addNoteButton );
			this .Controls .Add ( notesListBox );
			this .Controls .Add ( removeNoteButton );

			noteManager = new NoteManager();
            UpdateNotesList();
        }

		private void UpdateNotesList ( )
		{
			notesListBox .Items .Clear ( );
			foreach ( var note in noteManager .Notes )
			{
				notesListBox .Items .Add ( $"{note .Title}|{note .Content}|{note .Date:yyyy-MM-dd HH:mm:ss}" );
			}
		}

		private void AddNoteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(titleTextBox.Text) || string.IsNullOrEmpty(contentTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            Note newNote = new Note(titleTextBox.Text, contentTextBox.Text , DateTime .Now );
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
            string[] parts = selectedItem.Split(new[] { '|' }, 3);

            if (parts.Length == 3)
            {
                string title = parts[0];
                string content = parts[1];
                string dateStr = parts[2].Trim();

                var noteToRemove = noteManager.Notes.Find(n =>
                    n.Title == title &&
                    n.Content == content &&
                    n.Date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) == dateStr);

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
