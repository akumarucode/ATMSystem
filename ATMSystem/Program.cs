using System;
using System.Data.SQLite;

internal class Program
{
    private static void Main(string[] args)
    {


        // Specify the path to your SQLite database file
        string databasePath = @"ATM_DB.db";
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = System.IO.Path.Combine(basePath, databasePath);

        // Connection string for SQLite
        string connectionString = $"Data Source={fullPath};Version=3;";

        // SQL query to check if data
        string checkCard = "SELECT COUNT(*) FROM Users WHERE CardID = @CardID";
        string checkPIN = "SELECT COUNT(*) FROM Users WHERE CardID = @CardID AND PIN = @PIN";
        string checkBalance = "SELECT AccountNo,Balance FROM Users WHERE CardID = @CardID";
        string callBalance = "SELECT Balance FROM Users WHERE CardID = @CardID";
        string checkRecipientAccount = "SELECT Name FROM Users WHERE AccountNo = @AccountNo";
        string checkIfSameAccount = "SELECT AccountNo FROM Users WHERE CardID = @CardID";
        string updateBalance = "UPDATE Users SET Balance = @newBalance WHERE CardID = @CardID";
        string updateRecipientBalance = "UPDATE Users SET Balance = @newBalance WHERE AccountNo = @AccountNo";
        string callBalanceFromAccountNo = "SELECT Balance FROM Users WHERE AccountNo = @AccountNo";






        // Open a connection to the database
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {


            int counter = 0;

            while (counter < 3)
            {
                //For menu
                bool exitRequested = false;
                bool dataExists2 = false;        // Flag to indicate if data exists
                bool dataExists = false;


                //For Case 2
                bool exitRequested2 = false;
                bool exitRequested3 = false;

                

                //Enter Card ID 
                Console.WriteLine("Enter Card ID:");
                string cardID = Console.ReadLine();

                connection.Open();

                // Create a command with the query and parameters
                using (SQLiteCommand command = new SQLiteCommand(checkCard, connection))
                {
                    // Add parameter to the command
                    command.Parameters.AddWithValue("@CardID", cardID);

                    // Execute the query and get the result
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // Check if any rows exist
                    if (count > 0)
                    {
                        dataExists = true;
                    }
                }

                // Output the result
                if (dataExists)
                {
                    Console.WriteLine("Enter PIN:");
                    string PINString = Console.ReadLine();

                    if (int.TryParse(PINString, out int PIN))
                    {
                        // Create a command with the query and parameters
                        using (SQLiteCommand command2 = new SQLiteCommand(checkPIN, connection))
                        {
                            // Add parameter to the command
                            command2.Parameters.AddWithValue("@PIN", PIN);
                            command2.Parameters.AddWithValue("@CardID", cardID);

                            // Execute the query and get the result
                            int count2 = Convert.ToInt32(command2.ExecuteScalar());

                            // Check if any rows exist
                            if (count2 > 0)
                            {
                                dataExists2 = true;
                            }


                            if (dataExists2)
                            {
                                //Menu
                                while (!exitRequested)
                                {
                                    Console.WriteLine("===================");
                                    Console.WriteLine("Welcome to ATM!");
                                    Console.WriteLine("Menu:");
                                    Console.WriteLine("1.Check balanced");
                                    Console.WriteLine("2.Transfer");
                                    Console.WriteLine("3.Withdraw");
                                    Console.WriteLine("4.Exit");
                                    Console.WriteLine("===================");
                                    Console.WriteLine();

                                    Console.WriteLine("Please enter your choice: ");
                                    string choiceString = Console.ReadLine();
                                    int choice;
                                    if (int.TryParse(choiceString, out choice))
                                    {
                                        switch (choice)
                                        {
                                            case 1:
                                                using (SQLiteCommand command3 = new SQLiteCommand(checkBalance, connection))
                                                {

                                                    // Add parameter to the command
                                                    command3.Parameters.AddWithValue("@CardID", cardID);

                                                    using (SQLiteDataReader reader = command3.ExecuteReader())
                                                    {
                                                        while (reader.Read())
                                                        {
                                                            // Access data
                                                            Console.WriteLine($"Account No: {reader["AccountNo"]}, Balance (RM): {reader["Balance"]}"); //                Example, replace with your logic
                                                        }
                                                    }
                                                }
                                                break;

                                            case 2:

                                                exitRequested2 = false;
                                                exitRequested3 = false;
                                                while (!exitRequested2)
                                                {
                                                    Console.WriteLine("Please enter the recipient account (Enter 999 to exit to menu): ");
                                                    string recipientAccount = Console.ReadLine();

                                                    try

                                                    {
                                                        int convertedAccountNo = Convert.ToInt32(recipientAccount);

                                                        if (convertedAccountNo == 999)
                                                        {
                                                            exitRequested2 = true;
                                                            exitRequested3 = true;
                                                        }

                                                        else
                                                        {

                                                            using (SQLiteCommand checkIfAccountSameCommand = new SQLiteCommand(checkIfSameAccount, connection))
                                                            {
                                                                checkIfAccountSameCommand.Parameters.AddWithValue("@CardID", cardID);

                                                                using (SQLiteDataReader sameAccountReader = checkIfAccountSameCommand.ExecuteReader())
                                                                {
                                                                    sameAccountReader.Read();

                                                                    string OwnAccountNo = sameAccountReader["AccountNo"].ToString();



                                                                    if (OwnAccountNo == recipientAccount)
                                                                    {
                                                                        Console.WriteLine("You cannot transfer to your own account bruh~~~~");
                                                                    }

                                                                    else
                                                                    {
                                                                        exitRequested3 = false;

                                                                        using (SQLiteCommand command5 = new SQLiteCommand(checkRecipientAccount, connection))
                                                                        {
                                                                            // Add parameter to the command
                                                                            command5.Parameters.AddWithValue("@AccountNo", recipientAccount);
                                                                            using (SQLiteDataReader reader = command5.ExecuteReader())
                                                                            {
                                                                                if (reader.HasRows)
                                                                                {
                                                                                    while (reader.Read())
                                                                                    {


                                                                                        // Access data
                                                                                        Console.WriteLine($"Account No: {recipientAccount}, Name: {reader["Name"]}");

                                                                                        while (!exitRequested3)
                                                                                        {

                                                                                            Console.WriteLine("Is the Account No. correct? (1=Yes,2=No) ");
                                                                                            int correctORnot = Convert.ToInt16(Console.ReadLine());

                                                                                            if (correctORnot == 1)
                                                                                            {

                                                                                                try
                                                                                                {
                                                                                                    Console.WriteLine("Please enter transfer amount: ");
                                                                                                    double transferAmount = Convert.ToDouble(Console.ReadLine());

                                                                                                    if (transferAmount > 0)
                                                                                                    {
                                                                                                        using (
                                                                                                            SQLiteCommand command8 = new SQLiteCommand(callBalance, connection))
                                                                                                        {
                                                                                                            // Add parameter to the command
                                                                                                            command8.Parameters.AddWithValue("@CardID", cardID);

                                                                                                            var reader3 = command8.ExecuteReader();

                                                                                                            while (reader3.Read())
                                                                                                            {
                                                                                                                // Access and store data in variables
                                                                                                                string currentSenderBalance = reader3["Balance"].ToString();
                                                                                                                double DoubleSenderBalance;
                                                                                                                if (double.TryParse(currentSenderBalance, out DoubleSenderBalance))
                                                                                                                {

                                                                                                                    if (DoubleSenderBalance > transferAmount)
                                                                                                                    {

                                                                                                                        using (SQLiteCommand command6 = new SQLiteCommand(callBalanceFromAccountNo, connection))
                                                                                                                        {
                                                                                                                            // Add parameter to the command
                                                                                                                            command6.Parameters.AddWithValue("@AccountNo", recipientAccount);

                                                                                                                            var reader2 = command6.ExecuteReader();

                                                                                                                            // Attempt to read rows from the result set
                                                                                                                            while (reader2.Read())
                                                                                                                            {
                                                                                                                                // Access and store data in variables
                                                                                                                                string currentRecipientBalance = reader2["Balance"].ToString();
                                                                                                                                double doubleCurrentRecipientBalance;


                                                                                                                                if (double.TryParse(currentRecipientBalance, out doubleCurrentRecipientBalance))
                                                                                                                                {

                                                                                                                                    double newRecipientBalance = doubleCurrentRecipientBalance + transferAmount;
                                                                                                                                    using (SQLiteCommand command7 = new SQLiteCommand(updateRecipientBalance, connection))
                                                                                                                                    {
                                                                                                                                        command7.Parameters.AddWithValue("@newBalance", newRecipientBalance);
                                                                                                                                        command7.Parameters.AddWithValue("@AccountNo", recipientAccount);
                                                                                                                                        command7.ExecuteNonQuery();
                                                                                                                                    }

                                                                                                                                    Console.WriteLine("Transfer successful!");


                                                                                                                                }

                                                                                                                            }

                                                                                                                        }

                                                                                                                        double newSenderBalance = DoubleSenderBalance - transferAmount;
                                                                                                                        using (SQLiteCommand command9 = new SQLiteCommand(updateBalance, connection))
                                                                                                                        {
                                                                                                                            command9.Parameters.AddWithValue("@newBalance", newSenderBalance);
                                                                                                                            command9.Parameters.AddWithValue("@CardID", cardID);
                                                                                                                            command9.ExecuteNonQuery();
                                                                                                                            exitRequested2 = true;
                                                                                                                            exitRequested3 = true;
                                                                                                                        }
                                                                                                                    }

                                                                                                                    else
                                                                                                                    {
                                                                                                                        Console.WriteLine("Insufficient balance!");

                                                                                                                    }

                                                                                                                }


                                                                                                            }


                                                                                                        }

                                                                                                    }

                                                                                                    else
                                                                                                    {
                                                                                                        Console.WriteLine("Invalid value, dont enter 0 or negative");
                                                                                                    }

                                                                                                }
                                                                                                catch (Exception e) { Console.WriteLine(e); }



                                                                                            }

                                                                                            else if (correctORnot == 2)
                                                                                            {
                                                                                                exitRequested3 = true;
                                                                                            }

                                                                                            else
                                                                                            {
                                                                                                Console.WriteLine("Invalid input , please enter 1 or 2");
                                                                                            }

                                                                                        }

                                                                                    }
                                                                                }

                                                                                else
                                                                                {
                                                                                    Console.WriteLine("Account No doesn't exist!");
                                                                                }



                                                                            }

                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }


                                                    }

                                                    catch (FormatException)
                                                    {
                                                        Console.WriteLine("The string is not a valid integer.");
                                                    }
                                                    catch (ArgumentNullException)
                                                    {
                                                        Console.WriteLine("The string is null.");
                                                    }


                                                }


                                                break;
                                            case 3:
                                                using (SQLiteCommand command4 = new SQLiteCommand(callBalance, connection))
                                                {
                                                    // Add parameter for the ID
                                                    command4.Parameters.AddWithValue("@CardID", cardID);

                                                    using (SQLiteDataReader reader = command4.ExecuteReader())
                                                    {
                                                        if (reader.Read())
                                                        {
                                                            // Access and store data in variables
                                                            string currentBalance = reader["Balance"].ToString();

                                                            // Convert column1Value to double
                                                            double doubleCurrentBalance;
                                                            if (double.TryParse(currentBalance, out doubleCurrentBalance))
                                                            {

                                                                Console.WriteLine("Enter amount:");
                                                                string withdrawAmountString = Console.ReadLine();




                                                                if (double.TryParse(withdrawAmountString, out double withdrawAmount))
                                                                {

                                                                    if (doubleCurrentBalance > withdrawAmount)
                                                                    {
                                                                        if (withdrawAmount > 0)
                                                                        {

                                                                            double afterWithdraw = doubleCurrentBalance - withdrawAmount;
                                                                            Console.WriteLine("Please take the money, your new balance is RM " + afterWithdraw);

                                                                            using (SQLiteCommand command5 = new SQLiteCommand(updateBalance, connection))
                                                                            {
                                                                                command5.Parameters.AddWithValue("@newBalance", afterWithdraw);
                                                                                command5.Parameters.AddWithValue("@CardID", cardID);
                                                                                command5.ExecuteNonQuery();
                                                                            }

                                                                            exitRequested = true;
                                                                        }

                                                                        else
                                                                        {
                                                                            Console.WriteLine("Invalid input, please enter value more than 0");
                                                                        }

                                                                    }

                                                                    else
                                                                    {
                                                                        Console.WriteLine("Insufficient amount! , your balance is only " + currentBalance);
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    // Failed to parse the input to a double
                                                                    Console.WriteLine("Invalid input. Please enter a valid number.");
                                                                }


                                                            }
                                                            else
                                                            {
                                                                // Conversion failed
                                                                Console.WriteLine("Invalid input, make sure to input valid numbers.");
                                                            }
                                                        }
                                                    }
                                                }


                                                break;

                                            case 4:
                                                Console.WriteLine("Thank you for your time!");
                                                exitRequested = true;
                                                counter = 10;
                                                break;
                                        }
                                    }

                                    else
                                    {
                                        Console.WriteLine("Please enter only number. (1,2,3,4)");
                                    }
                                }
                            }

                            else
                            {
                                Console.WriteLine("Incorrect PIN");
                                connection.Close();
                                counter++;
                            }

                        }
                        connection.Close();
                    }
                    else
                    {
                        Console.WriteLine("Invalid input,please enter number only!");
                        connection.Close();
                    }


                }

                else
                {
                    Console.WriteLine("Card ID does not exist in the database.");
                    connection.Close();
                    counter++;
                }
            }


        }
    }
}