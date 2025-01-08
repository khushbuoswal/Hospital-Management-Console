using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;

namespace LoginSystem
{

    // Represents a person with basic contact information.
    public class Person
    {
        // Unique identifier for the person.
        public string Id { get; set; }

        // Person's first name.
        public string FirstName { get; set; }

        // Person's last name.
        public string LastName { get; set; }

        // Person's email address.
        public string Email { get; set; }

        // Person's phone number.
        public string Phone { get; set; }

        // Street number of the person's address.
        public string StreetNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        // Constructor to initialize a Person object with the provided parameters.
        public Person(string id, string firstName, string lastName, string email, string phone, string streetNumber, string street, string city, string state)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            StreetNumber = streetNumber;
            Street = street;
            City = city;
            State = state;
        }
    }

    // Represents a patient, inheriting properties from Person.
    public class Patient : Person
    {
        // Constructor to initialize a Patient object, calling the base Person constructor.
        public Patient(string id, string firstName, string lastName, string email, string phone, string streetNumber, string street, string city, string state)
            : base(id, firstName, lastName, email, phone, streetNumber, street, city, state)
        {
        }

        // Overrides the ToString method to provide a formatted string representation of a Patient object.
        public override string ToString()
        {
            return $"Patient ID: {Id}, Name: {FirstName} {LastName}, Email: {Email}, Phone: {Phone}";
        }

    }

    // Represents a doctor, inheriting properties from Person.
    public class Doctor : Person
    {
        // Constructor to initialize a Doctor object, calling the base Person constructor.
        public Doctor(string id, string firstName, string lastName, string email, string phone, string streetNumber, string street, string city, string state)
            : base(id, firstName, lastName, email, phone, streetNumber, street, city, state)
        {
        }

        // Overrides the ToString method to provide a formatted string representation of a Doctor object.
        public override string ToString()
        {
            return $"Doctor ID: {Id}, Name: {FirstName} {LastName}, Email: {Email}, Phone: {Phone}";
        }

    }

    // Contains extension methods for string manipulation, particularly for ID generation.
    public static class IdExtensions
    {
        // Generates a unique ID based on a prefix and checks against existing IDs stored in a file.
        public static string GenerateUniqueId(this string prefix, string filePath)
        {
            // Read all existing IDs from the file
            var existingIds = new HashSet<string>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var details = line.Split(',');
                    if (details.Length >= 1)
                    {
                        existingIds.Add(details[0]); // Assuming ID is the first item in each line
                    }
                }
            }

            // Start with the lowest possible ID and increment until we find an available one
            for (int i = 1; i <= 9999; i++)
            {
                string id = $"{prefix}{i:D4}"; // e.g., D0001, P0001, etc.
                if (!existingIds.Contains(id)) // Check if the generated ID is available.
                {
                    return id; // Return the first available unique ID.
                }
            }
            // Throw an exception if no available ID is found.
            throw new InvalidOperationException($"No available {prefix} IDs.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ShowLoginMenu(); // Show login on startup
        }

        static void ShowLoginMenu()
        {
            // Initialize empty strings for user ID and password
            string id = string.Empty;
            string password = string.Empty;

            // Infinite loop to keep prompting for login until successful
            while (true) // Keep retrying until valid login
            {
                Console.Clear(); // Clear the console for a fresh login screen
                Console.WriteLine(" -------------------------------------- ");
                Console.WriteLine("|           DOTNET Hospital System     |");
                Console.WriteLine("|               Login                  |");
                Console.WriteLine(" -------------------------------------- ");

                // Prompt for ID
                Console.Write("ID: ");
                id = Console.ReadLine();

                // Prompt for Password, mask it with '*'
                Console.Write("Password: ");
                password = GetPassword();

                // Validate credentials for each role
                if (ValidateCredentials(id, password, "credentials.txt"))
                {
                    Console.WriteLine("Patient login successful.");
                    ShowPatientMenu(id);
                }
                else if (ValidateCredentials(id, password, "userIdDB.txt"))
                {
                    Console.WriteLine("Doctor login successful.");
                    ShowDoctorMenu(id);
                }
                else if (ValidateCredentials(id, password, "userIDB.txt"))
                {
                    Console.WriteLine("Administrator login successful.");
                    ShowAdministratorMenu();
                }
                else
                {
                    // Handle invalid login attempts
                    Console.WriteLine("Invalid credentials, please try again.");
                    Console.ReadKey(); // Pause to let the user read the error
                }
            }
        }

        // Method to mask password input with '*'
        static string GetPassword()
        {
            string pass = string.Empty; // Initialize an empty string for the password
            ConsoleKeyInfo key; // Variable to capture key presses

            do
            {
                key = Console.ReadKey(true); // Read key without displaying it
                // Check if the key is not Backspace or Enter
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar; // Append the character to the password
                    Console.Write("*"); // Display '*' for each character entered
                }
                // Handle backspace to remove the last character
                else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1); // Remove last character
                    Console.Write("\b \b"); // Move cursor back, overwrite last '*' with space
                }
            } while (key.Key != ConsoleKey.Enter); // Continue until Enter is pressed

            Console.WriteLine(); // Move to the next line after password input
            return pass;
        }

        // Method to validate credentials from the respective file
        static bool ValidateCredentials(string id, string password, string filePath)
        {
            // Check if the credentials file exists
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath); // Read all lines from the file
                foreach (var line in lines) // Iterate through each line
                {
                    var details = line.Split(','); // Split line by comma to extract ID and password
                    if (details.Length >= 2) // Ensure there are enough parts to validate
                    {
                        string userId = details[0]; // First part is the user ID
                        string userPassword = details[1]; // Second part is the user password

                        // Check if ID and password match
                        if (userId == id && userPassword == password)
                        {
                            return true; // Return true if credentials are valid
                        }
                    }
                }
            }

            return false; // Return false if no match is found
        }

        // Method to display the Administrator menu and handle user inputs
        static void ShowAdministratorMenu()
        {
            bool exitSystem = false; // Flag to control the loop

            // Keep showing the menu until the user decides to exit
            while (!exitSystem)
            {
                Console.Clear(); // Clear the console before displaying the menu
                // Displaying the Administrator menu options
                Console.WriteLine(" -------------------------------------- ");
                Console.WriteLine("|          DOTNET Hospital             |");
                Console.WriteLine("|         Administrator System         |");
                Console.WriteLine("|              Menu                    |");
                Console.WriteLine(" -------------------------------------- ");
                Console.WriteLine("\nAdministrator Menu:");
                Console.WriteLine("1. List All Doctors");
                Console.WriteLine("2. Check Doctor Details");
                Console.WriteLine("3. List All Patients");
                Console.WriteLine("4. Check Patient Details");
                Console.WriteLine("5. Add Doctor");
                Console.WriteLine("6. Add Patient");
                Console.WriteLine("7. Logout");
                Console.WriteLine("8. Exit System");
                Console.Write("\nPlease select an option (1-8): ");

                string userInput = Console.ReadLine(); // Get user input

                // Switch statement to handle different user selections
                switch (userInput)
                {
                    case "1":
                        ListDoctorDetails(); // Call method to list all doctors
                        break;
                    case "2":
                        CheckDoctorDetails(); // Call method to check a specific doctor's details
                        break;
                    case "3":
                        ListAllPatients(); // Call method to list all patients
                        break;
                    case "4":
                        CheckPatientDetails(); // Call method to check specific patient details
                        break;
                    case "5":
                        AddDoctor(); // Call method to add a new doctor
                        break;
                    case "6":
                        AddPatient(); // Call method to add a new patient
                        break;
                    case "7":
                        exitSystem = true; // Exit to Login
                        Console.WriteLine("Exiting to login...");
                        break;
                    case "8":
                        exitSystem = true; // Exit System
                        Console.WriteLine("Exiting system...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 8.");
                        Console.ReadKey(); // Wait for the user to acknowledge before returning to menu
                        break;
                }
            }
        }

        // Overload to list all doctors (no parameters)
        static void ListDoctorDetails()
        {
            Console.Clear(); // Clear the console before displaying doctor details
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|         List All Doctors             |");
            Console.WriteLine(" -------------------------------------- ");

            // Adding a blank line for space
            Console.WriteLine();

            // Here, you would fetch and display all doctor details
            Console.WriteLine("Displaying all doctors...");
            Console.WriteLine("-------------------------");
            // Adding another blank line for space before displaying the content
            Console.WriteLine();

            try
            {
                // Check if the doctor file exists
                if (File.Exists("userIdDB.txt"))
                {
                    var lines = File.ReadAllLines("userIdDB.txt"); // Read all lines from the doctor file

                    bool foundDoctor = false; // Flag to track if doctors are found

                    // Loop through each line and parse the doctor's details
                    foreach (var line in lines)
                    {
                        string[] details = line.Split(',');

                        // Doctors' IDs start with "D"
                        if (details.Length >= 10)
                        {
                            foundDoctor = true;
                            var doctor = new Doctor(details[0], details[2], details[3], details[4], details[5], details[6], details[7], details[8], details[9]);
                            Console.WriteLine(doctor.ToString());
                            Console.WriteLine("--------------------------------------------------------------------------------------------------------------------");

                        }
                    }

                    if (!foundDoctor)
                    {
                        Console.WriteLine();
                        Console.WriteLine("No doctors found in the system.");
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("No doctor records found. File does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while listing doctors: {ex.Message}");
            }

            ReturnToMenu();
        }

        // Method to check the details of a specific doctor by ID
        static void CheckDoctorDetails()
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|       Check Doctor Details           |");
            Console.WriteLine(" -------------------------------------- ");

            // Adding a blank line for space
            Console.WriteLine();

            try
            {
                // Prompt for doctor ID
                Console.Write("Enter Doctor ID: ");
                string id = Console.ReadLine();

                Console.WriteLine("--------------------------------------");

                // Adding a blank line for space
                Console.WriteLine();

                // Find the doctor by ID
                bool doctorFound = false;

                // Check if the file exists and search for the doctor by ID
                if (File.Exists("userIdDB.txt"))
                {
                    var lines = File.ReadAllLines("userIdDB.txt");

                    foreach (var line in lines)
                    {
                        string[] details = line.Split(',');

                        // Check if the ID matches and it's a valid doctor
                        if (details[0] == id && id.StartsWith("D"))
                        {
                            doctorFound = true;
                            Console.WriteLine("Doctor Details:");

                            // Adding a blank line for space
                            Console.WriteLine();
                            Console.WriteLine("Doctor ID: " + details[0]);
                            Console.WriteLine("Name: " + details[2] + " " + details[3]);
                            Console.WriteLine("Email: " + details[4]);
                            Console.WriteLine("Phone: " + details[5]);
                            Console.WriteLine($"Address: {details[6]}{details[7]},{details[8]},{details[9]}");
                            Console.WriteLine("--------------------------------------");
                            break;
                        }
                    }
                }

                // If no matching doctor was found
                if (!doctorFound)
                {
                    Console.WriteLine();
                    Console.WriteLine("No doctor found with the given ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking doctor details: {ex.Message}");
            }

            ReturnToMenu(); // Return to the Administrator menu
        }

        // Method to list all patients
        static void ListAllPatients()
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|         List All Patients            |");
            Console.WriteLine(" -------------------------------------- ");

            // Adding a blank line for space
            Console.WriteLine();

            // Here, fetch and display all patient details
            Console.WriteLine("Displaying all patients...");

            // Adding a blank line for space
            Console.WriteLine();

            try
            {
                // Ensure the file exists
                if (File.Exists("credentials.txt"))
                {
                    // Read all lines from the file
                    var lines = File.ReadAllLines("credentials.txt");

                    if (lines.Length == 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("No patients found in the system.");
                    }
                    else
                    {
                        // Loop through each line and display the patient details
                        foreach (var line in lines)
                        {
                            string[] details = line.Split(',');

                            if (details.Length >= 10) // Ensure to have enough fields to display
                            {
                                var patient = new Patient(details[0], details[2], details[3], details[4], details[5], details[6], details[7], details[8], details[9]);
                                Console.WriteLine(patient.ToString());
                                Console.WriteLine("--------------------------------------------------------------------------------------------------------------------");

                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("Corrupted data in file for this patient.");
                            }
                        }
                    }
                }
                else
                {
                    // File not found message
                    Console.WriteLine();
                    Console.WriteLine("No patient records found. File does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while listing patients: {ex.Message}");
            }
            ReturnToMenu();
        }

        static void CheckPatientDetails()
        {
            Console.Clear();
            // Display header for the "Check Patient Details" section
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|       Check Patient Details          |");
            Console.WriteLine(" -------------------------------------- ");

            // Adding a blank line for space
            Console.WriteLine();
            try
            {
                // Here, you would prompt for a patient ID and display the details
                Console.Write("Enter Patient ID: ");
                string inputID = Console.ReadLine();
                Console.WriteLine("--------------------------------------");

                // Adding a blank line for space
                Console.WriteLine();

                // Attempt to fetch the patient details from the file
                string patientDetails = GetPatientDetailsFromFile(inputID);

                // If no details are found, inform the user; otherwise, display patient details
                if (string.IsNullOrEmpty(patientDetails))
                {
                    Console.WriteLine();
                    Console.WriteLine("No patient found with that ID.");
                }
                else
                {
                    Console.WriteLine("Patient Details:");
                    // Adding a blank line for space
                    Console.WriteLine();

                    // Split the patient details string into an array and display specific fields
                    string[] details = patientDetails.Split(',');
                    Console.WriteLine($"Name: {details[2]} {details[3]}"); // Patient's first and last name
                    Console.WriteLine($"Email: {details[4]}"); // Patient's email address
                    Console.WriteLine($"Phone: {details[5]}"); // Patient's phone number
                    Console.WriteLine($"Address: {details[6]} {details[7]}, {details[8]}, {details[9]}"); // Patient's full address
                    Console.WriteLine("--------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking patient details: {ex.Message}");
            }
            // Wait for user input to return to menu
            ReturnToMenu();
        }

        static string GetPatientDetailsFromFile(string patientID)
        {
            // Ensure that the file exists
            if (File.Exists("credentials.txt"))
            {
                // Read each line of the file
                var lines = File.ReadAllLines("credentials.txt");

                // Loop through each line to find the matching patient ID
                foreach (var line in lines)
                {
                    // Split the line by comma and compare the first element (patient ID) with the input ID
                    string[] details = line.Split(',');
                    if (details[0] == patientID) // Compare the ID
                    {
                        return line; // Return the matching line with patient details
                    }
                }
            }

            // Return null if the patient is not found
            return null;
        }

        static List<Doctor> doctors = new List<Doctor>(); // List to hold doctors in memory

        static int doctorCounter = 1; // Static counter for unique doctor IDs
        static void AddDoctor()
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|            Add Doctor                |");
            Console.WriteLine(" -------------------------------------- ");

            // Adding a blank line for space
            Console.WriteLine();

            // Here, you would prompt for doctor details and add them to the system
            Console.WriteLine("Registering a new doctor with the DOTNET Hospital Management System");
            Console.WriteLine("-------------------------------------------------------------------");

            // Adding a blank line for space
            Console.WriteLine();
            try
            {
                Console.Write("First Name: ");
                string firstName = Console.ReadLine();
                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Write("Phone: ");
                string phone = Console.ReadLine();
                Console.Write("Street Number: ");
                string streetNumber = Console.ReadLine();
                Console.Write("Street: ");
                string street = Console.ReadLine();
                Console.Write("City: ");
                string city = Console.ReadLine();
                Console.Write("State: ");
                string state = Console.ReadLine();
                // Simulate adding doctor

                // Generate a unique ID for doctor
                string id = "D".GenerateUniqueId("userIdDB.txt");

                // Generate a unique password for the doctor
                string password = GenerateUniquePassword("userIdDB.txt");

                // Create and add the new doctor
                Doctor newDoctor = new Doctor(id, firstName, lastName, email, phone, streetNumber, street, city, state);
                doctors.Add(newDoctor);

                // Save the doctor details to the credentials.txt file
                using (StreamWriter sw = File.AppendText("userIdDB.txt"))
                {
                    sw.WriteLine($"{id},{password},{firstName},{lastName},{email},{phone},{streetNumber}, {street}, {city}, {state}");
                }
                Console.WriteLine();
                Console.WriteLine($"Doctor {firstName} {lastName} added successfully with ID {id}.");
                Console.WriteLine("----------------------------------------------------------------------");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the doctor: {ex.Message}");
            }
            ReturnToMenu();

        }

        static List<Patient> patients = new List<Patient>(); // List to hold patients in memory

        static int patientCounter = 1; // Static counter for unique patient IDs
        static void AddPatient()
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|            Add Patient               |");
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine();

            // Here, you would prompt for patient details and add them to the system
            Console.WriteLine("Registering a new Patient with the DOTNET Hospital Management System");
            Console.WriteLine("--------------------------------------------------------------------");

            Console.WriteLine();

            try
            {
                Console.Write("First Name: ");
                string firstName = Console.ReadLine();

                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();

                Console.Write("Email: ");
                string email = Console.ReadLine();

                Console.Write("Phone: ");
                string phone = Console.ReadLine();

                Console.Write("Street Number: ");
                string streetNumber = Console.ReadLine();

                Console.Write("Street: ");
                string street = Console.ReadLine();

                Console.Write("City: ");
                string city = Console.ReadLine();

                Console.Write("State: ");
                string state = Console.ReadLine();

                // Generate a unique ID for patient
                string id = "P".GenerateUniqueId("credentials.txt");

                // Generate a unique password for the patient
                string password = GenerateUniquePassword("credentials.txt");

                // Create and add the new patient
                Patient newPatient = new Patient(id, firstName, lastName, email, phone, streetNumber, street, city, state);
                patients.Add(newPatient);

                // Write patient details to credentials.txt
                WritePatientToFile(newPatient, password);

                Console.WriteLine();
                // Simulate adding patient
                Console.WriteLine($"{firstName} {lastName} added successfully with ID {id}.");
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the doctor: {ex.Message}");
            }
            ReturnToMenu();
        }

        // Method to generate a random unique password
        static string GenerateUniquePassword(string filePath)
        {
            string password;
            var existingPasswords = new HashSet<string>();

            // Read all existing passwords from the file
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath); // Read all lines from the file
                foreach (var line in lines)
                {
                    var details = line.Split(','); // Split the line by commas to extract details
                    if (details.Length >= 2)
                    {
                        existingPasswords.Add(details[1]); // Assuming password is the second item in each line
                    }
                }
            }

            // Generate a new password until it is unique (not found in the existingPasswords set)
            do
            {
                password = GenerateRandomPassword();
            } while (existingPasswords.Contains(password)); // Ensure password is unique

            return password;
        }

        // Method to generate a random password
        static string GenerateRandomPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random(); // Random object to generate random indices
            char[] chars = new char[8]; // Password length is 8 characters
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)]; // Assign random valid character to each position
            }
            return new string(chars); // Convert char array to a string and return
        }

        // Method to write patient details to the file
        static void WritePatientToFile(Patient patient, string password)
        {
            string filePath = "credentials.txt"; // File path for storing patient credentials
            // Prepare the patient details string in CSV format
            string patientDetails = $"{patient.Id},{password},{patient.FirstName},{patient.LastName},{patient.Email},{patient.Phone},{patient.StreetNumber},{patient.Street},{patient.City},{patient.State}";
            // Append the patient details to the file
            File.AppendAllText(filePath, patientDetails + Environment.NewLine);
        }

        static void ShowDoctorMenu(string doctorId)
        {
            bool exitSystem = false;

            while (!exitSystem)
            {
                Console.Clear();
                Console.WriteLine(" -------------------------------------- ");
                Console.WriteLine("|          DOTNET Hospital             |");
                Console.WriteLine("|         Doctor Management            |");
                Console.WriteLine("|              Menu                    |");
                Console.WriteLine(" -------------------------------------- ");
                Console.WriteLine("\nDoctor Menu:");
                Console.WriteLine("1. List Doctor Details");
                Console.WriteLine("2. List Patients");
                Console.WriteLine("3. List Appointments");
                Console.WriteLine("4. Check Particular Patient");
                Console.WriteLine("5. List Appointments with Patient");
                Console.WriteLine("6. Logout");
                Console.WriteLine("7. Exit");
                Console.Write("\nPlease select an option (1-7): ");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        ListDoctorDetails(doctorId);
                        break;
                    case "2":
                        ListPatients(doctorId);
                        break;
                    case "3":
                        ListAppointments(doctorId);
                        break;
                    case "4":
                        CheckParticularPatient();
                        break;
                    case "5":
                        ListAppointmentsWithPatient(doctorId);
                        break;
                    case "6":
                        exitSystem = true; // Logout
                        Console.WriteLine("Logging out...");
                        break;
                    case "7":
                        exitSystem = true; // Exit System
                        Console.WriteLine("Exiting system...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid input.");
                        Console.ReadKey(); // Wait for user to press a key before continuing
                        break;
                }
            }
        }

        // Overload to list a specific doctor's details (with doctorId)
        static void ListDoctorDetails(string doctorId)
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|         Doctor Details               |");
            Console.WriteLine(" -------------------------------------- ");

            // Adding a blank line for space
            Console.WriteLine();
            Console.WriteLine("Your Details....");
            Console.WriteLine("----------------");
            Console.WriteLine();

            // Fetch doctor details from the file
            string doctorDetails = GetDoctorDetails(doctorId);

            if (string.IsNullOrEmpty(doctorDetails))
            {
                Console.WriteLine("\nDoctor details not found.");
            }
            else
            {
                // Split the line into components
                var details = doctorDetails.Split(',');

                if (details.Length >= 10) // Ensure there are enough fields
                {
                    string fullName = $"{details[2]} {details[3]}"; // FirstName LastName
                    string email = details[4];
                    string phone = details[5];

                    // Display the details
                    Console.WriteLine($"Doctor ID: {details[0]}");
                    Console.WriteLine($"Full Name: {fullName}");

                    // Placeholder for address (modify the file later to include real address)
                    Console.WriteLine($"Email: {email}");
                    Console.WriteLine($"Phone: {phone}");
                    Console.WriteLine($"Address: {details[6]}{details[7]},{details[8]},{details[9]}");
                    Console.WriteLine();
                    Console.WriteLine("----------------------------------------------");
                }
                else
                {
                    Console.WriteLine("\nInvalid doctor details format.");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        // Helper method to fetch the doctor details from the userIdDB.txt file
        static string GetDoctorDetails(string doctorId)
        {
            try
            {
                // Read all lines from the file
                if (File.Exists("userIdDB.txt"))
                {
                    var lines = File.ReadAllLines("userIdDB.txt");

                    // Search for the matching doctor ID
                    foreach (var line in lines)
                    {
                        if (line.StartsWith(doctorId))
                        {
                            return line; // Return the full line for this doctor
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading doctor data: {ex.Message}");
            }


            return null; // Return null if doctor not found
        }

        // Method to List Patients
        static void ListPatients(string doctorId)
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|         List of Patients             |");
            Console.WriteLine(" -------------------------------------- ");

            // Adding a blank line for space
            Console.WriteLine();
            Console.WriteLine("Patients Details...");
            Console.WriteLine("-------------------");
            Console.WriteLine();

            // Fetch patient IDs assigned to the doctor from admin.txt
            var patientIds = GetPatientIdsForDoctor(doctorId);

            if (patientIds.Count == 0)
            {
                Console.WriteLine("No patients assigned to this doctor.");
                Console.WriteLine("------------------------------------");
            }
            else
            {
                // Fetch and display details of each patient from credentials.txt
                foreach (var patientId in patientIds)
                {
                    string patientDetails = GetPatientDetails(patientId);
                    if (!string.IsNullOrEmpty(patientDetails))
                    {
                        var details = patientDetails.Split(',');
                        if (details.Length >= 7) // Ensure there are enough fields
                        {
                            // Create a Patient object using the fetched details
                            var patient = new Patient(
                                id: details[0],
                                firstName: details[2],
                                lastName: details[3],
                                email: details[4],
                                phone: details[5],
                                streetNumber: details[6],
                                street: details[7],
                                city: details[8],
                                state: details[9]
                            );

                            // Display patient information using the ToString method
                            Console.WriteLine(patient.ToString());
                            Console.WriteLine("---------------------------------------------------------------------------------------");
                        }
                    }
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        // Helper method to get patient IDs assigned to the doctor
        static List<string> GetPatientIdsForDoctor(string doctorId)
        {
            var patientIds = new List<string>();
            try
            {
                // Check if the file containing doctor-patient assignments exists
                if (File.Exists("admin.txt"))
                {
                    var lines = File.ReadAllLines("admin.txt");

                    // Loop through each line in the file
                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        // If the line contains a valid doctor-patient pairing and matches the doctor ID
                        if (parts.Length == 2 && parts[1] == doctorId)
                        {
                            patientIds.Add(parts[0]); // Add patient ID if it matches the current doctor
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
            return patientIds; // Return the list of patient IDs associated with the doctor
        }

        // Helper method to fetch patient details from credentials.txt
        static string GetPatientDetails(string patientId)
        {
            try
            {
                // Check if the file containing patient credentials exists
                if (File.Exists("credentials.txt"))
                {
                    var lines = File.ReadAllLines("credentials.txt");

                    // Loop through each line to find the patient ID
                    foreach (var line in lines)
                    {
                        // If the line starts with the patient ID, return the details
                        if (line.StartsWith(patientId))
                        {
                            return line; // Return the full line for this patient
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }

            return null; // Return null if patient not found
        }

        // Method to list appointments for a given doctor
        static void ListAppointments(string doctorId)
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|         List of Appointments         |");
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine();

            // Fetch the list of appointments for the given doctor
            var appointments = GetAppointmentsForDoctor(doctorId);

            // Display message if no appointments are found
            if (appointments.Count == 0)
            {
                Console.WriteLine("No appointments found for this doctor.");
            }
            else
            {
                // Display each appointment if found
                foreach (var appointment in appointments)
                {
                    Console.WriteLine(appointment);
                    Console.WriteLine("---------------------------------------------------------------------------------------");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(); // Wait for user input before returning to the menu
        }

        // Helper method to get all appointments involving the doctor
        static List<string> GetAppointmentsForDoctor(string doctorId)
        {
            var appointments = new List<string>();

            try
            {
                // Check if the appointments file exists
                if (File.Exists("appointments.txt"))
                {
                    var lines = File.ReadAllLines("appointments.txt");

                    // Loop through each line in the appointments file
                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');

                        // If the line contains valid appointment details and matches the doctor ID
                        if (parts.Length >= 5 && parts[1] == doctorId)
                        {
                            // Append appointment details if the doctor ID matches
                            string patientId = parts[0];
                            string date = parts[2];
                            string time = parts[3];
                            string description = parts[4];

                            // Add the formatted appointment details to the list
                            appointments.Add($"Patient ID: {patientId}, Date: {date}, Time: {time}, Description: {description}");
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }

            return appointments;
        }

        // Method to check details of a particular patient
        static void CheckParticularPatient()
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|       Check Particular Patient       |");
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine();

            // Prompt for a patient ID
            Console.Write("Enter Patient ID: ");
            string patientId = Console.ReadLine();
            Console.WriteLine("---------------------------------");

            try
            {
                // Check if the patient exists in the file
                if (!File.Exists("credentials.txt"))
                {
                    Console.WriteLine("Patient data file not found.");
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    return;
                }

                var lines = File.ReadAllLines("credentials.txt");
                bool patientFound = false;

                foreach (var line in lines)
                {
                    var details = line.Split(',');

                    // PatientID,Password,FirstName,LastName,Email,Phone,StreetNumber,Street,AddressCity,AddressState
                    if (details.Length >= 10 && details[0] == patientId)
                    {
                        patientFound = true;
                        Console.WriteLine($"\nPatient Details:");
                        Console.WriteLine($"Patient ID: {details[0]}");
                        Console.WriteLine($"First Name: {details[2]}");
                        Console.WriteLine($"Last Name: {details[3]}");
                        Console.WriteLine($"Email: {details[4]}");
                        Console.WriteLine($"Phone: {details[5]}");
                        Console.WriteLine($"Address: {details[6]}{details[7]},{details[8]},{details[9]}");
                        Console.WriteLine("---------------------------------------------------");
                        break;
                    }
                }

                // If no patient is found, display a message
                if (!patientFound)
                {
                    Console.WriteLine($"\nNo patient found with ID: {patientId}");
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(); // Wait for user input before returning to the menu
        }

        // Method to list appointments with a particular patient for a doctor
        static void ListAppointmentsWithPatient(string doctorId)
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("| List Appointments with Patient       |");
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine();

            // Prompt for a patient ID
            Console.Write("Enter Patient ID: ");
            string patientId = Console.ReadLine();
            Console.WriteLine("------------------------- ");

            // Check if the patient is registered with the doctor
            string registeredDoctorId = GetRegisteredDoctorId(patientId);

            // If the patient is not registered with the doctor, display a message
            if (string.IsNullOrEmpty(registeredDoctorId) || registeredDoctorId != doctorId)
            {
                Console.WriteLine($"No appointments found for Patient ID: {patientId} with the currently logged-in doctor.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            // Get doctor and patient names from files
            string doctorName = GetDoctorNameById(doctorId);


            // Display doctor and patient names
            Console.WriteLine($"\nDoctor: {doctorName}");


            // Check if appointments.txt exists
            if (!File.Exists("appointments.txt"))
            {
                Console.WriteLine("No appointments have been booked yet.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            // Read all appointments from appointments.txt
            var lines = File.ReadAllLines("appointments.txt");
            bool hasAppointments = false;

            // Loop through each appointment and check if it matches the patient and doctor
            foreach (var line in lines)
            {
                var details = line.Split(',');
                if (details.Length >= 5 && details[0] == patientId && details[1] == doctorId)
                {
                    hasAppointments = true;
                    string date = details[2];
                    string time = details[3];
                    string notes = details[4];

                    // Display the appointment details
                    Console.WriteLine($"Date: {date}");
                    Console.WriteLine($"Time: {time}");
                    Console.WriteLine($"Notes: {notes}");
                    Console.WriteLine("-------------------------------------- ");
                }
            }

            // If no appointments are found, display a message
            if (!hasAppointments)
            {
                Console.WriteLine($"No appointments found for Patient ID: {patientId}.");
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        // Helper method to get the doctor's name by their ID
        static string GetDoctorNameById(string doctorId)
        {
            // Read from userIdDB.txt to fetch doctor details
            if (File.Exists("userIdDB.txt"))
            {
                var lines = File.ReadAllLines("userIdDB.txt");

                // Loop through each line to find the doctor by their ID
                foreach (var line in lines)
                {
                    var details = line.Split(',');

                    // If the line contains valid details and matches the doctor ID, return the doctor's name
                    if (details.Length >= 4 && details[0] == doctorId)
                    {
                        return $"{details[2]} {details[3]}"; // Return the doctor's first and last name
                    }
                }
            }

            return "Unknown Doctor"; // Return a placeholder if the doctor is not found
        }

        static void ShowPatientMenu(string patientId)
        {
            bool exitSystem = false;

            while (!exitSystem)
            {
                Console.Clear();
                Console.WriteLine(" -------------------------------------- ");
                Console.WriteLine("|          DOTNET Hospital             |");
                Console.WriteLine("|         Patient Management           |");
                Console.WriteLine("|              System                  |");
                Console.WriteLine(" -------------------------------------- ");
                Console.WriteLine("\nPatient Menu:");
                Console.WriteLine("1. List Patient Details");
                Console.WriteLine("2. List My Doctor Details");
                Console.WriteLine("3. List All Appointments");
                Console.WriteLine("4. Book Appointments");
                Console.WriteLine("5. Exit to Login");
                Console.WriteLine("6. Exit System");
                Console.Write("\nPlease select an option (1-6): ");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        ListPatientDetails(patientId);
                        break;
                    case "2":
                        ListMyDoctorDetails1(patientId);
                        break;
                    case "3":
                        ListAllAppointments(patientId);
                        break;
                    case "4":
                        BookAppointments(patientId);
                        break;
                    case "5":
                        exitSystem = true; // Exit to Login
                        Console.WriteLine("Exiting to login...");
                        break;
                    case "6":
                        exitSystem = true; // Exit System
                        Console.WriteLine("Exiting system...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ListPatientDetails(string patientId)
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|         Patient Details              |");
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine();
            // Here, you would fetch and display patient details
            Console.WriteLine("Displaying patient details...");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();

            try
            {
                // Read patient data from the credentials.txt file
                string[] lines = File.ReadAllLines("credentials.txt");
                bool patientFound = false;

                foreach (string line in lines)
                {
                    // Split the line to extract details
                    var details = line.Split(',');

                    if (details.Length >= 9 && details[0] == patientId) // ID should match the logged-in patient
                    {
                        string id = details[0];
                        string fullName = details[2] + " " + details[3]; // First and Last Name
                        string email = details[4];
                        string phone = details[5];
                        string streetNumber = details[6];
                        string street = details[7];
                        string city = details[8];
                        string state = details.Length > 9 ? details[9] : "Unknown";

                        // Display the patient details
                        Console.WriteLine($"Patient ID: {id}");
                        Console.WriteLine($"Full Name: {fullName}");
                        Console.WriteLine($"Address: {streetNumber}, {street}, {city}, {state}");
                        Console.WriteLine($"Email: {email}");
                        Console.WriteLine($"Phone: {phone}");
                        Console.WriteLine();
                        Console.WriteLine("---------------------------------");
                        patientFound = true;
                        break;
                    }
                }

                if (!patientFound)
                {
                    Console.WriteLine();
                    Console.WriteLine("Patient details not found.");
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("The credentials file was not found.");
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while reading the credentials file.");
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine();

            ReturnToMenu();
        }

        // Displays doctor details for the given patient by checking the most recent appointment.
        static void ListMyDoctorDetails1(string patientId)
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|         Doctor Details               |");
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine();

            // Here, fetch and display doctor details
            Console.WriteLine("Displaying doctor details...");
            Console.WriteLine("----------------------------------------");

            Console.WriteLine();

            // Find the most recent appointment of the patient
            string doctorId = string.Empty;
            bool appointmentFound = false;

            try
            {
                // Check if appointments file exists, and read the data.
                if (File.Exists("appointments.txt"))
                {
                    string[] appointments = File.ReadAllLines("appointments.txt");

                    // Reverse loop to find the most recent appointment for the patient.
                    foreach (string appointment in appointments.Reverse())
                    {
                        var details = appointment.Split(',');
                        if (details.Length >= 4 && details[0] == patientId)
                        {
                            doctorId = details[1];
                            appointmentFound = true;
                            break; // Only need the most recent appointment
                        }
                    }
                }

                if (!appointmentFound)
                {
                    Console.WriteLine();
                    Console.WriteLine("No appointment found for the patient.");
                }
                else
                {
                    // Fetch and display the doctor's details
                    if (File.Exists("userIdDB.txt"))
                    {
                        string[] doctors = File.ReadAllLines("userIdDB.txt");
                        bool doctorFound = false;

                        // Loop through the doctor database to find matching doctorId.
                        foreach (string doctor in doctors)
                        {
                            var doctorDetails = doctor.Split(',');
                            if (doctorDetails.Length >= 5 && doctorDetails[0] == doctorId)
                            {
                                // Create a doctor object and populate with retrieved details.
                                Doctor doctorObj = new Doctor(
                                doctorDetails[0], // ID
                                doctorDetails[2], // First Name
                                doctorDetails[3], // Last Name
                                doctorDetails[4], // Email
                                doctorDetails[5], // Phone
                                "", // Street Number (not present in the file)
                                "", // Street (not present in the file)
                                "", // City (not present in the file)
                                ""  // State (not present in the file)
                                 );

                                // Display the doctor's details using the ToString method
                                Console.WriteLine(doctorObj.ToString());
                                doctorFound = true;
                                break;
                            }
                        }

                        if (!doctorFound)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Doctor details not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Doctor details file not found.");
                    }
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine("An error occurred while accessing the file: " + ioEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            ReturnToMenu();
        }

        // Displays all appointments for a given patient.
        static void ListAllAppointments(string patientId)
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|       Your Booked Appointments        |");
            Console.WriteLine(" -------------------------------------- ");

            try
            {
                // Fetch the patient's name from credentials.txt
                string patientName = GetPatientNameById(patientId);
                if (string.IsNullOrEmpty(patientName))
                {
                    Console.WriteLine("\nPatient not found.");
                    ReturnToMenu();
                    return;
                }

                Console.WriteLine($"\nPatient Name: {patientName}");
                Console.WriteLine("-------------------------------------------------");

                // Check if appointments.txt exists
                if (!File.Exists("appointments.txt"))
                {
                    Console.WriteLine("\nNo appointments have been booked yet.");
                    ReturnToMenu();
                    return;
                }

                // Read all lines from the appointments file
                var lines = File.ReadAllLines("appointments.txt");

                bool hasAppointments = false;

                // Loop through each line and check if it matches the patient's ID
                foreach (var line in lines)
                {
                    var details = line.Split(',');
                    if (details.Length >= 5 && details[0] == patientId)
                    {
                        hasAppointments = true;
                        string doctorId = details[1];
                        string date = details[2];
                        string time = details[3];
                        string notes = details[4];

                        // Fetch doctor's name from the doctor file
                        string doctorName = GetDoctorNameById(doctorId);

                        Console.WriteLine($"\nAppointment with Dr. {doctorName}");
                        Console.WriteLine($"Date: {date}");
                        Console.WriteLine($"Time: {time}");
                        Console.WriteLine($"Notes: {notes}");
                        Console.WriteLine("--------------------------------------------------------");
                    }
                }

                if (!hasAppointments)
                {
                    Console.WriteLine("\nYou have no booked appointments.");
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------------");
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine("An error occurred while accessing the file: " + ioEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }
            ReturnToMenu();
        }

        // Helper method to get the patient's name by their ID from credentials.txt
        static string GetPatientNameById(string patientId)
        {
            try
            {
                // Check if credentials.txt exists
                if (File.Exists("credentials.txt"))
                {
                    var lines = File.ReadAllLines("credentials.txt");
                    foreach (var line in lines)
                    {
                        var details = line.Split(',');
                        if (details.Length >= 3 && details[0] == patientId)
                        {
                            return $"{details[2]} {details[3]}"; // Return the patient's first and last name
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException("File credentials.txt not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching patient name: {ex.Message}");
            }


            return null;
        }

        // Handles booking appointments for a patient.
        static void BookAppointments(string patientId)
        {
            Console.Clear();
            Console.WriteLine(" --------------------------------------- ");
            Console.WriteLine("|       Book Appointment                |");
            Console.WriteLine(" --------------------------------------- ");
            try
            {
                string doctorId = GetRegisteredDoctorId(patientId);


                if (string.IsNullOrEmpty(doctorId))
                {
                    // Prompt patient to register with a doctor if not already done.
                    Console.WriteLine();
                    Console.WriteLine("You are not registered with a doctor. Please choose a doctor to register:");
                    Console.WriteLine("-------------------------------------------------------------------------");
                    doctorId = ListAll1Doctors(); // Allow the patient to choose a doctor

                    RegisterDoctorToPatient(patientId, doctorId); // Save the registration
                }
                Console.WriteLine();
                // Now that the patient is registered with a doctor, collect appointment details
                Console.WriteLine("Please enter the following details for your appointment:");
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine();
                // Input date
                Console.Write("Date (YYYY-MM-DD): ");
                string date = Console.ReadLine();

                // Input time
                Console.Write("Time (HH:MM): ");
                string time = Console.ReadLine();

                // Additional notes
                Console.Write("Additional Notes: ");
                string notes = Console.ReadLine();

                // Confirm the appointment
                Console.WriteLine("\nConfirming the appointment...");

                // Save the appointment to a file (appointments.txt)
                SaveAppointment(patientId, doctorId, date, time, notes);

                Console.WriteLine("\nYour appointment has been successfully booked!");
                Console.WriteLine("--------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error booking appointment: {ex.Message}");
            }
            ReturnToMenu();
        }

        // Helper method to check if the patient is already registered with a doctor
        static string GetRegisteredDoctorId(string patientId)
        {
            try
            {
                // Read from the patient-doctor registration file (patientDoctor.txt)
                if (File.Exists("admin.txt"))
                {
                    var lines = File.ReadAllLines("admin.txt");
                    foreach (var line in lines)
                    {
                        var details = line.Split(',');
                        if (details.Length >= 2 && details[0] == patientId)
                        {
                            return details[1]; // Return the doctor ID
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException("File admin.txt not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting registered doctor: {ex.Message}");
            }
            return null; // Not registered
        }

        // Helper method to register a doctor to the patient
        static void RegisterDoctorToPatient(string patientId, string doctorId)
        {
            try
            {
                // Append the patient-doctor registration to a file (patientDoctor.txt)
                using (StreamWriter sw = File.AppendText("admin.txt"))
                {
                    sw.WriteLine($"{patientId},{doctorId}");
                }
                Console.WriteLine();
                Console.WriteLine("Doctor registration successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering doctor to patient: {ex.Message}");
            }
        }

        // Helper method to save the appointment to a file
        static void SaveAppointment(string patientId, string doctorId, string date, string time, string notes)
        {
            try
            {
                // Append the appointment to a file (appointments.txt)
                using (StreamWriter sw = File.AppendText("appointments.txt"))
                {
                    sw.WriteLine($"{patientId},{doctorId},{date},{time},{notes}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving appointment: {ex.Message}");
            }
        }

        // Helper method to list all doctors and allow the patient to choose one
        static string ListAll1Doctors()
        {
            string selectedDoctorId = null;

            try
            {
                // Read all lines from the userIdDB.txt file (assumed to store doctor information)
                var lines = File.ReadAllLines("userIdDB.txt");

                // If no doctors are found in the file, print a message and return null
                if (lines.Length == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("No doctors are available.");
                    return null;
                }

                Console.WriteLine();
                Console.WriteLine("Available Doctors:");
                Console.WriteLine("------------------");
                Console.WriteLine();

                // Loop through each line in the file to extract and display doctor details
                for (int i = 0; i < lines.Length; i++)
                {
                    var details = lines[i].Split(','); // Assuming each line is comma-separated
                    if (details.Length >= 2)
                    {
                        // Display doctor ID and name (assuming details[0] is ID, details[2] is first name, details[3] is last name)
                        Console.WriteLine($"{i + 1}. Doctor ID: {details[0]}, Name: {details[2]} {details[3]}");
                    }
                }

                // Prompt the user to select a doctor by entering the corresponding number
                Console.WriteLine();
                Console.Write("Please select a doctor by entering the corresponding number: ");
                int doctorIndex = int.Parse(Console.ReadLine()) - 1; // Convert input to an index

                // Validate the selected doctor index and extract the corresponding doctor ID
                if (doctorIndex >= 0 && doctorIndex < lines.Length)
                {
                    selectedDoctorId = lines[doctorIndex].Split(',')[0]; // Get the doctor ID
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing doctors: {ex.Message}");
            }

            return selectedDoctorId;
        }

        // Helper method to prompt the user to return to the menu
        static void ReturnToMenu()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(); // Wait for user input before proceeding
        }

        // Method to draw the login screen with a simple user interface
        static void DrawLoginScreen()
        {
            Console.Clear();
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine("|                                      |");
            Console.WriteLine("|        DOTNET Hospital Management    |");
            Console.WriteLine("|                 System               |");
            Console.WriteLine("|                                      |");
            Console.WriteLine("|                 Login                |");
            Console.WriteLine("|                                      |");
            Console.WriteLine(" -------------------------------------- ");
            Console.WriteLine(); // Add some space for the user to start entering login details
        }
    }
}

