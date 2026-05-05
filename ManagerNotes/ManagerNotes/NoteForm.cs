using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ManagerNotes
{
    public partial class NoteForm : Form
    {
        private NoteManager noteManager;
        
        // Элементы для заметок
        private Label titleLabel;
        private TextBox titleTextBox;
        private Label contentLabel;
        private TextBox contentTextBox;
        private Button addNoteButton;
        private ListBox notesListBox;
        private Button removeNoteButton;
        
        // Элементы для тегов (упрощённые)
        private Label tagLabel;
        private TextBox tagTextBox;
        private Button addTagButton;
        private Button removeTagButton;
        private Label availableTagsLabel;
        private ComboBox availableTagsComboBox;
        private Button filterByTagButton;
        private Button clearFilterButton;

        public NoteForm()
        {
            this.Text = "Управление заметками";
            this.Width = 600;
            this.Height = 400;

            titleLabel = new Label { Location = new Point(10, 10), Text = "Заголовок:", AutoSize = true };
            titleTextBox = new TextBox { Location = new Point(10, 30), Width = 200 };

            contentLabel = new Label { Location = new Point(10, 55), Text = "Содержание:", AutoSize = true };
            contentTextBox = new TextBox { Location = new Point(10, 75), Width = 200, Height = 60, Multiline = true, ScrollBars = ScrollBars.Both };

            addNoteButton = new Button { Location = new Point(10, 145), Text = "Добавить заметку", Width = 120 };
            addNoteButton.Click += AddNoteButton_Click;

            notesListBox = new ListBox { Location = new Point(220, 10), Width = 350, Height = 200 };
			notesListBox.SelectedIndexChanged +=notesListBox_SelectedIndexChanged;
			removeNoteButton = new Button { Location = new Point(230, 220), Text = "Удалить заметку", Width = 120 };
            removeNoteButton.Click += RemoveNoteButton_Click;

            tagLabel = new Label { Location = new Point(10, 175), Text = "Название тега:", AutoSize = true };
            tagTextBox = new TextBox { Location = new Point(10, 195), Width = 150 };
            
            addTagButton = new Button { Location = new Point(10, 220), Text = "Добавить тег", Width = 100 };
            addTagButton.Click += AddTagButton_Click;
            
            removeTagButton = new Button { Location = new Point(120, 220), Text = "Удалить тег", Width = 100 };
            removeTagButton.Click += RemoveTagButton_Click;
            
            availableTagsLabel = new Label { Location = new Point(10, 245), Text = "Фильтр по тегу:", AutoSize = true };
            availableTagsComboBox = new ComboBox { Location = new Point(10, 263), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            availableTagsComboBox.SelectedIndexChanged += AvailableTagsComboBox_SelectedIndexChanged;
            
            clearFilterButton = new Button { Location = new Point(170, 261), Text = "Сброс", Width = 80 };
            clearFilterButton.Click += ClearFilterButton_Click;
            this.Controls.Add(titleLabel);
            this.Controls.Add(titleTextBox);
            this.Controls.Add(contentLabel);
            this.Controls.Add(contentTextBox);
            this.Controls.Add(addNoteButton);
            this.Controls.Add(notesListBox);
            this.Controls.Add(removeNoteButton);
            
            this.Controls.Add(tagLabel);
            this.Controls.Add(tagTextBox);
            this.Controls.Add(addTagButton);
            this.Controls.Add(removeTagButton);
            this.Controls.Add(availableTagsLabel);
            this.Controls.Add(availableTagsComboBox);
            this.Controls.Add(filterByTagButton);
            this.Controls.Add(clearFilterButton);

            noteManager = new NoteManager();
            UpdateNotesList();
            UpdateAvailableTags();
        }

        private void UpdateNotesList()
        {
            notesListBox.Items.Clear();
            foreach (var note in noteManager.Notes)
            {
                string tagsStr = note.Tags.Count > 0 ? $" [{string.Join(", ", note.Tags)}]" : "";
                string content = note.Content; 
                notesListBox.Items.Add($"{note.Title} | {content} | {note.Date:dd.MM.yyyy HH:mm:ss}{tagsStr}");
            }
        }

        private void notesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (notesListBox.SelectedIndex >= 0)
            {
                var note = noteManager.Notes[notesListBox.SelectedIndex];
                tagTextBox.Text = note.Tags.Count > 0 ? string.Join(", ", note.Tags) : string.Empty;
            }
            else
            {
                tagTextBox.Text = string.Empty;
            }
        }

        private void UpdateAvailableTags()
        {
            var selected = availableTagsComboBox.SelectedItem?.ToString();
            availableTagsComboBox.Items.Clear();
            availableTagsComboBox.Items.Add("(все заметки)");
            foreach (var tag in noteManager.AllTags)
            {
                availableTagsComboBox.Items.Add(tag);
            }
            if (selected != null && availableTagsComboBox.Items.Contains(selected))
                availableTagsComboBox.SelectedItem = selected;
            else
                availableTagsComboBox.SelectedIndex = 0;
        }

        private void AddNoteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(titleTextBox.Text) || string.IsNullOrEmpty(contentTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            
            var newNote = new Note(titleTextBox.Text, contentTextBox.Text, DateTime.Now);
            try
            {
                noteManager.AddNote(newNote);
                titleTextBox.Clear();
                contentTextBox.Clear();
                UpdateNotesList();
                UpdateAvailableTags();
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

            try
            {
                noteManager.RemoveNote(noteManager.Notes[notesListBox.SelectedIndex]);
                notesListBox.ClearSelected();
                UpdateNotesList();
                UpdateAvailableTags();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddTagButton_Click(object sender, EventArgs e)
        {
            if (notesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите заметку для добавления тега!");
                return;
            }
            string input = tagTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Введите теги (можно несколько через запятую)!");
                return;
            }
            try
            {
                var note = noteManager.Notes[notesListBox.SelectedIndex];
                var tagsToAdd = input.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).Distinct(StringComparer.OrdinalIgnoreCase);

                bool anyAdded = false;
                foreach (var tag in tagsToAdd)
                {
                    if (!note.Tags.Any(t => t.Trim().Equals(tag, StringComparison.OrdinalIgnoreCase)))
                    {
                        noteManager.AddTagToNote(note, tag);
                        anyAdded = true;
                    }
                }
                if (anyAdded)
                {
                    tagTextBox.Clear();
                    UpdateNotesList();
                    UpdateAvailableTags();
                }
                else
                {
                    MessageBox.Show("Все указанные теги уже присутствуют у этой заметки.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RemoveTagButton_Click(object sender, EventArgs e)
        {
            if (notesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите заметку!");
                return;
            }
            string input = tagTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Введите название тега (можно несколько через запятую)!");
                return;
            }

            try
            {
                var note = noteManager.Notes[notesListBox.SelectedIndex];
                var tagsToRemove = input.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).Distinct();

                List<string> notFoundTags = new List<string>();
                bool anyRemoved = false;
                foreach (var tag in tagsToRemove)
                {
                    if (note.Tags.Any(t => t.Trim().Equals(tag, StringComparison.OrdinalIgnoreCase)))
                    {
                        noteManager.RemoveTagFromNote(note, tag);
                        anyRemoved = true;
                    }
                    else
                    {
                        notFoundTags.Add(tag);
                    }
                }
                if (anyRemoved)
                {
                    tagTextBox.Clear();
                    UpdateNotesList();
                    UpdateAvailableTags();
                    if (notFoundTags.Count > 0)
                    {
                        MessageBox.Show(
                            $"Теги удалены.\n\nНе найдены и не удалены: {string.Join(", ", notFoundTags)}",
                            "Информация",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"Ни один из указанных тегов не найден у этой заметки!\n" +
                        $"Доступные теги: {string.Join(", ", note.Tags)}",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string FormatNoteForDisplay(Note note)
        {
            string tagsPart = "";
            if (note.Tags.Count > 0)
            {
                tagsPart = " [" + string.Join(", ", note.Tags) + "]";
            }
            string content = note.Content;
            return $"{note.Title} | {content} | {note.Date:dd.MM.yy}{tagsPart}";
        }
        private void FilterByTagButton_Click(object sender, EventArgs e)
        {
            string selectedTag = availableTagsComboBox.SelectedItem?.ToString();
            if (selectedTag == null || selectedTag == "(все заметки)")
            {
                UpdateNotesList();
                return;
            }
            var filteredNotes = noteManager.GetNotesByTag(selectedTag);
            notesListBox.Items.Clear();
            foreach (var note in filteredNotes)
            {
                notesListBox.Items.Add(FormatNoteForDisplay(note));
            }
        }
        private void ClearFilterButton_Click(object sender, EventArgs e)
        {
            availableTagsComboBox.SelectedIndex = 0;
            UpdateNotesList();
        }

        private void AvailableTagsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterByTagButton_Click(sender, e);
        }
    }
}