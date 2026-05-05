using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ManagerNotes;

namespace ManagerNotesTests
{
    [TestClass]
    public class TagsTest
    {
        [TestMethod]
        public void Note_AddTag_ShouldAddTagToList()
        {
            var note = new Note("Тест", "Содержание", System.DateTime.Now);
            note.AddTag("Работа");
            Assert.IsTrue(note.Tags.Contains("Работа"));
            Assert.AreEqual(1, note.Tags.Count);
        }

        [TestMethod]
        public void Note_AddTag_ShouldNotAddDuplicate()
        {
            var note = new Note("Тест", "Содержание", System.DateTime.Now);
            note.AddTag("Работа");
            note.AddTag("Работа");
            Assert.AreEqual(1, note.Tags.Count);
        }

        [TestMethod]
        public void Note_RemoveTag_ShouldRemoveTag()
        {
            var note = new Note("Тест", "Содержание", System.DateTime.Now);
            note.AddTag("Работа");
            note.RemoveTag("Работа");
            Assert.IsFalse(note.Tags.Contains("Работа"));
        }

        [TestMethod]
        public void NoteManager_GetNotesByTag_ShouldReturnFilteredNotes()
        {
            var manager = new NoteManager();
            var note1 = new Note("Заметка 1", "Текст 1", System.DateTime.Now);
            var note2 = new Note("Заметка 2", "Текст 2", System.DateTime.Now);
            note1.AddTag("Работа");
            note2.AddTag("Личное");
            manager.AddNote(note1);
            manager.AddNote(note2);
            var result = manager.GetNotesByTag("Работа");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Заметка 1", result[0].Title);
        }

        [TestMethod]
        public void NoteManager_DeleteTag_ShouldRemoveFromAllNotes()
        {
            var manager = new NoteManager();
            var note = new Note("Тест", "Текст", System.DateTime.Now);
            note.AddTag("Удалить");
            manager.AddNote(note);
            manager.DeleteTag("Удалить");
            Assert.IsFalse(manager.AllTags.Contains("Удалить"));
            Assert.IsFalse(note.Tags.Contains("Удалить"));
        }
    }
}