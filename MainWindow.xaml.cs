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
        public MainWindow()
        {
            InitializeComponent();
            PlayGreeting();
            botData = new Class1(BotResponses, IgnoreAll);
            LoadUserMemory();

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

    }
}