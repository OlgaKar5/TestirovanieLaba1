using ManagerNotes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace ManagerNoteTests
{
	[TestClass]
	public class NoteManagerTest
	{
		private const string TestFileName = "notes_test.txt";
		private NoteManager _manager;

		[TestInitialize] //Подготовка перед тестом
		public void SetUp () //удаление старого теста и создания нового
		{
			if (File.Exists (TestFileName))
				File.Delete (TestFileName);
			_manager=new NoteManager ();
		}

		[TestCleanup] //Очистка после теста
		public void TearDown () //Удаление тестового файла и файла записи
		{
			if (File.Exists (TestFileName))
				File.Delete (TestFileName);
			if (File.Exists ("notes.txt"))
				File.Delete ("notes.txt");
		}

		[TestMethod]
		public void Constructor_InitializesNotesList () //Проверка создания и инициализации списка записей
		{
			var manager = new NoteManager ();
			Assert.IsNotNull (manager.Notes);
			Assert.IsInstanceOfType (manager.Notes, typeof (System.Collections.Generic.List<Note>));
		}

		[TestMethod]
		public void AddNote_AddsNoteToList () //Проверка метода AddNote
		{
			var note = new Note ("Тест", "Содержимое теста");
			_manager.AddNote (note);
			Assert.AreEqual (1, _manager.Notes.Count);
			Assert.AreSame (note, _manager.Notes [ 0 ]);
		}

		[TestMethod]
		public void AddNote_ThrowsArgumentNullException_WhenNoteIsNull () //Проверка исключения при добавлении null
		{
			Assert.ThrowsException<ArgumentNullException> (() => _manager.AddNote (null));
		}

		[TestMethod]
		public void AddNote_CallsSaveNotes_CreatesFile () //Проверка создания файла
		{
			var note = new Note ("Тест", "Содержимое");
			_manager.AddNote (note);
			Assert.IsTrue (File.Exists ("notes.txt"));
		}

		[TestMethod]
		public void RemoveNote_RemovesNoteFromList () //Проверка метода RemoveNote
		{
			var note = new Note ("Тест", "Содержимое");
			_manager.Notes.Add (note);
			_manager.RemoveNote (note);
			Assert.AreEqual (0, _manager.Notes.Count);
		}

		[TestMethod]
		public void RemoveNote_ThrowsArgumentNullException_WhenNoteIsNull () //Проверка исключения при передаче null в методе RemoveNote
		{
			Assert.ThrowsException<ArgumentNullException> (() => _manager.RemoveNote (null));
		}

		[TestMethod]
		public void RemoveNote_CallsSaveNotes_AfterRemoval () //Проверка что файл существует после удаления записи, но запись отсутствует
		{
			
			var note = new Note ("Тест", "Содержимое");
			_manager.AddNote (note); 
			_manager.RemoveNote (note);
			Assert.IsTrue (File.Exists ("notes.txt"));
			var content = File.ReadAllText ("notes.txt");
			Assert.IsFalse (content.Contains ("Тест|Содержимое"));
		}

		[TestMethod]
		public void LoadNotes_LoadsNotesFromFile_WhenFileExists () //Проверка загрузки уже созданных записей при загрузке файла
		{
			
			File.WriteAllText ("notes.txt", "Заголовок1|Содержимое1|2024-01-15 10:30:00");
			var manager = new NoteManager ();
			Assert.AreEqual (1, manager.Notes.Count);
			Assert.AreEqual ("Заголовок1", manager.Notes [ 0 ].Title);
			Assert.AreEqual ("Содержимое1", manager.Notes [ 0 ].Content);
		}

		[TestMethod]
		public void LoadNotes_HandlesMissingFile_Gracefully () //Если файлов нет, то список создается пустым
		{
			
			if (File.Exists ("notes.txt"))
				File.Delete ("notes.txt");
			var manager = new NoteManager ();
			Assert.IsNotNull (manager.Notes);
			Assert.AreEqual (0, manager.Notes.Count);
		}

		[TestMethod]
		public void LoadNotes_IgnoresMalformedLines () //Проверка пропуска строк с неверным форматом
		{
			
			File.WriteAllText ("notes.txt",
				"Заголовок1|Содержимое1|2024-01-15 10:30:00\n"+
				"НевернаяСтрокаБезРазделителей\n"+
				"Заголовок2|Содержимое2|НевернаяДата");
			var manager = new NoteManager ();
			Assert.AreEqual (1, manager.Notes.Count);
			Assert.AreEqual ("Заголовок1", manager.Notes [ 0 ].Title);
		}
	}
}
