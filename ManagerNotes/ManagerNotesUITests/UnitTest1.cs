using System;
using System.Linq;
using System.IO;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagerNotes.UITests
{
    [TestClass]
    public class ManagerNotesTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;

        private const string AppRelativePath = @"..\..\..\ManagerNotes\bin\Debug\ManagerNotes.exe";
        private string AppPath;

        [TestInitialize]
        public void TestInitialize()
        {
            AppPath = Path.GetFullPath(AppRelativePath);

            
            try
            {
                var processes = System.Diagnostics.Process.GetProcessesByName("ManagerNotes");
                foreach (var proc in processes)
                {
                    proc.Kill();
                    proc.WaitForExit(2000);
                }
                Thread.Sleep(500);
            }
            catch { }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (_app != null)
            {
                try { _app.Close(); Thread.Sleep(500); _app.Dispose(); } catch { }
            }
            if (_automation != null)
            {
                try { _automation.Dispose(); } catch { }
            }
            
            try
            {
                var processes = System.Diagnostics.Process.GetProcessesByName("ManagerNotes");
                foreach (var proc in processes) { proc.Kill(); proc.WaitForExit(1000); }
            }
            catch { }
            Thread.Sleep(300);
        }

        #region Вспомогательные методы — только UI

        private void LaunchApp()
        {
            _app = Application.Launch(AppPath);
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation);
            Thread.Sleep(1500);
        }

        
        private void AddNoteViaUI(string title, string content)
        {
            var titleBox = FindTitleBox();
            var contentBox = FindContentBox();
            var addBtn = FindAddNoteButton();

            titleBox.Text = title;
            contentBox.Text = content;
            addBtn.Click();
            Thread.Sleep(600);

            
            CloseAnyMessageBox();
        }

        
        private void DeleteAllNotesViaUI()
        {
            var notesList = FindNotesListBox();
            var deleteBtn = FindDeleteNoteButton();

            while (notesList.Items.Length > 0)
            {
                notesList.Items[0].Select();
                Thread.Sleep(200);
                deleteBtn.Click();
                Thread.Sleep(400);
                CloseAnyMessageBox(); 
            }
        }

        
        private void AddTagToSelectedNote(string tagName)
        {
            var tagBox = FindTagBox();
            var addTagBtn = FindAddTagButton();

            tagBox.Text = tagName;
            addTagBtn.Click();
            Thread.Sleep(600);
            CloseAnyMessageBox();
        }

        
        private void RemoveTagFromSelectedNote(string tagName)
        {
            var tagBox = FindTagBox();
            var removeTagBtn = FindRemoveTagButton();

            tagBox.Text = tagName;
            removeTagBtn.Click();
            Thread.Sleep(600);
            CloseAnyMessageBox();
        }

        
        private void CloseAnyMessageBox()
        {
            var msgBox = _mainWindow.ModalWindows.FirstOrDefault();
            if (msgBox != null)
            {
                var okBtn = msgBox.FindFirstDescendant(cf => cf.ByAutomationId("2"));
                if (okBtn == null) okBtn = msgBox.FindFirstDescendant(cf => cf.ByName("OK"));
                if (okBtn == null) okBtn = msgBox.FindFirstDescendant(cf => cf.ByName("ОК"));
                if (okBtn != null) { okBtn.AsButton().Click(); Thread.Sleep(200); }
            }
        }

        
        private TextBox FindTitleBox()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit).And(cf.ByName("Заголовок:")));
            if (el == null) throw new Exception("Поле 'Заголовок:' не найдено");
            return el.AsTextBox();
        }

        private TextBox FindContentBox()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit).And(cf.ByName("Содержание:")));
            if (el == null) throw new Exception("Поле 'Содержание:' не найдено");
            return el.AsTextBox();
        }

        private Button FindAddNoteButton()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Добавить заметку")));
            if (el == null) throw new Exception("Кнопка 'Добавить заметку' не найдена");
            return el.AsButton();
        }

        private Button FindDeleteNoteButton()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Удалить заметку")));
            if (el == null) throw new Exception("Кнопка 'Удалить заметку' не найдена");
            return el.AsButton();
        }

        private ListBox FindNotesListBox()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            if (el == null) throw new Exception("ListBox заметок не найден");
            return el.AsListBox();
        }

        private TextBox FindTagBox()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit).And(cf.ByName("Название тега:")));
            if (el == null) throw new Exception("Поле 'Название тега:' не найдено");
            return el.AsTextBox();
        }

        private Button FindAddTagButton()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Добавить тег")));
            if (el == null) throw new Exception("Кнопка 'Добавить тег' не найдена");
            return el.AsButton();
        }

        private Button FindRemoveTagButton()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Удалить тег")));
            if (el == null) throw new Exception("Кнопка 'Удалить тег' не найдена");
            return el.AsButton();
        }

        private Button FindEditTagButton()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Редактировать тег")));
            if (el == null) throw new Exception("Кнопка 'Редактировать тег' не найдена");
            return el.AsButton();
        }

        private ComboBox FindFilterComboBox()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.ComboBox).And(cf.ByName("Фильтр по тегу:")));
            if (el == null) throw new Exception("ComboBox 'Фильтр по тегу:' не найден");
            return el.AsComboBox();
        }

        private Button FindClearFilterButton()
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Сброс")));
            if (el == null) throw new Exception("Кнопка 'Сброс' не найдена");
            return el.AsButton();
        }

        private bool WaitForAndCloseMessageBox(string expectedText, int timeoutMs = 3000)
        {
            int elapsed = 0;
            while (elapsed < timeoutMs)
            {
                var msgBox = _mainWindow.ModalWindows.FirstOrDefault();
                if (msgBox != null)
                {
                    var textEl = msgBox.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text));
                    if (textEl != null && textEl.Name != null && textEl.Name.Contains(expectedText))
                    {
                        var okBtn = msgBox.FindFirstDescendant(cf => cf.ByAutomationId("2"));
                        if (okBtn == null) okBtn = msgBox.FindFirstDescendant(cf => cf.ByName("OK"));
                        if (okBtn == null) okBtn = msgBox.FindFirstDescendant(cf => cf.ByName("ОК"));
                        if (okBtn != null) { okBtn.AsButton().Click(); Thread.Sleep(200); return true; }
                    }
                }
                Thread.Sleep(100);
                elapsed += 100;
            }
            return false;
        }

        #endregion

        #region Основные тест-кейсы

        [TestMethod]
        [DoNotParallelize]
        public void TC001_AddValidNote_AppearsInList()
        {
            LaunchApp();

            
            AddNoteViaUI("Тестовая заметка", "Проверка добавления");

            var notesList = FindNotesListBox();
            bool found = notesList.Items.Any(i => i.Text.Contains("Тестовая заметка") && i.Text.Contains("Проверка добавления"));
            Assert.IsTrue(found, "Заметка не появилась в списке");

            
            DeleteAllNotesViaUI();
        }

        [TestMethod]
        [DoNotParallelize]
        public void TC002_AddEmptyNote_ShowsValidation()
        {
            LaunchApp();

            var addBtn = FindAddNoteButton();
            addBtn.Click();

            bool closed = WaitForAndCloseMessageBox("Заполните все поля");
            Assert.IsTrue(closed, "Модальное окно не появилось");
        }

        [TestMethod]
        [DoNotParallelize]
        public void TC003_AddNoteWithoutTitle_ShowsValidation()
        {
            LaunchApp();

            var contentBox = FindContentBox();
            var addBtn = FindAddNoteButton();

            contentBox.Text = "Только содержание";
            addBtn.Click();

            bool closed = WaitForAndCloseMessageBox("Заполните все поля");
            Assert.IsTrue(closed, "Сообщение валидации не появилось");
        }

        [TestMethod]
        [DoNotParallelize]
        public void TC004_AddNoteWithoutContent_ShowsValidation()
        {
            LaunchApp();

            var titleBox = FindTitleBox();
            var addBtn = FindAddNoteButton();

            titleBox.Text = "Только заголовок";
            addBtn.Click();

            bool closed = WaitForAndCloseMessageBox("Заполните все поля");
            Assert.IsTrue(closed, "Сообщение валидации не появилось");
        }

        [TestMethod]
        [DoNotParallelize]
        public void TC005_DeleteExistingNote_RemovesFromList()
        {
            LaunchApp();

            
            AddNoteViaUI("Существующая", "Запись");
            var notesList = FindNotesListBox();
            Assert.AreEqual(1, notesList.Items.Length, "Заметка не создана");

            
            var deleteBtn = FindDeleteNoteButton();
            notesList.Items[0].Select();
            Thread.Sleep(400);
            deleteBtn.Click();
            Thread.Sleep(600);
            CloseAnyMessageBox();

            Assert.AreEqual(0, notesList.Items.Length, "Заметка не удалена");
        }

        [TestMethod]
        [DoNotParallelize]
        public void TC006_DeleteWithoutSelection_ShowsWarning()
        {
            LaunchApp();

            var deleteBtn = FindDeleteNoteButton();
            var notesList = FindNotesListBox();

            notesList.Click(); 
            Thread.Sleep(300);

            deleteBtn.Click();
            Thread.Sleep(600);

            bool closed = WaitForAndCloseMessageBox("Выберите заметку для удаления");
            Assert.IsTrue(closed, "Предупреждение не появилось");
        }

        #endregion

        #region Тест-кейсы по тегам

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC001_AddSingleTag_ShowsInList()
        {
            LaunchApp();

            
            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Items[0].Select();
            Thread.Sleep(400);

            
            AddTagToSelectedNote("Работа");

            Assert.IsTrue(notesList.Items[0].Text.Contains("[Работа]"), "Тег не отобразился");

            
            DeleteAllNotesViaUI();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC002_DuplicateTag_ShowsWarning()
        {
            LaunchApp();

            
            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Items[0].Select();
            Thread.Sleep(400);
            AddTagToSelectedNote("Работа");

            
            var tagBox = FindTagBox();
            var addTagBtn = FindAddTagButton();
            tagBox.Text = "Работа";
            addTagBtn.Click();
            Thread.Sleep(600);

            bool closed = WaitForAndCloseMessageBox("");
            Assert.IsTrue(closed, "Нет предупреждения о дубликате");

            DeleteAllNotesViaUI();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC003_RemoveSingleTag_RemovesFromList()
        {
            LaunchApp();

            
            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Items[0].Select();
            Thread.Sleep(400);
            AddTagToSelectedNote("Работа");
            Assert.IsTrue(notesList.Items[0].Text.Contains("[Работа]"), "Тег не добавлен");
            notesList.Items[0].Select();
            
            RemoveTagFromSelectedNote("Работа");

            Assert.IsFalse(notesList.Items[0].Text.Contains("[Работа]"), "Тег не удалён");

            DeleteAllNotesViaUI();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC004_AddMultipleTags_ShowsAll()
        {
            LaunchApp();

            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Items[0].Select();
            Thread.Sleep(400);

            
            AddTagToSelectedNote("Работа");
            notesList = FindNotesListBox();
            notesList.Items[0].Select();
            AddTagToSelectedNote("Идеи");

            Assert.IsTrue(notesList.Items[0].Text.Contains("Работа") && notesList.Items[0].Text.Contains("Идеи"), "Теги не добавлены");

            DeleteAllNotesViaUI();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC005_RemoveMultipleTags_RemovesAll()
        {
            LaunchApp();

            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Items[0].Select();
            Thread.Sleep(400);
            AddTagToSelectedNote("Работа");
            notesList.Items[0].Select();
            AddTagToSelectedNote("Идеи");

            var tagBox = FindTagBox();
            tagBox.Text = "Работа, Идеи";
            notesList.Items[0].Select();
            var removeTagBtn = FindRemoveTagButton();
            removeTagBtn.Click();
            Thread.Sleep(800);
            CloseAnyMessageBox();

            Assert.IsFalse(notesList.Items[0].Text.Contains("[Работа") && notesList.Items[0].Text.Contains("Идеи]"), "Теги не удалены");

            DeleteAllNotesViaUI();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC006_FilterByTag_ShowsOnlyMatching_AndClearFilter()
        {
            LaunchApp();
            DeleteAllNotesViaUI();
            AddNoteViaUI("Заметка1", "Текст1");
            var notesList = FindNotesListBox();
            notesList.Items[0].Click();
            Thread.Sleep(500);
            AddTagToSelectedNote("Работа");
            Thread.Sleep(500);

            AddNoteViaUI("Заметка2", "Текст2");
            notesList.Items[1].Click();
            Thread.Sleep(500);
            AddTagToSelectedNote("Личное");
            Thread.Sleep(500);

            Assert.AreEqual(2, notesList.Items.Length, "Должно быть 2 записи");

            var filterCombo = FindFilterComboBox();
            Thread.Sleep(1500); 
            var workItem = filterCombo.Items.FirstOrDefault(i => i.Name == "Работа");

            if (workItem == null)
                workItem = filterCombo.Items.FirstOrDefault(i => i.Name.Contains("Работа"));

            if (workItem != null)
            {
                workItem.Click();
            }
            else
            {
                Console.WriteLine("Элемент 'Работа' не найден, пробуем SetValue");
                filterCombo.Patterns.Value.Pattern.SetValue("Работа");
            }

            Thread.Sleep(2000);

            Assert.AreEqual(1, notesList.Items.Length, "Фильтр не сработал - должно остаться 1 записей");
            Assert.IsTrue(notesList.Items[0].Text.Contains("Заметка1"), "Отфильтрована не та заметка");

            FindClearFilterButton().Click();
            Thread.Sleep(500);
            DeleteAllNotesViaUI();
        }


        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC008_AddTagWithoutSelection_ShowsWarning()
        {
            LaunchApp();

            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Click(); 
            Thread.Sleep(300);

            var tagBox = FindTagBox();
            var addTagBtn = FindAddTagButton();
            tagBox.Text = "Идея";
            addTagBtn.Click();
            Thread.Sleep(600);

            bool closed = WaitForAndCloseMessageBox("Выберите заметку");
            Assert.IsTrue(closed, "Нет предупреждения");
        }

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC009_AddEmptyTag_ShowsWarning()
        {
            LaunchApp();

            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Items[0].Select(); Thread.Sleep(400);

            var tagBox = FindTagBox();
            var addTagBtn = FindAddTagButton();
            tagBox.Text = "";
            addTagBtn.Click();
            Thread.Sleep(600);

            bool closed = WaitForAndCloseMessageBox("Введите теги");
            Assert.IsTrue(closed, "Нет предупреждения");

            DeleteAllNotesViaUI();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC010_RemoveNonExistentTag_ShowsWarning()
        {
            LaunchApp();

            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Items[0].Select(); Thread.Sleep(400);
            AddTagToSelectedNote("Работа");

            var tagBox = FindTagBox();
            var removeTagBtn = FindRemoveTagButton();
            tagBox.Text = "Личное";
            removeTagBtn.Click();
            Thread.Sleep(600);

            bool closed = WaitForAndCloseMessageBox("");
            Assert.IsTrue(closed, "Нет предупреждения");

            DeleteAllNotesViaUI();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Tag_TC011_EditTag_RenamesSuccessfully()
        {
            LaunchApp();

            AddNoteViaUI("Заметка", "Текст");
            var notesList = FindNotesListBox();
            notesList.Items[0].Select(); Thread.Sleep(400);
            AddTagToSelectedNote("Работа");

            
            var tagBox = FindTagBox();
            notesList.Items[0].Select();
            var editTagBtn = FindEditTagButton();
            tagBox.Text = "Работа : Работа2026";
            editTagBtn.Click();
            Thread.Sleep(800);
            WaitForAndCloseMessageBox("");

            Assert.IsTrue(notesList.Items[0].Text.Contains("[Работа2026]"), "Тег не переименован");
            Assert.IsFalse(notesList.Items[0].Text.Contains("[Работа]"), "Старый тег остался");

            DeleteAllNotesViaUI();
        }

        #endregion
    }
}