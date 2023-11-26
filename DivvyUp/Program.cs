using DivvyUp;

DBUtility dbUtility = new DBUtility();
dbUtility.CreateDB();
dbUtility.InitializeDatabase();

List<Person>? people = dbUtility.GetPeopleFromDatabase();
ConsoleUI ui = new ConsoleUI(people, dbUtility);

ui.RunUI();
