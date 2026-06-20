# ChatBot_Part_2
Part two of the Chat_Bot_POE

The chatbot was created to mainly support:

* Keyword-based responses
* Sentiment detection (fear, worry, anger, etc.)
* User interest tracking (with simple memory)
* File-based persistence (saving interests across sessions)
* Basic audio greeting on startup



Some Key Features Include:

1. Keyword-Based Chat Responses

The chatbot uses a `Dictionary<string, List<string>>` to match user input keywords and return randomized responses.

Example supported topics:

* phishing
* malware
* firewall
* VPN
* passwords
* cybersecurity
* hackers
* fraud
* encryption

If no keyword is found, the bot returns a default fallback response.

2. Sentiment Detection

The chatbot detects emotional keywords such as:

* worried
* anxious
* afraid
* scared
* concerned
* angry
* frustrated
* happy

It responds with supportive or calming messages depending on the detected sentiment.

3. Interest Detection System

Users can type:

“I am interested in cybersecurity”

The system:

* Extracts meaningful keywords
* Removes filler words (ignore list)
* Stores interests per user
* Saves them to a file (`interested_topic.txt`)

It then confirms:

“Great, I’ll remember that you are interested in cybersecurity.”


4. Basic Memory System 

The chatbot stores user interests in two ways:

File Storage

* Saves user interests to a text file
* Allows persistence across sessions

5. Voice Greeting (Audio Feature)

On application startup, a `.wav` file is played using:

* `MediaPlayer`
* Local file stored in project directory (`Greeting.wav`)

Challenges faced:

* File format mismatch errors (“not a WAV file”)
* Fix required ensuring proper encoding, not just renaming extension
* Audio path resolution using `AppDomain.CurrentDomain.BaseDirectory`

6. WPF Chat Interface

The UI includes the:

* Chat window (ListView)
* Username input screen
* Message input textbox
* Dynamic message display using `TextBlock` and `Run`

Messages are styled differently for the:

* User (purple)
* Bot (light blue)


7. Send on Enter Key (UX Improvement)

The project was extended to support sending messages using the **Enter key**, improving usability like a real chat application.


8. Auto-Clear Input Box

After sending a message:

* The textbox automatically clears
* Improves chat flow and prevents duplicate sending


9. Username Persistence

* Usernames are stored in `user_names.txt`
* The system checks if a user is returning
* Displays:

  * First-time greeting
  * OR welcome back message



Some of the technical challenges I faced:

This project involved several debugging and learning challenges such as:

1. NullReferenceException Errors

* Occurred when `botData` or dictionaries were not properly initialized
* Fixed by ensuring correct constructor setup:


Dictionary Logic Confusion

* Initial confusion between:

  * keyword responses (`BotResponses`)
  * safety tips (`SafetyTips`)
* Required restructuring `Class1` to expose `SafetyTips` properly



3. WAV File Was Not Being Recognised

* Renaming file extension did NOT fix issue
* Real fix: ensure correct encoding + valid WAV format

4. WPF Live Preview / Build Issues

* App occasionally launched in preview mode
* Caused confusion when debugging UI behaviour
* Resolved by rebuilding solution and running correct startup project


 5. Method and Variable Scope Errors

* Issues like:

  * “userMemory does not exist”
  * method group indexing errors
* Caused by incomplete refactoring during memory feature expansion



I also spent quite alot of time on debugging:

* Null reference issues
* Dictionary access logic
* UI event handling
* Audio file integration

#ChatBot_Part_3

1. Added Task Assistant functionality
   Users can now create cybersecurity-related reminders and keep track of important security activities and each task contains a Task title, description, optional reminder date, and a completion status.

* Created a helper method to capture the task title.
* Created a helper method to create a task description
* Created a method to capture the reminder date
* Created a method to mark the completetion status of the tasks, which are displayed in a list view so that the user can easily monitor and view any outstandong work


2. MySQL Database Intergration

* Integrated database storege for the application to store things relatesd to the tasks such as the task_ID, title, description, reminder date and completion status.
* Created a class called TaskDatabaseService.cs
* I created the database was using MySql workbench 8.0 and proceeded to intergrate it into the application through TaskDatabaseService.cs


* Created CyberTask.cs to hold the properties of the the tasks such as the id, title, ddescription, reinder date and the task status
* Created a method that allows the user to mark the task as completed
* Created a method that allows the user to delete a task

* Redesigned the UI to have a tab for the mini game and the task assistant has been intergrted into the chatbot conversation section

3. Mini game (Cyber Quiz)

* Intergrated a quiuz game into the application
* the quiz includes:
* multiple-choice questions
* four answer options
* immediate feedback
* score tracking
* progress display
* next question navigation

* Quiz navigation features include a start quiz, submit answer, and next question button

* Improved the layout by making use of nested grids

* Improved the user experience by adding additional touches such as:
* Enter key sends messages
* Chat textbox clears automatically after sending
* Automatic scrolling to the newest message
* Better spacing throughout the interface
* Improved visual organisation

* The Main Window is responsible for:
* User interface
* Event handling
* Displaying chatbot messages
* Managing user interaction

* Class 1 is responsible for:
* chatbot responses
* cybersecurity knowledge
* safety tips
* keyword detection

* CyberTask is responsible for:
* chatbot responses
* cybersecurity knowledge
* safety tips
* keyword detection

* TaskDatabaseService is responsible for:
* All sql statements and database storage

4. NLP intergration
* Added NLP intergration to ensure a smooth conversational flow.
* This required a small refactoring of the methods such as send messages to apply the change

5. Challenges faced

* The biggest challenge I faced was connecting the WPF application to MySQL because MySql required configuring of:

* MySQL Server
* MySQL Connector
* Connection Strings
* SQL Commands

* Maintaining Separation 
* As the project became larger, keeping business logic separate from UI code became increasingly important.
* Creating dedicated classes helped prevent MainWindow from becoming overly complicated.


* Refreshing the Task List
* After adding, deleting or completing a task, the interface needed to refresh immediately.
*This required reloading task data from the database after each operation to keep the display synchronized.


* Designing the Interface
*Combining the chatbot,task manager and quiz inside one application without making the interface confusing required several layout redesigns, and making use of a TabControl ultimately provided the cleanest solution.
