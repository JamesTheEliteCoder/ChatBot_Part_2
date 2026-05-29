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

