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
        private bool waitingForReminderResponse = false;
        private bool waitingForTaskTitle = false;
        private bool waitingForReminderDate = false;
        // Stores a task temporarily while the bot asks follow-up questions
        private CyberTask? pendingTask = null;
        // Handles saving, loading, completing, and deleting tasks in the MySQL Database
        private TaskDatabaseService taskDatabase = new TaskDatabaseService();
        //Store all the quiz questions for the mini game
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        //Track which uqestions the user is currently answering
        private int currentQuizIndex = 0;
        //Tracks how many questions the user answered correctly
        private int quizScore = 0;
        //Track if the current question has already been answered
        private bool quizAnswerSubmitted = false;
        //Store recent actions taken by the chatbot and app features
        private List<string> activityLog = new List<string>();





        public MainWindow()
        {
            InitializeComponent();
            PlayGreeting();
            botData = new Class1(BotResponses, IgnoreAll);
            LoadUserMemory();
            //load the saved tasks from te database when the app starts
            LoadTasksFromDatabase();
            LoadQuizQuestions();
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


            // Show the activity log when the user asks what the chatbot has done
            if (lower.Contains("show activity log") ||
                lower.Contains("activity log") ||
                lower.Contains("log") ||
                lower.Contains("what have you done for me"))
            {
                ShowActivityLog();
                question.Clear();
                return;
            }


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
                    lower.Contains("anxious") ||
                    lower.Contains("afraid") ||
                    lower.Contains("concerned") ||
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
            if (chats.Items.Count > 0)
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

            string reminderMessage = reminderDate.HasValue
                ? " Reminder set for " + reminderDate.Value.ToShortDateString() + "."
                : " No reminder was set.";

            error_method("Chatbot", "Your task has been added:" + title + "." + reminderMessage);
            taskTitle.Clear();
            taskDescription.Clear();
            taskReminderDate.SelectedDate = null;


                // To record the task action in the activity log
                AddActivityLog("Task added: " + title);


        } //end of AddTask_Click method
















        //method to mark task as completed
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask selectedTask = taskList.SelectedItem as CyberTask;

            if (selectedTask == null)
            {
                error_method("ChatBot", "Please select a task to mark as completed.");
                return;
            }

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

            error_method("ChatBot", "Task marked as completed: " + selectedTask.Title + ".");
        } //end of CompleteTask_Click method













        //method to delete task
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask selectedTask = taskList.SelectedItem as CyberTask;

            if (selectedTask == null)
            {
                error_method("ChatBot", "Please select a task to delete.");
                return;
            }

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

            error_method("ChatBot", "Task deleted: " + selectedTask.Title + ".");

            // To record the task action in the activity log
            AddActivityLog("Task deleted: " + selectedTask.Title);

        } //end of DeleteTask_Click method






        //method to refresh the task list
        private void RefreshTaskList()
        {
            taskList.ItemsSource = null;
            taskList.ItemsSource = cyberTasks;
        } //end of RefreshTaskList method







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

















        // Method to handle task-related chatbot conversations before normal chatbot responses
        private bool TryHandleTaskConversation(string input)
        {
            // Convert input to lowercase for easier keyword checks
            string lower = input.ToLower();

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

            // If the bot asked for a task title, treat this message as the title
            if (waitingForTaskTitle)
            {
                string title = input.Trim();

                // Create a cybersecurity-related description for the task title
                string description = CreateTaskDescription(title);

                if (description == null)
                {
                    error_method("ChatBot", "That task does not seem cybersecurity-related. Try a task like 'Review privacy settings' or 'Enable two-factor authentication'.");
                    return true;
                }

                // Store the task temporarily until the reminder question is answered
                pendingTask = new CyberTask
                {
                    Title = title,
                    Description = description,
                    IsCompleted = false
                };

                waitingForTaskTitle = false;
                waitingForReminderResponse = true;

                error_method("ChatBot", "Sure, I've created a task called \"" + title + "\". Would you like a reminder?");

                return true;
            }

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

                        // To record any chatbot-created task in the activity log
                        AddActivityLog("Task added through chatbot: " + pendingTask.Title);
                    }
                    catch (Exception ex)
                    {
                        error_method("ChatBot", "Database error while saving task: " + ex.Message);
                        return true;
                    }

                    error_method("ChatBot", "No problem. I've created the task without a reminder.");

                    // Clear the conversation state
                    pendingTask = null;
                    waitingForReminderResponse = false;

                    return true;
                }

                error_method("ChatBot", "Please answer yes or no. Would you like a reminder?");
                return true;
            }

            // Detect when the user wants to create or add a task
            if (lower.Contains("create a task") || lower.Contains("add a task") || lower.Contains("add task"))
            {
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

                if (description == null)
                {
                    error_method("ChatBot", "That task does not seem cybersecurity-related. Try something like 'Review privacy settings' or 'Enable two-factor authentication'.");
                    return true;
                }

                // Store the task temporarily until the reminder question is answered
                pendingTask = new CyberTask
                {
                    Title = title,
                    Description = description,
                    IsCompleted = false
                };

                waitingForReminderResponse = true;

                error_method("ChatBot", "Sure, I've created a task called \"" + title + "\". Would you like a reminder?");
                return true;
            }

            // Returning false means this message was not a task command
            return false;
        }













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















        //Class for the Quiz questions
        // Each represents one question in the cybersecurity quiz
        public class QuizQuestion
        {
            // The question shown to the user
            public string Question { get; set; }

            // Four possible answers
            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }

            // The correct answer letter: A, B, C, or D
            public string CorrectAnswer { get; set; }

            // Feedback shown after the user answers a question
            public string Explanation { get; set; }
        } //end of QuizQuestion class


















        // Method to load the quiz questions into a list
        private void LoadQuizQuestions()
        {
            quizQuestions = new List<QuizQuestion>
    {
        new QuizQuestion
        {
            Question = "What should you do if you receive an email asking for your password?",
            OptionA = "Reply with your password",
            OptionB = "Delete or report the email as phishing",
            OptionC = "Forward it to friends",
            OptionD = "Click the link to check if it is real",
            CorrectAnswer = "B",
            Explanation = "Correct answer: B. Legitimate services will not ask for your password by email."
        },

        new QuizQuestion
        {
            Question = "What is two-factor authentication used for?",
            OptionA = "Making your screen brighter",
            OptionB = "Adding an extra layer of account security",
            OptionC = "Deleting old files",
            OptionD = "Speeding up your internet",
            CorrectAnswer = "B",
            Explanation = "Correct answer: B. Two-factor authentication adds another proof of identity besides your password."
        },

        new QuizQuestion
        {
            Question = "Which password is the strongest?",
            OptionA = "password123",
            OptionB = "qwerty",
            OptionC = "MyDogSpot",
            OptionD = "R8!vL2#pQ9$z",
            CorrectAnswer = "D",
            Explanation = "Correct answer: D. Strong passwords are long, unique, and use a mix of characters."
        },

        new QuizQuestion
        {
            Question = "What is phishing?",
            OptionA = "A scam that tricks users into giving away sensitive information",
            OptionB = "A safe way to store passwords",
            OptionC = "A type of computer hardware",
            OptionD = "A method for cleaning your keyboard",
            CorrectAnswer = "A",
            Explanation = "Correct answer: A. Phishing tries to fool people into sharing private information."
        },

        new QuizQuestion
        {
            Question = "Why should you update software regularly?",
            OptionA = "To make your device heavier",
            OptionB = "To patch security vulnerabilities",
            OptionC = "To remove the internet",
            OptionD = "To make passwords visible",
            CorrectAnswer = "B",
            Explanation = "Correct answer: B. Updates often fix security weaknesses attackers could exploit."
        },

        new QuizQuestion
        {
            Question = "What should you do on public Wi-Fi?",
            OptionA = "Access sensitive banking sites without protection",
            OptionB = "Share your passwords openly",
            OptionC = "Use caution and avoid sensitive activity if unprotected",
            OptionD = "Disable all security settings",
            CorrectAnswer = "C",
            Explanation = "Correct answer: C. Public Wi-Fi can expose your data if the connection is not secure."
        },

        new QuizQuestion
        {
            Question = "What does malware do?",
            OptionA = "Protects your files automatically",
            OptionB = "Harms, steals, or disrupts systems and data",
            OptionC = "Improves battery life",
            OptionD = "Creates secure passwords",
            CorrectAnswer = "B",
            Explanation = "Correct answer: B. Malware is malicious software designed to cause harm."
        },

        new QuizQuestion
        {
            Question = "What is a safe browsing habit?",
            OptionA = "Clicking every pop-up advert",
            OptionB = "Ignoring browser security warnings",
            OptionC = "Checking website links before entering personal information",
            OptionD = "Downloading files from unknown websites",
            CorrectAnswer = "C",
            Explanation = "Correct answer: C. Checking links helps you avoid fake or dangerous websites."
        },

        new QuizQuestion
        {
            Question = "What should you do if you suspect an account was hacked?",
            OptionA = "Ignore it",
            OptionB = "Change the password and enable two-factor authentication",
            OptionC = "Post your password online",
            OptionD = "Use the same password everywhere",
            CorrectAnswer = "B",
            Explanation = "Correct answer: B. Changing the password and enabling 2FA helps secure the account."
        },

        new QuizQuestion
        {
            Question = "Why is password reuse risky?",
            OptionA = "One leaked password can expose multiple accounts",
            OptionB = "It makes websites load faster",
            OptionC = "It improves encryption",
            OptionD = "It blocks phishing emails",
            CorrectAnswer = "A",
            Explanation = "Correct answer: A. If one reused password is stolen, attackers can try it on other accounts."
        }
    };
        } //end of LoadQuizQuestions method














        // Starts the quiz from the first question and resets the score
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            // Make sure questions are available before starting
            if (quizQuestions.Count == 0)
            {
                error_method("ChatBot", "The quiz could not start because no questions were loaded.");
                return;
            }

            // Reset quiz progress
            currentQuizIndex = 0;
            quizScore = 0;
            quizAnswerSubmitted = false;

            // Enable quiz buttons for a new attempt
            submitAnswerButton.IsEnabled = true;
            nextQuestionButton.IsEnabled = false;

            // Display the first question
            DisplayQuizQuestion();

            // Record that the user started a quiz attempt
            AddActivityLog("Cybersecurity quiz started.");
        } //end of StartQuiz_Click method












        // Method to display the current quiz question and answer options
        private void DisplayQuizQuestion()
        {
            // Get the current question from the list
            QuizQuestion currentQuestion = quizQuestions[currentQuizIndex];

            // Update the question progress
            quizProgressText.Text = "Question " + (currentQuizIndex + 1) + " of " + quizQuestions.Count +
                                    " | Score: " + quizScore;

            // Display the question and answer choices
            quizQuestionText.Text = currentQuestion.Question;
            answerOptionA.Content = "A) " + currentQuestion.OptionA;
            answerOptionB.Content = "B) " + currentQuestion.OptionB;
            answerOptionC.Content = "C) " + currentQuestion.OptionC;
            answerOptionD.Content = "D) " + currentQuestion.OptionD;

            // Clear previous answer selection and feedback
            answerOptionA.IsChecked = false;
            answerOptionB.IsChecked = false;
            answerOptionC.IsChecked = false;
            answerOptionD.IsChecked = false;
            quizFeedbackText.Text = "";

            // Allow the user to submit the current question
            quizAnswerSubmitted = false;
            submitAnswerButton.IsEnabled = true;
            nextQuestionButton.IsEnabled = false;
        } //end of DisplayQuizQuestion method












        // Method to check the selected answer and give immediate feedback
        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            // Prevent the same question from being submitted more than once
            if (quizAnswerSubmitted)
            {
                return;
            }

            // Find the answer that the user selected
            string selectedAnswer = GetSelectedQuizAnswer();

            if (string.IsNullOrWhiteSpace(selectedAnswer))
            {
                quizFeedbackText.Text = "Please select an answer before submitting.";
                return;
            }

            QuizQuestion currentQuestion = quizQuestions[currentQuizIndex];

            // Compare that selected answer with the correct answer
            if (selectedAnswer == currentQuestion.CorrectAnswer)
            {
                quizScore++;
                quizFeedbackText.Text = "Correct! " + currentQuestion.Explanation;
            }
            else
            {
                quizFeedbackText.Text = "Incorrect. " + currentQuestion.Explanation;
            }

            // Mark the question as answered
            quizAnswerSubmitted = true;

            // Update the score display immediately
            quizProgressText.Text = "Question " + (currentQuizIndex + 1) + " of " + quizQuestions.Count +
                                    " | Score: " + quizScore;

            // Disable the submit button and allow moving to the next question
            submitAnswerButton.IsEnabled = false;
            nextQuestionButton.IsEnabled = true;
        } // end of SubmitAnswer_Click method
















        // Method to move onto the next question or end the quiz if the user chooses to 
        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            // Move to the next question
            currentQuizIndex++;

            // If there are more questions, display the next one
            if (currentQuizIndex < quizQuestions.Count)
            {
                DisplayQuizQuestion();
                return;
            }

            // End the quiz after the last question
            EndQuiz();
        } // end of NextQuestion_Click method









        // Method to get the selected multiple-choice answer 
        private string GetSelectedQuizAnswer()
        {
            if (answerOptionA.IsChecked == true)
            {
                return "A";
            }

            if (answerOptionB.IsChecked == true)
            {
                return "B";
            }

            if (answerOptionC.IsChecked == true)
            {
                return "C";
            }

            if (answerOptionD.IsChecked == true)
            {
                return "D";
            }

            // If no answer is selected
            return "";
        } //end of GetSelectedQuizAnswer method
















        // Method to show the final quiz score and give performance feedback
        private void EndQuiz()
        {
            // Disable quiz action buttons after the quiz ends
            submitAnswerButton.IsEnabled = false;
            nextQuestionButton.IsEnabled = false;

            // Send a final feedback message based on the user's score
            string finalMessage;

            if (quizScore >= 8)
            {
                finalMessage = "Great job! You're a cybersecurity pro.";
            }
            else if (quizScore >= 5)
            {
                finalMessage = "Good effort! You know some key cybersecurity ideas, but keep practicing.";
            }
            else
            {
                finalMessage = "Keep learning to stay safe online. Cybersecurity takes practice.";
            }

            // Show the final score in the quiz tab
            quizProgressText.Text = "Quiz complete! Final score: " + quizScore + " out of " + quizQuestions.Count;
            quizQuestionText.Text = finalMessage;
            quizFeedbackText.Text = "Click Start Quiz if you want to try again.";

            // Record final quiz result in the activity log 
            AddActivityLog("Cybersecurity quiz completed. Score: " + quizScore + " out of " + quizQuestions.Count);

            // Clear answer options
            answerOptionA.Content = "";
            answerOptionB.Content = "";
            answerOptionC.Content = "";
            answerOptionD.Content = "";

            answerOptionA.IsChecked = false;
            answerOptionB.IsChecked = false;
            answerOptionC.IsChecked = false;
            answerOptionD.IsChecked = false;
        } //end of EndQuiz method









        // Method to add a timestamped action to the activity log
        private void AddActivityLog(string action)
        {
            // Store the action with the current date and time
            string logEntry = DateTime.Now.ToString("yyyy-MM-dd HH:mm") + " - " + action;

            activityLog.Add(logEntry);
        } //end of AddActivityLog method












        // Method to displays the most recent activity log entries in the chatbot
        private void ShowActivityLog()
        {
            if (activityLog.Count == 0)
            {
                error_method("ChatBot", "No activity has been recorded yet.");
                return;
            }

            // Show only the last 20 actions to keep the response short
            List<string> recentActions = activityLog
                .Skip(Math.Max(0, activityLog.Count - 20))
                .ToList();

            string message = "Here's a summary of recent actions:\n";

            for (int i = 0; i < recentActions.Count; i++)
            {
                message += (i + 1) + ". " + recentActions[i] + "\n";
            }

            error_method("ChatBot", message.Trim());
        } //end of ShowActivityLog method
























    } //end of class
}