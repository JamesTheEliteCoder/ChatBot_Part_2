using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Media;

namespace Chat_Bot_Part2_POE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        Dictionary<string, List<string>> BotResponses = new Dictionary<string, List<string>>();
        ArrayList IgnoreAll = new ArrayList();
        string username = string.Empty;
        MediaPlayer player = new MediaPlayer();
        private Class1 botData;
        private Dictionary<string, List<string>> userInterests = new Dictionary<string, List<string>>(); //for memory recollection
        private List<CyberTask> cyberTasks = new List<CyberTask>(); //for task management   
<<<<<<< HEAD
        private CyberTask pendingTask = null;
        private bool waitingForReminderResponse = false;
        private bool waitingForTaskTitle = false;
=======
        private bool waitingForReminderResponse = false;
        private bool waitingForTaskTitle = false;
        private bool waitingForReminderDate = false;
        // Stores a task temporarily while the bot asks follow-up questions
        private CyberTask pendingTask = null;
        // Handles saving, loading, completing, and deleting tasks in the MySQL Database
        private TaskDatabaseService taskDatabase = new TaskDatabaseService();





>>>>>>> f40da29 (Refactored Code to use Database storage)
        public MainWindow()
        {
            InitializeComponent();
            PlayGreeting();
            botData = new Class1(BotResponses, IgnoreAll);
            LoadUserMemory();
            //oad the saved tasks from te database when the app starts
            LoadTasksFromDatabase();

        } //end of  MainWindow constructor

        






        //methos to play greeting
        private void PlayGreeting()
        {
            try
            {
                string path = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Greeting.wav");

                player.Open(new Uri(path));
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error playing greeting: " + ex.Message);
            }
        } //end of PlayGreeting method







        //method to send messages and detect user interests
        private void SendMessage(object sender, RoutedEventArgs e)
        {
            string questions = question.Text.Trim();
            

            if (string.IsNullOrEmpty(questions))
            {
                error_method("ChatBot", "Please enter a message!");
                return;
            }

            // Show user input
            error_method(username, questions);

            string lower = questions.ToLower();

            // Check if the message is part of the task assistant conversation
            // If it is handled here, stop the method so normal chatbot replies do not also run
            if (TryHandleTaskConversation(questions))
            {
                question.Clear();
                return;
            }

            //First chec if there are any interests
            if (lower.Contains("interested"))
            {
                //Then run interest detection logic
                ProcessUserInput(questions);
            }

            //sentimetiment detection
            else if (lower.Contains("worried") || 
                    lower.Contains("anxious")  || 
                    lower.Contains("afraid")   || 
                    lower.Contains("concerned")||
                    lower.Contains("scared"))
            {

                string topic = "";

                if (botData != null && botData.SafetyTips != null)
                {
                    foreach (var key in botData.SafetyTips.Keys)
                    {
                        if (lower.Contains(key))
                        {
                            topic = key;
                            break;
                        }
                    }
                }
                string message = string.IsNullOrWhiteSpace(topic) 
                    ? "I understand that you're feeling anxious." : 
                    $"I understand that you're anxious about {topic}.";

                    if (botData != null &&
                        botData.SafetyTips != null &&
                        !string.IsNullOrWhiteSpace(topic) &&
                        botData.SafetyTips.ContainsKey(topic))
                    {
                        message += $" \nHere's a tip: {botData.SafetyTips[topic]}";
                    }
                    else
                    {
                        message += " I don't have specific tips for that topic, but remember to always be cautious and stay informed on all things pertaining to " + (topic) + ".";
                    }

                    error_method("ChatBot", message);
                }

                else
            {
                //Respond normally
                string botReply = GetRandomResponse(questions);


                error_method("ChatBot", botReply);

            }

            // Auto scroll if need 
            if(chats.Items.Count > 0)
            {
            chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);

            }
            question.Clear(); 
        }




      




        //method to submit the username and check if it exists or not
        private void Submit_Username(object sender, RoutedEventArgs e)
        {
            //temporary variable to store the username in a text file 
            string filename = "user_names.txt";

            //check if the filename exists or not , then auto create
            if (!File.Exists(filename))
            {
                //auto create the file using AppendAllText() function
                File.AppendAllText(filename, "auto_create\n");

            }//end 

            //temporary variables
            string name = user_name.Text.ToString();
            bool found = Check_name(name);

            //check if the user name has been found or not and write the name in a text file
            if (!found)
            {
                //write the name in a txt file
                File.AppendAllText(filename, name + "\n");

                error_method("chatbot ", "Welcome  " + name + "!" + " What can I do for you today?");
            }
            else
            {
                //welcome the user back
                error_method("chatbot ", "Welcome back " + name + "!" + " What can I do for you today?");

            } //end of else

            //hide username grid and set the chats grid to visible
            name_grid.Visibility = Visibility.Hidden;
            chat_grid.Visibility = Visibility.Visible;

            //assign the usernae to the global variable
            username = name;
        }









        //method to check name of the user
        private static bool Check_name(string name)
        { //start of check name method
            //temporary variable to store the file name of the text file that contains the names of the users
            string file_name = "user_names.txt";

            bool found = false;

            //store all the names in a text file then store them in an 1D array
            string[] names = File.ReadAllLines(file_name);

            //a foreach loop to search for the name in the file
            foreach (string name_found in names)
            { //start of loop

                //if statement to check for the user
                if (name_found.ToLower() == name.ToLower())
                {
                    //the name has been found, so set the bool vlue to true
                    found = true;

                } //end of 

            } //end of loop


            //return the status of the found or not (true or false)
            return found;
        }









        //mthod to change the font colour of the user and the chatbot
        private void error_method(string name, string message)
        {//star of error mehtod

            //call the chats which is a listview
            chats.Items.Add(
                new TextBlock
                {
                    Inlines = {
                     new Run{
                     Text=name + " : ",
                     Foreground =Brushes.Purple

                     }   ,
                     new Run {
                     Text= " " + message ,
                     Foreground =Brushes.LightBlue

                     }

                    }

                }

                );

        } //end of error_method










        //Method to get a random response from the user based on the user input
        private string GetRandomResponse(string userInput)
        {
            string lowerInput = userInput.ToLower();


            //split the sentence into words
            string[] words = lowerInput.Split(new char[]
                { ' ', ',', '.', '?', '!', ';', ':'
                
                }, StringSplitOptions.RemoveEmptyEntries);
        
            // Loop through the dictionary to find matching keywords and store the corresponding responses
            foreach (var entry in BotResponses)
            {


                if (words.Contains(entry.Key))
                {
                    Random random = new Random();
                    int index = random.Next(entry.Value.Count);
                    return entry.Value[index]; //to pick a random response
                }
            }

           

            // Default response if no keyword is matched
            return "I don’t have a response for that yet, please try rephrasing or asking a different question";
        } //end of get random response method




<<<<<<< HEAD
=======












>>>>>>> f40da29 (Refactored Code to use Database storage)
        //method to detect interests in the user input and store them in a text file
        private void ProcessUserInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                error_method("ChatBot", "Please enter a valid question.");
                return;
            }

            string[] words = input.ToLower().Split(new char[] { ' ', ',', '.', '?', '!', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
            string message = string.Empty;
            HashSet<string> ignore = new HashSet<string> { "i", "am", "in", "and", "the", "is" };

            // Logic to detect interests
            if (input.ToLower().Contains("interested"))
            {
                HashSet<string> currentInterests = new HashSet<string>();

                foreach (string word in words)
                {
                    string clean = Regex.Replace(word.ToLower().Trim(), @"[^a-zA-Z0-9\s]", "");
                    if (!ignore.Contains(clean) && clean != "interested" && clean.Length >= 3)
                    {
                        currentInterests.Add(clean);
                    }
                }

                if (currentInterests.Count > 0)
                {
                    string filename = "interested_topic.txt";
                    string store_interests = string.Join(", ", currentInterests);

                    if (File.Exists(filename))
                    {
                        var lines = File.ReadAllLines(filename).ToList();
                        bool updated = false;

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].StartsWith(username))
                            {
                                var existingSet = new HashSet<string>(
                                    lines[i].Replace(username + " interested in:", "")
                                            .Split(',')
                                            .Select(x => x.Trim())
                                            .Where(x => x != "")
                                );

                                foreach (var item in currentInterests)
                                    existingSet.Add(item);

                                lines[i] = username + " interested in: " + string.Join(", ", existingSet);
                                File.WriteAllLines(filename, lines);
                                updated = true;
                                break;
                            }
                        }

                        if (!updated)
                        {
                            lines.Add(username + " interested in: " + store_interests);
                            File.WriteAllLines(filename, lines);
                        }
                    }
                    else
                    {
                        File.WriteAllText(filename, username + " interested in: " + store_interests + Environment.NewLine);
                    }

                    message = $"Great, I’ll remember that you are interested in {store_interests}.";
                }
                else
                {
                    message = "Please specify what you're interested in (e.g., 'I am interested in cybersecurity').";
                }
            }
            else
            {
                message = "I didn’t detect any interests in your input.";
            }

            // Show the response in your chat window
            error_method("ChatBot", message);
        } //end of ProcessUserInput method







<<<<<<< HEAD
=======




>>>>>>> f40da29 (Refactored Code to use Database storage)
        //method to enable memory recollection of the user interests and display them when the user asks about them
        private void LoadUserMemory()
        {
            string filename = "interested_topic.txt";

            if (!File.Exists(filename)) return;

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                if (line.Contains("interested in:"))
                {
                    string[] parts = line.Split(new string[] { "interested in:" }, StringSplitOptions.None);

                    string user = parts[0].Trim();
                    string[] interests = parts[1].Split(',')
                                                .Select(x => x.Trim())
                                                .ToArray();

                    userInterests[user] = new List<string>(interests);
                }
            }
        } //end of LoadUserMemory method





<<<<<<< HEAD
=======











>>>>>>> f40da29 (Refactored Code to use Database storage)
        //method to handle the key down event for the question textbox to allow sending messages by pressing enter
        private void Question_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //prevent the beep sound when pressing enter
                e.Handled = true;

                //call your existing send method
                SendMessage(sender, e);
            }
        }





<<<<<<< HEAD
=======









>>>>>>> f40da29 (Refactored Code to use Database storage)
        //method to create tasks
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            //create task title
            string title = taskTitle.Text.Trim();
            //create task description
            string description = taskDescription.Text.Trim();
            //set task date
            DateTime? reminderDate = taskReminderDate.SelectedDate;

            if (string.IsNullOrWhiteSpace(title))
            {
                error_method("ChatBot", "Please enter a task title.");
                    return;
            }


            //Creation of the taask
            CyberTask newTask = new CyberTask
            {
                Title = title,
                Description = description,
                ReminderDate = reminderDate,
                IsCompleted = false
            };

<<<<<<< HEAD
            cyberTasks.Add(newTask);
            RefreshTaskList();
=======
            // Save the new task to the database, then reload the task list
            try
            {
                taskDatabase.AddTask(newTask);
                LoadTasksFromDatabase();
            }
            catch (Exception ex)
            {
                error_method("ChatBot", "Database error while adding task: " + ex.Message);
                return;
            }
>>>>>>> f40da29 (Refactored Code to use Database storage)

            string reminderMessage = reminderDate.HasValue
                ? " Reminder set for " + reminderDate.Value.ToShortDateString() + "."
                : " No reminder was set.";

            error_method("Chatbot", "Your task has been added:" + title + "." + reminderMessage);
            taskTitle.Clear();
            taskDescription.Clear();
            taskReminderDate.SelectedDate = null;

        } //end of AddTask_Click method






<<<<<<< HEAD
=======










>>>>>>> f40da29 (Refactored Code to use Database storage)
        //method to mark task as completed
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask selectedTask = taskList.SelectedItem as CyberTask;

            if (selectedTask == null)
            {
                error_method("ChatBot", "Please select a task to mark as completed.");
                return;
            }

<<<<<<< HEAD
            selectedTask.IsCompleted = true;
            RefreshTaskList();
=======
            // Update the selected task in the database, then reload the task list
            try
            {
                taskDatabase.CompleteTask(selectedTask.Id);
                LoadTasksFromDatabase();
            }
            catch (Exception ex)
            {
                error_method("ChatBot", "Database error while completing task: " + ex.Message);
                return;
            }
>>>>>>> f40da29 (Refactored Code to use Database storage)

            error_method("ChatBot", "Task marked as completed: " + selectedTask.Title + ".");
        } //end of CompleteTask_Click method






<<<<<<< HEAD
=======







>>>>>>> f40da29 (Refactored Code to use Database storage)
        //method to delete task
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask selectedTask = taskList.SelectedItem as CyberTask;

            if (selectedTask == null)
            {
                error_method("ChatBot", "Please select a task to delete.");
                return;
            }

<<<<<<< HEAD
            cyberTasks.Remove(selectedTask);
            RefreshTaskList();
=======
            // Delete the selected task from the database, then reload the task list
            try
            {
                taskDatabase.DeleteTask(selectedTask.Id);
                LoadTasksFromDatabase();
            }
            catch (Exception ex)
            {
                error_method("ChatBot", "Database error while deleting task: " + ex.Message);
                return;
            }
>>>>>>> f40da29 (Refactored Code to use Database storage)

            error_method("ChatBot", "Task deleted: " + selectedTask.Title + ".");
        }






        //method to refresh the task list
        private void RefreshTaskList()
        {
            taskList.ItemsSource = null;
            taskList.ItemsSource = cyberTasks;
        } //end of RefreshTaskList method





<<<<<<< HEAD
=======


        // Method to load all the saved tasks from the database and display them in the task list
        private void LoadTasksFromDatabase()
        {
            try
            {
                cyberTasks = taskDatabase.GetAllTasks();
                RefreshTaskList();
            }
            catch (Exception ex)
            {
                error_method("ChatBot", "Database error while loading tasks: " + ex.Message);
            }
        } //end of LoadTasksFromDatabase method








>>>>>>> f40da29 (Refactored Code to use Database storage)
        // Method to handle task-related chatbot conversations before normal chatbot responses
        private bool TryHandleTaskConversation(string input)
        {
            // Convert input to lowercase for easier keyword checks
            string lower = input.ToLower();

<<<<<<< HEAD
=======
            // If the bot is waiting for the actual reminder date, handle that here
            if (waitingForReminderDate && pendingTask != null)
            {
                DateTime? reminderDate = ExtractReminderDate(lower);

                if (!reminderDate.HasValue)
                {
                    error_method("ChatBot", "Please enter a reminder like 'in 3 days', 'tomorrow', or 'next week'.");
                    return true;
                }

                // Save the reminder date on the pending task
                pendingTask.ReminderDate = reminderDate;

                // Save the task to MySQL, then reload the list
                try
                {
                    taskDatabase.AddTask(pendingTask);
                    LoadTasksFromDatabase();
                }
                catch (Exception ex)
                {
                    error_method("ChatBot", "Database error while saving task: " + ex.Message);
                    return true;
                }

                error_method("ChatBot", "Got it! I've created the task and set the reminder.");

                // Clear the conversation state
                pendingTask = null;
                waitingForReminderDate = false;
                waitingForReminderResponse = false;

                return true;
            }

>>>>>>> f40da29 (Refactored Code to use Database storage)
            // If the bot asked for a task title, treat this message as the title
            if (waitingForTaskTitle)
            {
                string title = input.Trim();

                // Create a cybersecurity-related description for the task title
                string description = CreateTaskDescription(title);

<<<<<<< HEAD
                // Reject the task only after the user has provided the title
                if (description == null)
                {
                    error_method("ChatBot", "That task does not seem cybersecurity-related. Try a task like 'Review privacy settings' or 'Enable two-factor authentication'.");

                    // Keep waiting so the user can provide another task title
                    waitingForTaskTitle = true;
                    return true;
                }

                // Create the pending task now that the title has been accepted
=======
                if (description == null)
                {
                    error_method("ChatBot", "That task does not seem cybersecurity-related. Try a task like 'Review privacy settings' or 'Enable two-factor authentication'.");
                    return true;
                }

                // Store the task temporarily until the reminder question is answered
>>>>>>> f40da29 (Refactored Code to use Database storage)
                pendingTask = new CyberTask
                {
                    Title = title,
                    Description = description,
                    IsCompleted = false
                };

<<<<<<< HEAD
                // Move to the reminder question
                waitingForTaskTitle = false;
                waitingForReminderResponse = true;

                error_method("ChatBot", "Task added with the description \"" + pendingTask.Description + "\". Would you like a reminder?");
=======
                waitingForTaskTitle = false;
                waitingForReminderResponse = true;

                error_method("ChatBot", "Sure, I've created a task called \"" + title + "\". Would you like a reminder?");
>>>>>>> f40da29 (Refactored Code to use Database storage)

                return true;
            }

<<<<<<< HEAD
            // If the bot already asked about a reminder, handle the user's next reply here
            if (waitingForReminderResponse && pendingTask != null)
            {
                // If the user says yes, try to extract a reminder date
                if (lower.Contains("yes") || lower.Contains("remind"))
                {
                    DateTime? reminderDate = ExtractReminderDate(lower);

                    // Add the selected reminder date to the temporary task
                    pendingTask.ReminderDate = reminderDate;

                    // Save the task into the current in-memory task list
                    cyberTasks.Add(pendingTask);
                    RefreshTaskList();

                    string reminderText = reminderDate.HasValue
                        ? "in " + (reminderDate.Value.Date - DateTime.Now.Date).Days + " days"
                        : "soon";

                    error_method("ChatBot", "Got it! I'll remind you " + reminderText + ".");

                    // Clear the temporary task state
                    pendingTask = null;
                    waitingForReminderResponse = false;

                    return true;
                }

                // If the user says no, add the task without a reminder
                if (lower.Contains("no"))
                {
                    cyberTasks.Add(pendingTask);
                    RefreshTaskList();

                    error_method("ChatBot", "No problem. I've added the task without a reminder.");

                    // Clear the temporary task state
=======
            // If the bot is waiting for yes/no about a reminder, handle that here
            if (waitingForReminderResponse && pendingTask != null)
            {
                if (lower.Contains("yes"))
                {
                    waitingForReminderResponse = false;
                    waitingForReminderDate = true;

                    error_method("ChatBot", "When should I set the reminder for?");
                    return true;
                }

                if (lower.Contains("no"))
                {
                    // Save the task without a reminder
                    try
                    {
                        taskDatabase.AddTask(pendingTask);
                        LoadTasksFromDatabase();
                    }
                    catch (Exception ex)
                    {
                        error_method("ChatBot", "Database error while saving task: " + ex.Message);
                        return true;
                    }

                    error_method("ChatBot", "No problem. I've created the task without a reminder.");

                    // Clear the conversation state
>>>>>>> f40da29 (Refactored Code to use Database storage)
                    pendingTask = null;
                    waitingForReminderResponse = false;

                    return true;
                }
<<<<<<< HEAD
=======

                error_method("ChatBot", "Please answer yes or no. Would you like a reminder?");
                return true;
>>>>>>> f40da29 (Refactored Code to use Database storage)
            }

            // Detect when the user wants to create or add a task
            if (lower.Contains("create a task") || lower.Contains("add a task") || lower.Contains("add task"))
            {
<<<<<<< HEAD
                // Pull the task title out of the user's sentence
                string title = ExtractTaskTitle(input);

                // If no specific title was provided, ask the user for it
                if (string.IsNullOrWhiteSpace(title) || title.ToLower() == "for me")
                {
                    waitingForTaskTitle = true;
                    error_method("ChatBot", "Sure, what should the task be called?");
                    return true;
                }

                // Create a cybersecurity-related description for the task title
                string description = CreateTaskDescription(title);

                // Reject only after a title was actually provided
=======
                string title = ExtractTaskTitle(input);

                // If the user did not provide a clear title, ask for it
                if (string.IsNullOrWhiteSpace(title))
                {
                    waitingForTaskTitle = true;
                    error_method("ChatBot", "Sure, what would you like the task to be called?");
                    return true;
                }

                // Create a cybersecurity-related description for the provided task title
                string description = CreateTaskDescription(title);

>>>>>>> f40da29 (Refactored Code to use Database storage)
                if (description == null)
                {
                    error_method("ChatBot", "That task does not seem cybersecurity-related. Try something like 'Review privacy settings' or 'Enable two-factor authentication'.");
                    return true;
                }

<<<<<<< HEAD
                // Create the task temporarily while waiting for reminder confirmation
=======
                // Store the task temporarily until the reminder question is answered
>>>>>>> f40da29 (Refactored Code to use Database storage)
                pendingTask = new CyberTask
                {
                    Title = title,
                    Description = description,
                    IsCompleted = false
                };

<<<<<<< HEAD
                // Tell the program that the bot is now waiting for a reminder answer
                waitingForReminderResponse = true;

                error_method("ChatBot", "Task added with the description \"" + pendingTask.Description + "\". Would you like a reminder?");

=======
                waitingForReminderResponse = true;

                error_method("ChatBot", "Sure, I've created a task called \"" + title + "\". Would you like a reminder?");
>>>>>>> f40da29 (Refactored Code to use Database storage)
                return true;
            }

            // Returning false means this message was not a task command
            return false;
<<<<<<< HEAD
        } //end of TryHandleTaskConversation method
=======
        }
>>>>>>> f40da29 (Refactored Code to use Database storage)





        // Helper method to extract the task title from common task creation phrases
        private string ExtractTaskTitle(string input)
        {
            string title = input;

            // Remove common phrases so only the task name remains
            title = Regex.Replace(title, "i want you to create a task called", "", RegexOptions.IgnoreCase);
            title = Regex.Replace(title, "create a task called", "", RegexOptions.IgnoreCase);
            title = Regex.Replace(title, "add a task called", "", RegexOptions.IgnoreCase);
            title = Regex.Replace(title, "add task", "", RegexOptions.IgnoreCase);
            title = Regex.Replace(title, "called", "", RegexOptions.IgnoreCase);
            title = Regex.Replace(title, "can you create a task called", "", RegexOptions.IgnoreCase);
            title = Regex.Replace(title, "for me", "", RegexOptions.IgnoreCase);
            return title.Trim();
        }//end of ExtractTaskTitle method



        // Helper method to create a cybersecurity description based on the task title
        private string CreateTaskDescription(string title)
        {
            string lowerTitle = title.ToLower();

            // Match common cybersecurity topics and return a correct description
            if (lowerTitle.Contains("privacy"))
            {
                return "Review account privacy settings to ensure your data is protected.";
            }

            if (lowerTitle.Contains("password"))
            {
                return "Update your password and make sure it is strong, unique, and secure.";
            }

            if (lowerTitle.Contains("two-factor") || lowerTitle.Contains("2fa") || lowerTitle.Contains("authentication"))
            {
                return "Enable two-factor authentication to add an extra layer of security to your account.";
            }

            if (lowerTitle.Contains("phishing"))
            {
                return "Review phishing warning signs and avoid clicking suspicious links or attachments.";
            }

            if (lowerTitle.Contains("malware") || lowerTitle.Contains("antivirus"))
            {
                return "Check your device for malware and make sure your antivirus protection is up to date.";
            }

            if (lowerTitle.Contains("backup") || lowerTitle.Contains("back up"))
            {
                return "Back up important files so your data can be recovered if your device is lost, damaged, or attacked.";
            }

            if (lowerTitle.Contains("wifi") || lowerTitle.Contains("wi-fi") || lowerTitle.Contains("network"))
            {
                return "Review your Wi-Fi and network security settings to prevent unauthorized access.";
            }

            if (lowerTitle.Contains("software") || lowerTitle.Contains("update"))
            {
                return "Update your software to patch security vulnerabilities and keep your device protected.";
            }

            // Return null if the task is not related to cybersecurity
            return null;
        } //end of CreateTaskDescription



        // Helper method that tries to detect reminder phrases 
        private DateTime? ExtractReminderDate(string input)
        {
            // Looks for the phrases
            Match match = Regex.Match(input, @"in\s+(\d+)\s+day");

            if (match.Success)
            {
                int days = int.Parse(match.Groups[1].Value);
                return DateTime.Now.AddDays(days);
            }

            // Handle simple reminder wording
            if (input.Contains("tomorrow"))
            {
                return DateTime.Now.AddDays(1);
            }

            if (input.Contains("next week"))
            {
                return DateTime.Now.AddDays(7);
            }

            // No reminder date found
            return null;
        }



<<<<<<< HEAD
        public class CyberTask
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? ReminderDate { get; set; }
            public bool IsCompleted { get; set; }

            public string ReminderText
            {
                get
                {
                    if (ReminderDate.HasValue)
                    {
                        return ReminderDate.Value.ToShortDateString();
                    }

                    return "No reminder";
                }
            }

            public string Status
            {
                get
                {
                    return IsCompleted ? "Completed" : "Pending";
                }
            }
        }
=======
       
>>>>>>> f40da29 (Refactored Code to use Database storage)



    } //end of class
}