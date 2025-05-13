using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ContactsBook
{
    public class ContactsBook
    {
        List<Contact> allContacts;
        int entriesPerPage;
        bool changesSaved;

        public static void Main(string[] args)
        {
            ContactsBook mj = new ContactsBook();
        }

        //*************************************************************************

        public ContactsBook()
        {
            allContacts = new List<Contact>();
            entriesPerPage = 10;
            changesSaved = true;

            ShowSplashScreen();
            ShowMainMenu();

        }

        //*************************************************************************

        public void ShowSplashScreen()
        {
            string screen = "Welcome to Contacts Book!\r\n\n" +
                "Author: Elena E. Andino Perez\r\n" +
                "Version: 1.0 Final\r\n" +
                "Last Revised: 2025-05-29 05:09 PM\r\n\n" +
                "Description:\r\n" +
                "This program allows you to keep track of your contacts.\r\n";

            Console.WriteLine(screen);
            PressEnterToContinue();

        }

        //*************************************************************************

        public void PressEnterToContinue()
        {
            Console.Write("Press ENTER to continue.");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            Console.Clear();
        }

        //*************************************************************************

        public void ShowMainMenu()
        {
            string menu = $@"
[1] Load contacts from a file.
[2] Show all contacts.
[3] Add contact.
[4] Edit contact.
[5] Delete contact.
[6] Merge duplicate contacts.
[7] Save contacts to a file.
[8] Exit.";

            int option = 8;
            do
            {
                Console.Clear();
                Console.WriteLine(menu);
                option = GetOption("Select an option: ", 1, 8);
                Console.Clear();

                if (option == 1)
                {
                    LoadContactsFromFile();
                }
                else if (option == 2)
                {
                    ShowContacts();
                }
                else if (option == 3)
                {
                    AddContact();
                }
                else if (option == 4)
                {
                    EditContact();
                }
                else if (option == 5)
                {
                    DeleteContact();
                }
                else if (option == 6)
                {
                    MergeDuplicateContacts();
                }
                else if (option == 7)
                {
                    SaveContactsToFile();
                }
                else
                {
                    if (!Exit()) { option = -1; }
                }

                PressEnterToContinue();
            }
            while (option != 8);
        }

        //*************************************************************************

        public void LoadContactsFromFile()
        {
            Console.WriteLine("**Load contacts from a file** \n");

            Console.WriteLine("Loading from: ");
            Console.WriteLine("Write the filename or nothing to cancel.");
            Console.Write("Filename: ");
            string filename = Console.ReadLine();

            LoadContactsFromFile(filename);
        }

        public void LoadContactsFromFile(string filename)
        {
            if (filename == null || filename.Length == 0)
            {
                Console.WriteLine("The operation was canceled.");
            }
            else if (!File.Exists(filename))
            {
                Console.WriteLine($"ERROR: file \"{filename}\" was not found!");
            }
            else
            {
                StreamReader sr = null;

                try
                {
                    sr = new StreamReader(filename);

                    while (!sr.EndOfStream)
                    {
                        Contact e = new Contact();

                        e.firstName = sr.ReadLine();
                        e.lastName = sr.ReadLine();
                        e.phoneNum = sr.ReadLine();
                        e.email = sr.ReadLine();

                        allContacts.Add(e);
                    }

                    Console.WriteLine("Contacts book loaded successfully!");

                }
                catch (Exception)
                {
                    Console.WriteLine("ERROR: An error ocurred while reading the file!");
                }
                finally
                {
                    if (sr != null) { sr.Dispose(); }
                }
            }

        }

        //*************************************************************************

        public void ShowContacts()
        {

            int page = 1;

            do
            {
                Console.Clear();
                int pageCount = Math.Max(1, (int)Math.Ceiling(allContacts.Count / (double)entriesPerPage));
                int offset = (page - 1) * entriesPerPage;
                int limit = Math.Min(offset + entriesPerPage, allContacts.Count);

                Console.WriteLine("###   Name         Last name    Phone Number   E-mail");

                for (int i = offset; i < limit; i++)
                {
                    Contact e = allContacts[i];
                    Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {e.firstName,-12} {e.lastName,-12} {e.phoneNum,-14} {e.email,-20}");
                }
                for (int i = 0; i < entriesPerPage - (limit - offset); i++)
                {
                    Console.WriteLine();
                }

                Console.WriteLine($"Page {page} of {pageCount}");
                string option = GetOption($"Sort by [1] Name [2] Last Name [3] Phone Number [4] E-mail\r\n" +
                    $"Go to [0] Main Menu [-] Previous Page [+] Next Page: ", "0", "1", "2", "3", "4", "-", "+");
                if (option == "1")
                {
                    allContacts.Sort(new FirstNameComparer());
                }
                else if (option == "2")
                {
                    allContacts.Sort(new LastNameComparer());
                }
                else if (option == "3")
                {
                    allContacts.Sort(new PhoneNumComparer());
                }
                else if (option == "4")
                {
                    allContacts.Sort(new EmailComparer());
                }
                else if (option == "0")
                {
                    page = 0;
                }
                else if (option == "-")
                {
                    if (page > 1)
                    {
                        page -= 1;
                    }
                    
                }
                else if (option == "+")
                {
                    if (page < pageCount)
                    {
                        page += 1;
                    }
                    
                }

            } while (page != 0);
        }

        //*************************************************************************

        public void MergeDuplicateContacts()
        {
            Console.WriteLine("**Merge duplicate contacts** \n");

            
            var allContactsCopy = new List<Contact>(allContacts);
            var allDuplicateSets = new List<List<Contact>>();

            int i = 0;

            while (allContactsCopy.Count > 0)
            {
                var duplicateSet = new List<Contact>();
                var originalContact = allContactsCopy[allContactsCopy.Count - 1];
                duplicateSet.Add(originalContact);
                allContactsCopy.RemoveAt(allContactsCopy.Count - 1);

                for (i = allContactsCopy.Count - 1; i >= 0; i--)
                {
                    if (IsDuplicate(originalContact, allContactsCopy[i]))
                    {
                        Contact duplicateContact = allContactsCopy[i];
                        duplicateSet.Add(duplicateContact);
                        allContactsCopy.RemoveAt(i);
                        // Optional: Search also for this duplicate contact's duplicates.
                        for (int j = allContactsCopy.Count - 1; j >= 0; j--)
                        {
                            if (IsDuplicate(duplicateContact, allContactsCopy[j]))
                            {
                                duplicateSet.Add(allContactsCopy[j]);
                                allContactsCopy.RemoveAt(j);
                                i--;
                            }
                        }
                    }
                    
                }
                if (duplicateSet.Count > 1) { allDuplicateSets.Add(duplicateSet); }
            }

            i = 0;
            foreach (List<Contact> e in allDuplicateSets)
            {
                
                Console.WriteLine($"Contact set [{i.ToString().PadLeft(3, '0')}]");
                Console.WriteLine("###   Name         Last name    Phone Number   E-mail");

                int j = 0;
                foreach (Contact contact in e)
                {

                    Contact t = e[j];
                    Console.WriteLine($"[{j.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");
                    
                    j++;
                }

                Console.WriteLine();
                i++;
            }

            int option = GetOption($"Choose which set of contacts to merge [{0}-{allDuplicateSets.Count-1}] or -1 to cancel: ",
                -1, allDuplicateSets.Count);

            if (option == -1)
            {
                Console.WriteLine("Operation cancelled.");
            }
            else
            {
                Console.Clear();
                i = 0;

                Console.WriteLine("###   Name         Last name    Phone Number   E-mail");

                foreach (Contact c in allDuplicateSets[option])
                {

                    Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {c.firstName,-12} {c.lastName,-12} {c.phoneNum,-14} {c.email,-20}");
                    i++;

                }

                int rFirName = GetOption($"Which of the duplicates contains the right first name [{0}-{allDuplicateSets[option].Count - 1}]? ",
                    0, allDuplicateSets[option].Count);
                string firstName = allDuplicateSets[option].ElementAt(rFirName).firstName;

                int rLasName = GetOption($"Which of the duplicates contains the right last name [{0}-{allDuplicateSets[option].Count - 1}]? ",
                    0, allDuplicateSets[option].Count);
                string lastName = allDuplicateSets[option].ElementAt(rLasName).lastName;

                int rPNum = GetOption($"Which of the duplicates contains the right phone number [{0}-{allDuplicateSets[option].Count - 1}]? ",
                    0, allDuplicateSets[option].Count);
                string phoneNum = allDuplicateSets[option].ElementAt(rPNum).phoneNum;

                int rEmail = GetOption($"Which of the duplicates contains the right email [{0}-{allDuplicateSets[option].Count - 1}]? ",
                    0, allDuplicateSets[option].Count);
                string email = allDuplicateSets[option].ElementAt(rEmail).email;

                Console.WriteLine($"\n[Merged contact]\nFirst name: {firstName}\nLast name: {lastName}\n" +
                    $"Phone number: {phoneNum}\nE-mail: {email}\n");

                string answer = GetOption("Do you want to add this contact? [Y/N] ", "Y", "N");

                if (answer == "Y")
                {
                    Contact cont = new Contact();

                    cont.firstName = firstName;
                    cont.lastName = lastName;
                    cont.phoneNum = phoneNum;
                    cont.email = email;

                    allContacts.Add(cont);

                    Console.WriteLine("Merged contact created successfully!\n");

                    i = 0;

                    while (i < allDuplicateSets[option].Count)
                    {

                        GetOption($"Do you want to delete duplicate contact [{i.ToString().PadLeft(3, '0')}]? [Y/N] ", "Y", "N");
                        if (answer == "Y")
                        {
                            allContacts.Remove(allDuplicateSets[option].ElementAt(i));

                            Console.WriteLine("Duplicate deleted successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Contact was not deleted.");
                        }

                        i++;

                    }

                    changesSaved = false;
                }
                else
                {
                    Console.WriteLine("Operation was cancelled.");
                }

            }

            Console.WriteLine();
        }
        public bool IsDuplicate(Contact a, Contact b)
        {

             if(b.firstName == null || b.firstName.Length == 0 || a.firstName == null || a.firstName.Length == 0)
             {
                 if (b.lastName.Equals(a.lastName) ||
                 b.phoneNum.Equals(a.phoneNum) || b.email.Equals(a.email))
                 {
                     return true;
                 }
                else
                 {
                     return false;
                 }
             }
             else if (b.lastName == null || b.lastName.Length == 0 || a.lastName == null || a.lastName.Length == 0)
             {
                 if (b.firstName.Equals(a.firstName) ||
                 b.phoneNum.Equals(a.phoneNum) || b.email.Equals(a.email))
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             }
             else if (b.phoneNum == null || b.phoneNum.Length == 0 || a.phoneNum == null || a.phoneNum.Length == 0)
             {
                 if (b.firstName.Equals(a.firstName) || 
                     b.lastName.Equals(a.lastName) || b.email.Equals(a.email))
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             }
             else if (b.email == null || b.email.Length == 0 || a.email == null || a.email.Length == 0)
             {
                 if (b.firstName.Equals(a.firstName) || b.lastName.Equals(a.lastName) ||
                 b.phoneNum.Equals(a.phoneNum) || b.email.Equals(a.email))
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             }
             else
             {
                 if (b.firstName.Equals(a.firstName) || b.lastName.Equals(a.lastName) ||
                b.phoneNum.Equals(a.phoneNum) || b.email.Equals(a.email))
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             }
            
        }

        //*************************************************************************

        public void AddContact()
        {
            Console.WriteLine("**Add a new contact** \n");

            Console.Write("First name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last name: ");
            string lastName = Console.ReadLine();

            Console.Write("Phone number: ");
            string phoneNum = Console.ReadLine();

            Console.Write("E-mail: ");
            string email = Console.ReadLine();

            if ((firstName == null || firstName.Length == 0) && (lastName == null || lastName.Length == 0))
            {
                Console.WriteLine("ERROR: At least the first or last name must be present.");
            }
            else if ((phoneNum == null || phoneNum.Length == 0) && (email == null || email.Length == 0))
            {
                Console.WriteLine("ERROR: At least a phone number or e-mail must be present.");
            }
            else
            {
                string answer = GetOption("Do you want to add this contact? [Y/N] ", "Y", "N");

                if (answer == "Y")
                {
                    Contact e = new Contact();

                    e.firstName = firstName;
                    e.lastName = lastName;
                    e.phoneNum = phoneNum;
                    e.email = email;

                    allContacts.Add(e);

                    Console.WriteLine("Contact created successfully!");
                    changesSaved = false;
                }
                else
                {
                    Console.WriteLine("Operation was cancelled.");
                }
            }

        }

        //*************************************************************************

        public void EditContact()
        {
            Console.WriteLine("**Edit existing contact** \n");

            int option = GetOption("Search by [1] index or [2] field or [0] cancel: ", 0, 2);

            if (option == 1)
            {
                int s = 0;
                int e = allContacts.Count - 1;
                int index = GetOption($"Choose index of contact [{s}-{e}]: ", s, e);

                EditContact(index);
            }
            else if (option == 2)
            {
                int index;
                Console.Write("Enter Field content: ");
                string search = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("**Edit existing contact** \n");

                Console.WriteLine("###   Name         Last name    Phone Number   E-mail");

                int i = 0;

                foreach (Contact e in allContacts)
                {
                    if (e.firstName == null || e.firstName.Length == 0)
                    {
                        if (e.lastName.Contains(search) || e.phoneNum.Contains(search) || e.email.Contains(search) )
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }
                    else if (e.lastName == null || e.lastName.Length == 0)
                    {
                        if (e.firstName.Contains(search) || e.phoneNum.Contains(search) || e.email.Contains(search) )
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }
                    else if (e.phoneNum == null || e.phoneNum.Length == 0)
                    {
                        if (e.firstName.Contains(search) || e.lastName.Contains(search) || e.email.Contains(search) )
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }
                    else if (e.email == null || e.email.Length == 0)
                    {
                        if (e.firstName.Contains(search) || e.lastName.Contains(search) || e.phoneNum.Contains(search) )
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }
                    else
                    {
                        if (e.firstName.Contains(search) || e.lastName.Contains(search) ||
                        e.phoneNum.Contains(search) || e.email.Contains(search))
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }

                        i++;
                }

                int s = 0;
                int m = allContacts.Count - 1;
                index = GetOption("\nSelect index of which contact to edit: ", s, m);

                EditContact(index);
            }
            else 
            {
                Console.WriteLine("Edit was cancelled.");
            }

        }

        public void EditContact(int index)
        {
            Console.Clear();
            Console.WriteLine("**Edit existing contact** \n");

            Contact e = allContacts[index];
            string text = $"[{index.ToString().PadLeft(3, '0')}]\nFirst name: {e.firstName}\nLast name: {e.lastName}\n" +
                $"Phone number: {e.phoneNum}\nE-mail: {e.email}\n";

            Console.WriteLine(text);
            Console.WriteLine("[1] First name [2] Last name [3] Phone number [4] E-mail ");
            int option = 0;

            do
            {

                option = GetOption("Select property to edit or [0] if done: ", 0, 4);

                if (option == 1)
                {

                    Console.Write("Enter new First name or nothing to cancel: ");
                    string firstName = Console.ReadLine();

                    if (firstName == null || firstName.Length == 0)
                    {
                        Console.WriteLine("Edit was canceled.");
                    }
                    else
                    {
                        e.firstName = firstName;
                        Console.WriteLine("Edit was successful!");
                        changesSaved = false;
                    }

                }
                else if (option == 2)
                {

                    Console.Write("Enter new Last name or nothing to cancel: ");
                    string lastName = Console.ReadLine();

                    if (lastName == null || lastName.Length == 0)
                    {
                        Console.WriteLine("Edit was canceled.");
                    }
                    else
                    {
                        e.lastName = lastName;
                        Console.WriteLine("Edit was successful!");
                        changesSaved = false;
                    }

                }
                else if (option == 3)
                {
                    Console.Write("Enter new Phone number or nothing to cancel: ");
                    string phoneNum = Console.ReadLine();

                    if (phoneNum == null || phoneNum.Length == 0)
                    {
                        Console.WriteLine("Edit was canceled.");
                    }
                    else
                    {
                        e.phoneNum = phoneNum;
                        Console.WriteLine("Edit was successful!");
                        changesSaved = false;
                    }
                }
                else if (option == 4)
                {
                    Console.Write("Enter new E-mail or nothing to cancel: ");
                    string email = Console.ReadLine();

                    if (email == null || email.Length == 0)
                    {
                        Console.WriteLine("Edit was canceled.");
                    }
                    else
                    {
                        e.email = email;
                        Console.WriteLine("Edit was successful!");
                        changesSaved = false;
                    }
                }

            } while (option != 0);

            text = $"[{index.ToString().PadLeft(3, '0')}]\nFirst name: {e.firstName}\nLast name: {e.lastName}\n" +
                $"Phone number: {e.phoneNum}\nE-mail: {e.email}\n";

            Console.WriteLine(text);

        }

        //*************************************************************************

        public void DeleteContact()
        {
            Console.WriteLine("**Delete existing contact** \n");

            int option = GetOption("Search by [1] index or [2] field or [0] cancel: ", 0, 2);

            if (option == 1)
            {
                int s = 0;
                int e = allContacts.Count - 1;
                int index = GetOption($"Choose index of contact [{s}-{e}]: ", s, e);

                DeleteContact(index);
            }
            else if (option == 2)
            {
                int index;
                Console.Write("Enter Field content: ");
                string search = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("**Delete existing contact** \n");

                Console.WriteLine("###   Name         Last name    Phone Number   E-mail");

                int i = 0;

                foreach (Contact e in allContacts)
                {
                    if (e.firstName == null || e.firstName.Length == 0)
                    {
                        if (e.lastName.Contains(search) || e.phoneNum.Contains(search) || e.email.Contains(search))
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }
                    else if (e.lastName == null || e.lastName.Length == 0)
                    {
                        if (e.firstName.Contains(search) || e.phoneNum.Contains(search) || e.email.Contains(search))
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }
                    else if (e.phoneNum == null || e.phoneNum.Length == 0)
                    {
                        if (e.firstName.Contains(search) || e.lastName.Contains(search) || e.email.Contains(search))
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }
                    else if (e.email == null || e.email.Length == 0)
                    {
                        if (e.firstName.Contains(search) || e.lastName.Contains(search) || e.phoneNum.Contains(search))
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }
                    else
                    {
                        if (e.firstName.Contains(search) || e.lastName.Contains(search) ||
                        e.phoneNum.Contains(search) || e.email.Contains(search))
                        {

                            Contact t = allContacts[i];
                            Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {t.firstName,-12} {t.lastName,-12} {t.phoneNum,-14} {t.email,-20}");

                        }
                    }

                    i++;
                }

                int s = 0;
                int m = allContacts.Count - 1;
                index = GetOption("\nSelect index of which contact to delete: ", s, m);

                DeleteContact(index);
            }
            else
            {
                Console.WriteLine("Deletion was cancelled.");
            }
        }

        public void DeleteContact(int index)
        {
            Console.Clear();
            Console.WriteLine("**Delete existing contact** \n");

            Contact e = allContacts[index];

            Console.WriteLine($"[{index.ToString().PadLeft(3, '0')}]\nFirst name: {e.firstName}\nLast name: {e.lastName}\n" +
                $"Phone number: {e.phoneNum}\nE-mail: {e.email}\n");

            string option = GetOption("Are you sure that you want to delete this contact? [Y/N] ", "Y", "N");

            if (option == "Y")
            {
                allContacts.RemoveAt(index);
                Console.WriteLine("Successfully removed contact.");
                changesSaved = false;
            }
            else
            {
                Console.WriteLine("The operation was cancelled.");
            }

        }

        //*************************************************************************

        public void SaveContactsToFile()
        {
            Console.WriteLine("**Save contacts to a file** \n");

            Console.WriteLine("Saving to: ");
            Console.WriteLine("Write the filename or nothing to cancel.");
            Console.Write("Filename: ");
            string filename = Console.ReadLine();

            SaveContactsToFile(filename);

        }

        public void SaveContactsToFile(string filename)
        {
            if (filename == null || filename.Length == 0)
            {
                Console.WriteLine("The operation was canceled.");
            }
            else if (!File.Exists(filename))
            {

                FileStream fs = new FileStream(filename,
                FileMode.Create,
                FileAccess.Write,
                FileShare.Write, 1024,
                FileOptions.Asynchronous | FileOptions.WriteThrough);

                fs.Dispose();

                StreamWriter sw = null;

                try
                {
                    sw = new StreamWriter(filename);

                    foreach (Contact e in allContacts)
                    {
                        sw.WriteLine(e.firstName);
                        sw.WriteLine(e.lastName);
                        sw.WriteLine(e.phoneNum);
                        sw.WriteLine(e.email);
                    }

                    Console.WriteLine("Contacts book saved successfully!");
                    changesSaved = true;

                }
                catch (Exception)
                {
                    Console.WriteLine("ERROR: An error ocurred while saving the file!");
                }
                finally
                {
                    if (sw != null) { sw.Dispose(); }
                }
            }
            else
            {
                string option = GetOption($"WARNING: The file \"{filename}\" already exists," +
                    $"\ndo you wish to override it? [Y/N] ", "Y", "N");
                if (option == "Y")
                {
                    StreamWriter sw = null;

                    try
                    {
                        sw = new StreamWriter(filename);

                        foreach (Contact e in allContacts)
                        {
                            sw.WriteLine(e.firstName);
                            sw.WriteLine(e.lastName);
                            sw.WriteLine(e.phoneNum);
                            sw.WriteLine(e.email);
                        }

                        Console.WriteLine("Contacts book saved successfully!");
                        changesSaved = true;

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("ERROR: An error ocurred while saving the file!");
                    }
                    finally
                    {
                        if (sw != null) { sw.Dispose(); }
                    }
                }
                else
                {
                    Console.WriteLine("Saving has been cancelled.");
                }
            }

        }

        //**************************************************************************

        public bool Exit()
        {
            Console.WriteLine("**Exit application** \n");

            if (changesSaved == false)
            {
                Console.WriteLine("WARNING: You have made changes to the contact list that have not been stored.\n");
            }

            string answer = GetOption("Are you sure that you want to exit? [Y/N] ", "Y", "N");
            if (answer == "Y")
            {
                Console.WriteLine("\nThanks for using contacts book, see you next time!");
                return true;
            }
            else
            {
                return false;
            }
        }

        //**************************************************************************

        public class Contact
        {

            public string firstName;
            public string lastName;
            public string phoneNum;
            public string email;

            public Contact()
            {
                firstName = "";
                lastName = "";
                phoneNum = "";
                email = "";
            }

        }

        //**************************************************************************

        private class FirstNameComparer : IComparer<Contact>
        {
            public int Compare(Contact a, Contact b)
            {
                return string.Compare(a.firstName, b.firstName);
            }
        }

        private class LastNameComparer : IComparer<Contact>
        {
            public int Compare(Contact a, Contact b)
            {
                return string.Compare(a.lastName, b.lastName);
            }
        }

        private class PhoneNumComparer : IComparer<Contact>
        {
            public int Compare(Contact a, Contact b)
            {
                return string.Compare(a.phoneNum, b.phoneNum);
            }
        }

        private class EmailComparer : IComparer<Contact>
        {
            public int Compare(Contact a, Contact b)
            {
                return string.Compare(a.email, b.email);
            }
        }

        //**************************************************************************

        public int GetOption(string prompt, int min, int max)
        {
            Console.Write(prompt);
            string input = Console.ReadLine().Trim();
            int option;
            while (!int.TryParse(input, out option) || option < min || option > max)
            {
                Console.WriteLine("ERROR: Invalid option.");
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
            }
            return option;
        }

        public string GetOption(string prompt, params string[] validOptions)
        {
            Console.Write(prompt);
            string option = Console.ReadLine().Trim().ToUpper();
            while (!validOptions.Contains(option))
            {
                Console.WriteLine("ERROR: Invalid option.");
                Console.Write(prompt);
                option = Console.ReadLine().Trim().ToUpper();
            }
            return option;
        }

    }
}
