using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Chat_Bot_Part2_POE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        //Instance creation of the Array list
        Dictionary<string, List<string>> BotResponses = new Dictionary<string, List<string>>();
        ArrayList IgnoreAll = new ArrayList();

        string username = string.Empty;


        public MainWindow()
        {
            InitializeComponent();

          //  Dictionary<string, List<string>> BotResponses = new Dictionary<string, List<string>>();
           // ArrayList ignore = new ArrayList();

            new Class1(BotResponses, IgnoreAll);


            //logic to display the logo
            try
            {
                string logoText = File.ReadAllText("logo.txt");
                logoBlock.Text = logoText;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logo: " + ex.Message);
            }

        } //end of  MainWindow constructor


        //async is
        ///used to keep the UI responsive while the speech synthesis is happening, preventing the application from freezing during the operation.
        //method to speak the greeting message 
        private async Task SpeakGreeting()
        {
            // in order to get the chat to output seech synthesis, an Azure Speech service resource was used. 
            var config = SpeechConfig.FromSubscription("YourAzureKeyHere", "YourRegionHere");

            using var synthesizer = new SpeechSynthesizer(config);
            await synthesizer.SpeakTextAsync( "Hello there I'm Etanda Secure Cybersecurity Awareness Chat Bot, here to help you navigate the digital world safely and securely. Let's get started!"  );
        }



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

            // Get bot reply 
            string botReply = GetRandomResponse(questions);

            // Show bot reply
            error_method("ChatBot", botReply);

            // Auto scroll if need be
            chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
        }




        /*
        private void SendMessage(object sender, RoutedEventArgs e)
        {//start of send method

            
            //user input
            string questions = question.Text.ToString();

            //show what the user has typed
            error_method(username, questions);

            //if statement to check if user entered a question or not
            if (questions == "")
            {
                //call the error method
                error_method("ChatBot", "Please enter a message!");
            }
            else
            {//start of else


                //temp varaibles and arrays
                string[] words = questions.Split(' ');

                bool found = false;
                string message = string.Empty;

                Random indexer = new Random();

                ArrayList per_word = new ArrayList();
                ArrayList answers_found = new ArrayList();

                //alternate per word from the words array
                foreach (string word in words)
                {//start of the main foreach 


                    //check if the word is allowed or not
                    if (!IgnoreAll.Contains(word.ToLower()))
                    {//start of check word if

                        
                        per_word.Clear();


                        //check if the word INTERESTED has been found or not
                        if (word.ToLower().Contains("interested"))
                        {

                            //obtain that which the user is interested in and only that
                            string store_interests = string.Empty;
                            bool found_interest = false;
                            //loop through each word to find the interested word and then store the next words after it
                            foreach (string interest in words)
                            {
                                if (!IgnoreAll.Contains(interest) && interest != "interested")
                                {
                                    //then append what they are interested in
                                    found_interest = true;
                                    store_interests += interest + ", ";
                                }

                            }

                            //store the interests in a text file
                            if (found_interest)
                            { //start 

                                //filename
                                string filename = "interested_topic.txt";
                                File.AppendAllText(filename, username + " " + store_interests);
                                answers_found.Add("Great, I'll remember that you are interested in " + store_interests);

                            } //end 
                            else
                            {
                                answers_found.Add("sorry, please make sure the topics" + store_interests + "are related");
                            } //end of else

                        } //end of check interested if


                        //foreach to search for the answer of the word allowed
                        foreach (var entry in BotResponses)
                        {//start of answer loop
                            string keyword = entry.Key; //to detect the keywords to trigger the responses
                            List<string> responses = entry.Value; //the list of responses

                            //check and store
                            if (word.ToLower().Contains(keyword.ToLower()))
                            {//start of check answer if

                                found = true;

                                // then pick a random response from the list
                                int index = indexer.Next(responses.Count);
                                answers_found.Add(responses[index]);

                            }//end of check answer if

                        }//end of answer loop

                        //then check if found is true and store
                        //per random
                        if (found)
                        {//start of found if

                            //get the random indexer
                            int indexing = indexer.Next(0, per_word.Count);

                            //store one answer per word now
                            answers_found.Add(per_word[indexing]);



                        }//end of found if


                    }//end of check word if



                }//end of the main foreach


                //check and show the user the answers
                if (found)
                {//start of found if true

                    //get all of answers and show to the user
                    foreach (string per_answer in answers_found)
                    {//start of show answer loop

                        //append all message
                        message += per_answer + "\n";

                    }//end of show answer loop

                    //add the message or answers to the list view
                    //chats.Items.Add(message);
                    error_method("Chatbot", message);
                    //auto scroll
                    chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);


                }//end of found if true



            }



        }//end of send method


        */

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


        private void error_method(string name, string message)
        {//star of error mehtod

            //call the chats which is a listview
            chats.Items.Add(
                new TextBlock
                {
                    Inlines = {
                     new Run{
                     Text=name + " : ",
                     Foreground =Brushes.Blue

                     }   ,
                     new Run {
                     Text= " " + message ,
                     Foreground =Brushes.Red

                     }

                    }

                }

                );

        }



        //Method to get a random response from the ArrayList based on the user input
        private string GetRandomResponse(string userInput)
        {
            string lowerInput = userInput.ToLower();

            // Loop through the dictionary to find matching keywords and store the corresponding responses
            foreach (var entry in BotResponses)
            {


                if (lowerInput.Contains(entry.Key))
                {
                    Random random = new Random();
                    int index = random.Next(entry.Value.Count);
                    return entry.Value[index]; //to pick a random response
                }
            }

           

            // Default response if no keyword is matched
            return "I don’t have a response for that yet.";
        } //end of get random response method














    }
}