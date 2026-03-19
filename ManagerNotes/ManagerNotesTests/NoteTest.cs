using ManagerNotes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ManagerNotesTests
{
	[TestClass]
	public class NoteTest
	{
		[TestMethod]
		public void Constructor_SetsTitleCorrectly () //Проверка добавления заголовка
		{
			var note = new Note ("Заголовок", "Содержимое");
			Assert.AreEqual ("Заголовок", note.Title);
		}

		[TestMethod]
		public void Constructor_SetsContentCorrectly () //Проверка добавления содержимого
		{
			var note = new Note ("Заголовок", "Содержимое");
			Assert.AreEqual ("Содержимое", note.Content);
		}

		[TestMethod]
		public void Constructor_SetsDateToCurrentTime () //Проверка изменения старой даты на новый 
        {
			var before = DateTime.Now;
			var note = new Note ("Заголовок", "Содержимое");
			var after = DateTime.Now;
			Assert.IsTrue (note.Date>=before&&note.Date<=after);
		}

		[TestMethod]
		public void Title_CanBeGetAndSet () //Проверка изменения старого заголовка на новый 
		{
			var note = new Note ("Старый", "Содержимое");
			note.Title="Новый заголовок";
			Assert.AreEqual ("Новый заголовок", note.Title);
		}

		[TestMethod]
		public void Content_CanBeGetAndSet () //Проверка изменения старого содержания на новый 
		{
			var note = new Note ("Заголовок", "Старое содержимое");
			note.Content="Новое содержимое";
			Assert.AreEqual ("Новое содержимое", note.Content);
		}

		[TestMethod]
		public void Date_CanBeGetAndSet () //Проверка добавления даты
        {
			
			var note = new Note ("Заголовок", "Содержимое");
			var newDate = new DateTime (2024, 1, 15, 10, 30, 0);
			note.Date=newDate;
			Assert.AreEqual (newDate, note.Date);
		}

		[TestMethod]
		public void Constructor_AllowsNullTitle () //Проверка создания записи без заголовка
		{
			var note = new Note (null, "Содержимое");
			Assert.IsNull (note.Title);
		}

		[TestMethod]
		public void Constructor_AllowsEmptyTitle () //Проверка создания записи с пустым заголовком
		{
			var note = new Note (string.Empty, "Содержимое");
			Assert.AreEqual (string.Empty, note.Title);
		}

		[TestMethod]
		public void Constructor_AllowsNullContent () //Проверка создания записи без содержания
		{
			var note = new Note ("Заголовок", null);
			Assert.IsNull (note.Content);
		}

		[TestMethod]
		public void Constructor_AllowsEmptyContent () //Проверка создания записи с пустым содержанием
		{
			var note = new Note ("Заголовок", string.Empty);
			Assert.AreEqual (string.Empty, note.Content);
		}
	}
}
