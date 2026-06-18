using System;

namespace Chat_Bot_Part2_POE
{
    // This class represents one cybersecurity task in the app and database
    public class CyberTask
    {
        // Database ID for the task
        public int Id { get; set; }

        // Task title shown in the task list
        public string Title { get; set; }

        // Task description shown in the task list
        public string Description { get; set; }

        // Optional reminder date for the task
        public DateTime? ReminderDate { get; set; }

        // Tracks whether the task is completed
        public bool IsCompleted { get; set; }

        // Displays a readable reminder date in the ListView
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

        // Displays the task status in the ListView
        public string Status
        {
            get
            {
                return IsCompleted ? "Completed" : "Pending";
            }
        }
    }
}