using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace ContactBook
{
    class Program
    {
        const string filePath = "contacts.json";
        static void Main(string[] args)
        {
            List<Contact> contacts = new List<Contact>();
            loadContacts(ref contacts);
            string[] options = { "Add contact", "View contacts", "Edit contacts", "Open JSON", "Exit" };
            while (true)
            {
                int selected = menu("Welcome to Contacts!", options);

                switch (selected)
                {
                    case 0:
                        contacts.Add(addContact());
                        sortContacts(contacts);
                        saveContacts(contacts);
                        break;
                    case 1:
                        viewContacts(contacts);
                        break;
                    case 2:
                        editContact(contacts);
                        sortContacts(contacts);
                        saveContacts(contacts);
                        break;
                    case 3:
                        printJson();
                        break;
                    case 4:
                        Console.WriteLine("Exiting...");
                        Environment.Exit(0);
                        break;
                }
            }
        }
        static int menu(string header, string[] options)
        {                
            int selected = 0;
            ConsoleKey key;

            //loop until one is selected
            do
            {
                Console.Clear();
                Console.WriteLine($"{header}\nUse arrow keys to navigate and Enter to select:\n");

                // Draw menu and highlights the selected option
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                    {                        
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                        Console.WriteLine($"  {options[i]}");
                }

                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                    selected = (selected == 0) ? options.Length - 1 : selected - 1;    //if user goes up from first option, selects last option            
                else if (key == ConsoleKey.DownArrow)
                    selected = (selected == options.Length - 1) ? 0 : selected + 1;    //if user goes down from last option, selects first option 
            } while (key != ConsoleKey.Enter);

            Console.Clear();
            return selected;
        }

        static Contact addContact()
        {
            //second paramenter true if field is mandatory, third the error message, forth parameter indicates additional controls over input 
            string? name = enterField("name", true, "Contact name is mandatory", input => string.IsNullOrEmpty(input));               
            string? surname = enterField("surname", false, null, input => false);                              
            string? number = enterField("phone number", true, "Invalid phone number. Only digits are allowed (optionally +, spaces, or dashes).", input => !Regex.IsMatch(input.Trim(), @"^\+?[0-9\s\-]{7,20}$"));
            string? email = enterField("email", false, "Invalid email format.", input => !Regex.IsMatch(input.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"));
            string? birthday = enterField("birthday (dd/mm/yyyy)", false, "Invalid date format.", input => !DateTime.TryParse(input, out _));
            string? notes = enterField("additional notes", false, null, input => false);

            if (DateTime.TryParse(birthday, out DateTime data))
                birthday = data.ToShortDateString();
            return new Contact(name, surname, number, email, birthday, notes);
        }

        static void viewContacts(List<Contact> contacts)
        {
            Console.Clear();
            Console.WriteLine("Contacts:\n\n=====\n");
            foreach (Contact c in contacts)
                Console.WriteLine("Name: " + c.Name + "\nSurname: " + c.Surname + "\nNumber: " + c.Number + "\nEmail: " + c.Email +
                    "\nBirthday: " + c.Birthday + "\nNotes: " + c.Notes + "\n\n=====\n");
            Console.WriteLine("Press ESC to return to main menu");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) { }
        }

        static void editContact(List<Contact> contacts)
        {
            if (contacts.Count == 0)
            {
                Console.WriteLine("No contact to edit. Press ESC to return.");
                while (Console.ReadKey(true).Key != ConsoleKey.Escape) { }
            }
            else
            {                
                int selectedC = 0;      //selected contact
                string[] options = contacts.Select(c => $"{c.Name}{(string.IsNullOrWhiteSpace(c.Surname) ? "" : " " + c.Surname)} {c.Number}").Append("Exit").ToArray();
                while (selectedC != options.Length - 1)
                {
                    selectedC = menu("Choose one contact to edit", options);
                    if (selectedC != options.Length - 1)
                    {
                        int selectedF = 0;                      //selected field
                        string[] options2 =
                        [
                            "Name: " + contacts[selectedC].Name,
                            "Surname: " + contacts[selectedC].Surname,
                            "Phone number: " + contacts[selectedC].Number,
                            "Email: " + contacts[selectedC].Email,                            
                            "Birthday: " + contacts[selectedC].Birthday,
                            "Notes: " + contacts[selectedC].Notes,
                            "Exit"
                        ];
                        while (selectedF != options2.Length - 1)
                        {
                            if (selectedF != options2.Length - 1)
                            {
                                selectedF = menu("Choose one field to edit", options2);
                                switch (selectedF)
                                {
                                    case 0:
                                        string? name = enterField("name", true, "Contact name is mandatory", input => string.IsNullOrEmpty(input));
                                        contacts[selectedC].Name = name;
                                        updateMenus(contacts[selectedC], selectedC, ref options, ref options2);
                                        break;
                                    case 1:
                                        string? surname = enterField("surname", false, null, input => false);
                                        contacts[selectedC].Surname = surname;
                                        updateMenus(contacts[selectedC], selectedC, ref options, ref options2);
                                        break;
                                    case 2:
                                        string? number = enterField("phone number", true, "Invalid phone number. Only digits are allowed (optionally +, spaces, or dashes).", input => !Regex.IsMatch(input.Trim(), @"^\+?[0-9\s\-]{7,20}$"));
                                        contacts[selectedC].Number = number;
                                        updateMenus(contacts[selectedC], selectedC, ref options, ref options2);
                                        break;
                                    case 3:
                                        string? email = enterField("email", false, "Invalid email format.", input => !Regex.IsMatch(input.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"));
                                        contacts[selectedC].Email = email;
                                        updateMenus(contacts[selectedC], selectedC, ref options, ref options2);
                                        break;
                                    case 4:
                                        string? birthday = enterField("birthday (dd/mm/yyyy)", false, "Invalid date format.", input => !DateTime.TryParse(input, out _));
                                        contacts[selectedC].Birthday = birthday;
                                        updateMenus(contacts[selectedC], selectedC, ref options, ref options2);
                                        break;
                                    case 5:
                                        string? notes = enterField("additional notes", false, null, input => false);
                                        contacts[selectedC].Notes = notes;
                                        updateMenus(contacts[selectedC], selectedC, ref options, ref options2);
                                        break;
                                    case 6:
                                        break;
                                }
                            }
                        }
                    }
                }
            }                             
        }

        public static void saveContacts(List<Contact> contacts)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) 
            };

            string jsonString = JsonSerializer.Serialize(contacts, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static void printJson()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                Console.WriteLine(json);
            }
            else
                Console.WriteLine($"File not found: {filePath}");
            Console.WriteLine("\n\nPress ESC to return to main menu");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) { }
        }

        static void loadContacts(ref List<Contact> contacts)
        {    
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var loadedContacts = JsonSerializer.Deserialize<List<Contact>>(jsonString);

                if (loadedContacts != null)
                    contacts.AddRange(loadedContacts);
            } catch (Exception ex) {}
        }

        static void updateMenus(Contact c, int selectedC, ref string[] options, ref string[] options2)
        {
            options[selectedC] = $"{c.Name}{(string.IsNullOrWhiteSpace(c.Surname) ? "" : " " + c.Surname)} {c.Number}";
            options2 =
            [
                "Name: " + c.Name,
                "Surname: " + c.Surname,
                "Phone number: " + c.Number,
                "Email: " + c.Email,
                "Birthday: " + c.Birthday,
                "Notes: " + c.Notes,
                "Exit"
            ];
        }

        static String? enterField(String field, bool mandatory, String? error, Func<string?, bool> isInvalid)
        {
            String? input;
            int n = 0;
            do
            {
                Console.Clear();
                n++;
                if (n > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(error + "\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.Write($"Enter {field}{(mandatory ? "*" : "")}: ");
                input = Console.ReadLine();
            } while (isInvalid(input) && (mandatory || !string.IsNullOrEmpty(input)));

            return input;
        }

        static void sortContacts(List<Contact> contacts)
        {
            contacts.Sort((a, b) =>
            {
                int nameComparison = string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
                if (nameComparison != 0)
                    return nameComparison;

                return string.Compare(a.Surname, b.Surname, StringComparison.OrdinalIgnoreCase);
            });
        }
    }

    public class Contact
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Number { get; set; }
        public string? Email { get; set; } = null;        
        public string? Birthday { get; set; }
        public string? Notes { get; set; }

        public Contact(string? name, string? surname, string? number, string? email, string? birthday, string? notes)
        {
            Name = name;
            Surname = surname;
            Number = number;
            Email = email;
            Birthday = birthday;
            Notes = notes;            
        }
    }
}