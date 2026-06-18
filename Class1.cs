using Azure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace Chat_Bot_Part2_POE
{
    public class Class1
    {
        public Dictionary<string, string> SafetyTips { get; private set; }



        public Class1(Dictionary<string, List<string>> reply, ArrayList ignore)
        {
            BotResponses(reply);
            IgnoreAll(ignore);
        


                SafetyTips = new Dictionary<string, string>
        {
            { "scams", "Always verify the sender before clicking links or sharing personal information." },

            { "phishing", "Check the email address carefully and avoid downloading attachments from unknown sources." },

            { "malware", "Keep your antivirus software updated and avoid installing software from untrusted sites." },

            { "fraud", "Be cautious of unexpected emails or messages, especially those asking for sensitive information. Always double-check the source and look for signs of phishing." }

        };
            
        
        }




        // Storing of the bot responses 
        public void BotResponses(Dictionary<string, List<string>> responses)
        {
            // Greeting responses
            responses["hi"] = new List<string>
            {
                "Hello there! Welcome to Etanda Secure, your friendly cybersecurity chatbot. Let’s dive into keeping you safe online!",
                "Hi! I’m Etanda Secure, here to guide you through the digital world with tips and tricks to stay protected.",
                "Greetings! I’m Etanda Secure, your cybersecurity awareness companion. Ready to explore how to stay secure?"
            };

            responses["hello"] = new List<string>(responses["hi"]);
            responses["hey"] = new List<string>(responses["hi"]);

            responses["purpose"] = new List<string>
            {
                "My purpose is to guide you in understanding cybersecurity risks and how to protect yourself online.",
                "I exist to raise awareness about digital safety, helping you recognize threats like phishing and avoid them.",
                "My role is to educate and support you in navigating the digital world securely, making sure you stay protected."
            };

            // phishing
            responses["phishing"] = new List<string>
            {
                "Phishing is a cyber attack where criminals trick you into giving away sensitive information, often by pretending to be a trusted source like a bank or service.",
                "Phishing happens when attackers send fake emails or messages that look real, trying to get you to click links or share personal details.",
                "Phishing is a form of online fraud where scammers disguise themselves as legitimate organizations to steal your passwords, credit card numbers, or other private data."
            };

            // cybersecurity
            responses["cybersecurity"] = new List<string>
            {
                "Cybersecurity is the practice of protecting systems, networks, and data from digital attacks, ensuring information stays safe and private.",
                "Cybersecurity involves defending computers and online services against threats like viruses, hackers, and phishing scams to keep users secure.",
                "Cybersecurity is all about safeguarding digital information and resources, making sure they remain confidential, available, and trustworthy."
            };

            // firewall
            responses["firewall"] = new List<string>
            {
                "A firewall is a security system that monitors and controls incoming and outgoing network traffic based on predetermined rules, acting like a barrier between trusted and untrusted networks.",
                "Firewalls help protect computers and networks by blocking unauthorized access while allowing safe communication, making them a key part of cybersecurity.",
                "Think of a firewall as a digital security guard—it checks data packets trying to enter or leave your network and decides whether to allow or block them based on rules."
            };
            responses["firewalls"] = new List<string>(responses["firewall"]);

            // password
            responses["password"] = new List<string>
            {
                "Passwords are secret keys that protect your accounts. Strong passwords should be long, unique, and include a mix of letters, numbers, and symbols.",
                "A password is like a digital lock. Using complex and unique passwords helps prevent hackers from breaking into your accounts.",
                "Passwords secure your personal information online. To stay safe, avoid reusing them across sites and consider using a password manager."
            };
            responses["passwords"] = new List<string>(responses["password"]);

            // vpn
            responses["vpn"] = new List<string>
            {
                "A VPN, or Virtual Private Network, creates a secure connection over the internet, protecting your data and privacy while you browse.",
                "VPNs work by encrypting your internet traffic and hiding your IP address, making it harder for hackers or websites to track you.",
                "Using a VPN helps you stay safe online, especially on public Wi-Fi, by ensuring your personal information is shielded from prying eyes."
            };

            // hackers
            responses["hackers"] = new List<string>
            {
                "Hackers are individuals who use their technical skills to gain unauthorized access to systems or data, often for malicious purposes.",
                "A hacker is someone who exploits weaknesses in computer systems or networks, sometimes to steal information or disrupt services.",
                "Hackers can be both good and bad—some break into systems illegally, while ethical hackers help organizations find and fix security flaws."
            };

            // fraud
            responses["fraud"] = new List<string>
            {
                "Fraud is when someone deliberately deceives another person or organization to gain money, services, or personal information illegally.",
                "Fraud involves tricking people or systems for dishonest gain, such as stealing identities, forging documents, or misrepresenting facts.",
                "Fraud is a criminal act where individuals or groups use lies or manipulation to exploit others, often resulting in financial or personal loss."
            };

            // malicious
            responses["malicious"] = new List<string>
            {
                "Malicious refers to harmful intent in the digital world, such as software or actions designed to damage systems or steal information.",
                "When something is described as malicious, it means it was created to cause harm—like malware, viruses, or cyber attacks targeting users.",
                "Malicious behavior in cybersecurity is any activity meant to disrupt, exploit, or compromise systems, often for personal gain or sabotage."
            };

            // social engineering
            responses["social engineering"] = new List<string>
            {
                "Social engineering is when attackers trick people into giving away information or access by manipulating trust.",
                "Instead of hacking computers, social engineering targets human psychology to gain entry or steal data.",
                "Phishing emails and fake phone calls are common examples of social engineering attacks."
            };

            // 2FA
            responses["authentication"] = new List<string>
            {
                "Two‑Factor Authentication adds an extra step to logging in, like a code sent to your phone, making accounts harder to hack.",
                "2FA combines something you know (password) with something you have (phone or app) for stronger security.",
                "Using 2FA greatly reduces the risk of attackers breaking into your accounts, even if they steal your password."
            };

            responses["authenticate"] = new List<string>(responses["authentication"]);


            // wifi
            responses["wifi"] = new List<string>
            {
                "Public Wi‑Fi can be dangerous because attackers may intercept your data on unsecured networks.",
                "Using free Wi‑Fi without protection can expose your passwords, emails, and personal information to hackers.",
                "Always be cautious on public Wi‑Fi—use a VPN or avoid accessing sensitive accounts when connected."
            };

            // encryption
            responses["encryption"] = new List<string>
            {
                "Encryption is the process of converting information into code so only authorized people can read it.",
                "Data encryption protects sensitive information by scrambling it, making it useless to attackers without the key.",
                "Encryption ensures privacy and security, whether it’s protecting messages, files, or online transactions."
            };

            // Sentiment detection
            responses["frustrated"] = new List<string>
            {
                "I can sense you’re frustrated—let’s slow down and work through this together.",
                "It sounds like this is getting on your nerves. I’m here to help make it easier.",
                "I hear your frustration. Let’s take it step by step so it feels less overwhelming."
            };

            responses["frustrating"] = new List<string>(responses["frustrated"]);


            responses["angry"] = new List<string>
            {
                "I can sense you’re angry. Let’s pause and work through this calmly together.",
                "It sounds like this situation is making you upset. I’m here to help ease the tension and find a solution.",
                "I hear the anger in your tone. Let’s take a step back and approach this in a way that feels more constructive."
            };
            responses["mad"] = new List<string>(responses["angry"]);
            responses["angering"] = new List<string>(responses["angry"]);


            responses["happy"] = new List<string>
            {
                "I can sense you’re feeling happy—that’s wonderful! Let’s keep this positive energy going.",
                "It seems like you’re in a great mood. I’m glad to share this moment with you.",
                "I hear the joy in your tone. Let’s celebrate that happiness and keep building on it."
            };

            responses["worried"] = new List<string>
            {
                "I can sense you’re feeling worried. Let’s take this one step at a time to ease the pressure.",
                "It sounds like you’re anxious. I’m here to help make things feel safer and more manageable.",
                "I hear the fear in your tone. Let’s slow down and focus on what we can control together."
            };

            responses["relief"] = new List<string>
            {
                "I can sense your relief—that’s great! Let’s take a moment to appreciate this positive outcome.",
                "It sounds like you’re feeling relieved. I’m glad things are looking up for you.",
                "I hear the relief in your tone. Let’s acknowledge this positive change and move forward with confidence."
            };
        }

        




        
            



       

        public void IgnoreAll(ArrayList ignore)
        {
            ignore.Add("a");
            ignore.Add("about");
            ignore.Add("above");
            ignore.Add("across");
            ignore.Add("after");
            ignore.Add("afterwards");
            ignore.Add("again");
            ignore.Add("against");
            ignore.Add("all");
            ignore.Add("almost");
            ignore.Add("alone");
            ignore.Add("along");
            ignore.Add("already");
            ignore.Add("also");
        } //end of ignoreAll method




      











    }
}
